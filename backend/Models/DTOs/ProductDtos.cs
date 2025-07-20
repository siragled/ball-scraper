using System.ComponentModel.DataAnnotations;

namespace Wishlist.Models.DTOs;

public record CreateProductDto(
    [Required, MaxLength(500)] string Name,
    [MaxLength(2000)] string? Description = null,
    string? ImageUrl = null,
    [MaxLength(200)] string? Brand = null
);

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string? Brand,
    DateTime CreatedAt
);