
using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDto orderDto) => 
            new()
            {
                Id = orderDto.Id,
                ProductId = orderDto.ProductId,
                ClientId = orderDto.ClientId,
                PurchaseQuantity = orderDto.PurchaseQuantity,
                OrderedDate = orderDto.OrderedDate,
            };

        public static OrderDto? ToDto(Order order) =>
            new(order.Id, order.ProductId, order.ClientId, order.PurchaseQuantity, order.OrderedDate);

        public static IEnumerable<OrderDto>? ToDto(IEnumerable<Order> order) =>
            order.Select(o =>
                new OrderDto(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderedDate))
            .ToList();

    }
}
