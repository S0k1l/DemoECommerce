using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDto productDto) => new()
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Price = productDto.Price,
            Quantity = productDto.Quantity
        };

        public static ProductDto? ToDto(Product product) =>
            new ProductDto(product.Id, product.Name!, product.Quantity, product.Price);

        public static IEnumerable<ProductDto>? ToDto(IEnumerable<Product> products) => 
            products
                .Select(p => new ProductDto(p.Id, p.Name!, p.Quantity, p.Price))
                .ToList();
    }
}
