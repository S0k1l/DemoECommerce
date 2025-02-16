using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderRepository _orderRepository;

        public OrderServiceTest() 
        {
            _orderRepository = A.Fake<IOrderRepository>();
        }

        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
        }

        private static HttpClient CreateFakeHttpClient(object option)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK) 
            {
                Content = JsonContent.Create(option)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler) 
            {
                BaseAddress = new Uri("http://localhost")
            };

            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            //Arrange
            int productId = 1;
            var productDto = new ProductDto(1, "Product 1", 20, 10.50m);
            var _httpClient = CreateFakeHttpClient(productDto);

            var _orderService = new OrderService(null!, _httpClient, null!); 
            
            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            //Arrange
            int productId = 1;
            var _httpClient = CreateFakeHttpClient(null!);

            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersByClientId_OrdersExists_ReturnsListOfOrderDetails()
        {
            //Arrange
            int clientId = 1;
            var orders = new List<Order>
            {
                new (){Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow},
                new (){Id = 2, ProductId = 2, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow},
            };

            A.CallTo(() => _orderRepository.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var _orderService = new OrderService(_orderRepository, null!, null!);

            //Act
            var result = await _orderService.GetOrdersByClientId(clientId);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}
