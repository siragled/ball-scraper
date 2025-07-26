using System.ComponentModel.DataAnnotations;
using Wishlist.Models.Common;

namespace Wishlist.Models.DTOs;

public record CreateProductDto(
    [Required, Url] string SourceUrl
);

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string? Brand,
    string SourceUrl,
    string? StoreName,
    decimal LastPrice,
    DateTime CreatedAt
);

public record ProductsRequest : PagedRequest
{
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}