using System.ComponentModel.DataAnnotations;

namespace Wishlist.Models.DTOs;

public record CreateWishlistDto(
    [Required, MaxLength(200)] string Title,
    [MaxLength(1000)] string? Description = null,
    bool IsPublic = false
);

public record AddWishlistItemDto(
    Guid ProductId,
    decimal? GlobalTargetPrice = null
);

public record UpdateWishlistItemDto(
    decimal? GlobalTargetPrice = null,
    bool? IsBought = null
);

public record WishlistDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<WishlistItemDto> Items
);

public record WishlistItemDto(
    Guid Id,
    ProductDto Product,
    decimal? GlobalTargetPrice,
    bool IsBought,
    DateTime CreatedAt
);
