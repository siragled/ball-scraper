using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Wishlist.Models;
using Wishlist.Models.DTOs;
using Wishlist.Services;

namespace Wishlist.Controllers;

[ApiController]
[Route("users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtService _jwtService;

    public UsersController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        var token = _jwtService.GenerateToken(user);
        var response = new AuthResponseDto(token, user.UserName!, user.Email!, user.Id);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Username)
            ?? await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            return BadRequest(new { Error = "Invalid credentials" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return BadRequest(new { Error = "Invalid credentials" });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponseDto(token, user.UserName!, user.Email!, user.Id));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponseDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new AuthResponseDto(null, user.UserName!, user.Email!, user.Id));
    }

    [HttpGet("{id:guid}", Name = nameof(GetUser))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult GetUser(Guid id)
    {
        return Ok();
    }
}