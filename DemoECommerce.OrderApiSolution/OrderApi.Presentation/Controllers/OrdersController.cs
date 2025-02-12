using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(
        IOrderRepository orderRepository,
        IOrderService orderService)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await orderRepository.GetAllAsync();

            if (orders is null || !orders.Any() )
                return NotFound("No orders was detected in the database");

            var ordersDto = OrderConversion.ToDto(orders);

            return ordersDto!.Any() ? Ok(ordersDto) : NotFound("No orders detected in the database");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await orderRepository.FindByIdAsync(id);

            if (order is null)
                return NotFound("No order was detected in the database");

            var orderDto = OrderConversion.ToDto(order);

            return orderDto is not null ? Ok(orderDto) : NotFound("No order was detected in the database");
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provided");

            var orders = await orderService.GetOrdersByClientId(clientId);

            return orders.Any() ? Ok(orders) : NotFound("No order was detected in the database");
        }

        [HttpGet("{orderId:int}/details")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
        {
            if(orderId <= 0) return BadRequest("Invalid data provided");

            var orderDetails = await orderService.GetOrderDetails(orderId);

            return orderDetails is not null ? Ok(orderDetails) : NotFound("No order was detected in the database");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getOrder = await orderRepository.FindByIdAsync(orderDto.Id);

            if (getOrder is not null)
                return BadRequest("Order already placed");

            var order = OrderConversion.ToEntity(orderDto);

            var result = await orderRepository.CreateAsync(order);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var getOrder = await orderRepository.FindByIdAsync(orderDto.Id);
            var orders = OrderConversion.ToEntity(orderDto);
            var result = await orderRepository.UpdateAsync(orders);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var order = await orderRepository.FindByIdAsync(id);

            if (order is null)
                return NotFound("No order was detected in the database");

            var result = await orderRepository.DeleteAsync(order);

            return result.Flag ? Ok(result) : BadRequest(result);
        }
    }
}
