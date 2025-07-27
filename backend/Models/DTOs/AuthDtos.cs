using System.ComponentModel.DataAnnotations;

namespace Wishlist.Models.DTOs;

public record RegisterDto(
    [EmailAddress] string Email,
    [MinLength(3)] string Username,
    [MinLength(8)] string Password,
    [MinLength(8)] string ConfirmPassword
);

public record LoginDto(
    string Username,
    string Password
);

public record AuthResponseDto(
    string Token,
    string Username,
    string Email,
    Guid UserId
);