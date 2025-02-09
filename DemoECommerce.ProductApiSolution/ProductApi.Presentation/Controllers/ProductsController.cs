using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            if (!products.Any()) 
                return NotFound("No product detected in database");

            var productDtos = ProductConversion.ToDto(products);
            return productDtos!.Any() ? Ok(productDtos) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);

            if(product is null)
                return NotFound("No product detected in database");

            var productDto = ProductConversion.ToDto(product);
            return productDto is not null ? Ok(productDto) : NotFound("No product found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = ProductConversion.ToEntity(productDto);
            var result = await _productRepository.CreateAsync(product);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = ProductConversion.ToEntity(productDto);
            var result = await _productRepository.UpdateAsync(product);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);

            if (product is null) return NotFound("No product detected in database");

            var result = await _productRepository.DeleteAsync(product);

            return result.Flag ? Ok(result) : BadRequest(result);
        }
    }
}
