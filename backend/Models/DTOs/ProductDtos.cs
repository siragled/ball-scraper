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
    decimal? UsualPrice,
    bool IsOnSale,
    bool IsInStock,
    DateTime CreatedAt
);

public record ProductSnapshotDto(
    Guid Id,
    DateTime CreatedAt,
    decimal Price,
    decimal? UsualPrice,
    bool IsOnSale,
    bool IsInStock
);

public record ProductsRequest : PagedRequest
{
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public record ProductSnapshotsRequest : PagedRequest { }