using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace UnitTests.ProductApiSolution.Repositories
{
    public class ProductRepositoryTest
    {
        private async Task<ProductDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ProductDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Products.CountAsync() == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.Products.Add(
                      new Product()
                      {
                          Name = "Product " + i.ToString(),
                          Price = (decimal)(i * 0.1),
                          Quantity = i * 2,
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists_ReturnErrorResponse()
        {
            //Arrange
            var product = new Product{ Name = "Product 1" };

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act

            var result = await _productRepository.CreateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be($"{product.Name} already added");
        }

        [Fact]
        public async Task CreateAsync_WhenProductIsCreatedSuccessfully_AddProductAndReturnsSuccessfulResponse()
        {
            //Arrange
            var product = new Product { Name = "Product 99" };

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.CreateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be($"{product.Name} is added successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnNotFoundErrorResponse()
        {
            //Arrange
            var product = new Product { Name = "Not Existing Product" };

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.DeleteAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be($"{product.Name} not found");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnProduct()
        {
            //Arrange
            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            int productId = 2;

            //Act
            var result = await _productRepository.FindByIdAsync(productId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Product>();
            result.Id.Should().Be(2);
            result.Name.Should().Be("Product 2");
            result.Price.Should().Be((decimal)(2 * 0.1));
            result.Quantity.Should().Be(2 * 2);
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNotFoundErrorResponse()
        {
            //Arrange
            int productId = 103;

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.FindByIdAsync(productId);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsIsFound_ReturnListOfProducts()
        {
            //Arrange
            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            //Arrange
            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.GetByAsync(p => p.Name == "Product 5");

            //Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Product 5");
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            //Arrange
            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.GetByAsync(p => p.Name == "Name");

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsUpdatedSuccessfully_ReturnSuccessfulResponse()
        {
            //Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Name",
                Price = 10.5m,
                Quantity = 20,
            };

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.UpdateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be($"{product.Name} is updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsNotFound_ReturnNotFoundErrorResponse()
        {
            //Arrange
            var product = new Product
            {
                Id = 999,
                Name = "Name",
                Price = 10.5m,
                Quantity = 20,
            };

            var context = await GetDbContext();
            var _productRepository = new ProductRepository(context);

            //Act
            var result = await _productRepository.UpdateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be($"{product.Name} not found");
        }
    }
}
