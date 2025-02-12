using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record OrderDetailsDto(
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int ClientId,
        [Required] string ClientName,
        [Required, EmailAddress] string ClientEmail,
        [Required] string ClientAddress,
        [Required] string PhoneNumber,
        [Required] string ProductName,
        [Required] int PurchaseQuantity,
        [Required, DataType(DataType.Currency)] decimal UnitPrice,
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderedDate);
}
