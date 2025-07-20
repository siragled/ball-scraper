using System.ComponentModel.DataAnnotations;

namespace Wishlist.Models.DTOs;

public record RegisterDto(
    [EmailAddress] string Email,
    [MinLength(8)] string Password,
    [MinLength(8)] string ConfirmPassword
);

public record LoginDto(
    [EmailAddress] string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    string Email,
    Guid UserId
);