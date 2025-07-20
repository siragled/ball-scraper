using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Wishlist.Models;
using Wishlist.Models.DTOs;
using Wishlist.Services;

namespace Wishlist.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtService _jwtService;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        var token = _jwtService.GenerateToken(user);
        var response = new AuthResponseDto(token, user.Email!, user.Id);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
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

        return Ok(new AuthResponseDto(token, user.Email!, user.Id));
    }

    [HttpGet("users/{id:guid}", Name = nameof(GetUser))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult GetUser(Guid id)
    {
        return Ok();
    }
}
