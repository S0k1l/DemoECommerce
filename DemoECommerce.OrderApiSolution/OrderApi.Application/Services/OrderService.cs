using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        public async Task<ProductDto> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
            return product!;
        }

        public async Task<AppUserDto> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var user = await getUser.Content.ReadFromJsonAsync<AppUserDto>();
            return user!;
        }
        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            var order = await orderRepository.FindByIdAsync(orderId);
            if(order is null || order.Id == 0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDto(
                order.Id,
                productDto.Id,
                appUserDto.Id,
                appUserDto.Name,
                appUserDto.Email,
                appUserDto.Address,
                appUserDto.PhoneNumber,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.Price,
                order.PurchaseQuantity * productDto.Price,
                order.OrderedDate);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderRepository.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any())
                return null!;

            var ordersDtos = OrderConversion.ToDto(orders);
            return ordersDtos!;
        }
    }
}
