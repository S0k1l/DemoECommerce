using ECommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTests.ProductApiSolution.Controllers
{
    public class ProductsControllerTest
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductsController _productsController;

        public ProductsControllerTest()
        {
            _productRepository = A.Fake<IProductRepository>();
            _productsController = new ProductsController(_productRepository);
        }

        [Fact]
        public async Task GetProducts_WhenProductExists_ReturnOkResponseWithProduct()
        {
            //Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Name 1", Price = 100.70m, Quantity = 10 },
                new Product { Id = 2, Name = "Name 2", Price = 150m, Quantity = 200 },
            };

            A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

            //Act
            var result = await _productsController.GetProducts();

            //Assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDto>;

            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProducts_WhenNoProductExists_ReturnNotFoundResponse()
        {
            //Arrange
            var products = new List<Product>();

            A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

            //Act
            var result = await _productsController.GetProducts();

            //Assert
            var notFoundResult = result.Result as NotFoundObjectResult;

            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;

            message.Should().NotBeNull();
            message.Should().Be("No product detected in database");
        }

        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            //Arrange
            var productDto = new ProductDto(1,"Name 1", 0 ,0);

            //Act 
            var result = await _productsController.CreateProduct(productDto);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;

            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSuccessful_ReturnOkResponse()
        {
            //Arrange
            var productDto = new ProductDto(0, "Name 1", 10, 150.50m);
            var response = new Response(true, $"{productDto.Name} is added successfully");

            A.CallTo(() => _productRepository.CreateAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.CreateProduct(productDto);

            //Assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeTrue();
            responseResult.Message.Should().Be($"{productDto.Name} is added successfully");
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsFailed_ReturnBadRequest()
        {
            //Arrange
            var productDto = new ProductDto(0, "Name 1", 10, 150.50m);
            var response = new Response(false, $"Error occurred while adding {productDto.Name}");

            A.CallTo(() => _productRepository.CreateAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.CreateProduct(productDto);

            //Assert
            var BadRequestResult = result.Result as BadRequestObjectResult;

            BadRequestResult.Should().NotBeNull();
            BadRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = BadRequestResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeFalse();
            responseResult.Message.Should().Be($"Error occurred while adding {productDto.Name}");
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnOkResponse()
        {
            //Arrange
            var productDto = new ProductDto(1, "Name 1", 10, 150.50m);
            var response = new Response(true, $"{productDto.Name} is updated successfully");

            A.CallTo(() => _productRepository.UpdateAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.UpdateProduct(productDto);

            //Assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeTrue();
            responseResult.Message.Should().Be($"{productDto.Name} is updated successfully");
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsFailed_ReturnBadRequest()
        {
            //Arrange
            var productDto = new ProductDto(1, "Name 1", 10, 150.50m);
            var response = new Response(false, "Error occurred while updating product");

            A.CallTo(() => _productRepository.UpdateAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.UpdateProduct(productDto);

            //Assert
            var BadRequestResult = result.Result as BadRequestObjectResult;

            BadRequestResult.Should().NotBeNull();
            BadRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = BadRequestResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeFalse();
            responseResult.Message.Should().Be("Error occurred while updating product");
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccessful_ReturnOkResponse()
        {
            //Arrange
            int ProductId = 1;
            var response = new Response(true, "Product is deleted successfully");

            A.CallTo(() => _productRepository.DeleteAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.DeleteProduct(ProductId);

            //Assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeTrue();
            responseResult.Message.Should().Be("Product is deleted successfully");
        }
        [Fact]
        public async Task DeleteProduct_WhenDeletedIsFailed_ReturnBadRequest()
        {
            //Arrange
            int ProductId = 1;
            var response = new Response(false, "Error occurred while deleting product");

            A.CallTo(() => _productRepository.DeleteAsync(A<Product>.Ignored)).Returns(response);

            //Act 
            var result = await _productsController.DeleteProduct(ProductId);

            //Assert
            var BadRequestResult = result.Result as BadRequestObjectResult;

            BadRequestResult.Should().NotBeNull();
            BadRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = BadRequestResult.Value as Response;

            responseResult.Should().NotBeNull();
            responseResult.Flag.Should().BeFalse();
            responseResult.Message.Should().Be("Error occurred while deleting product");
        }
    }
}
