using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wishlist.Services;
using Wishlist.Models.DTOs;

namespace Wishlist.Controllers;

[ApiController]
[Route("users/me/wishlists")]
[Authorize]
[Produces("application/json")]
public class WishlistsController : ControllerBase
{
    private readonly WishlistService _wishlistService;

    public WishlistsController(WishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpPost]
    public async Task<ActionResult<WishlistDto>> CreateWishlist(CreateWishlistDto dto)
    {
        var userId = GetUserId();
        var wishlist = await _wishlistService.CreateWishlistAsync(userId, dto);
        var wishlistDto = MapToDto(wishlist);

        return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.Id }, wishlistDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistDto>>> GetWishlists()
    {
        var userId = GetUserId();
        var wishlists = await _wishlistService.GetUserWishlistsAsync(userId);
        var wishlistDtos = wishlists.Select(MapToDto);

        return Ok(wishlistDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WishlistDto>> GetWishlist(Guid id)
    {
        var userId = GetUserId();
        var wishlist = await _wishlistService.GetWishlistByIdAsync(id, userId);

        if (wishlist == null)
            return NotFound();

        return Ok(MapToDto(wishlist));
    }

    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<WishlistItemDto>> AddWishlistItem(Guid id, AddWishlistItemDto dto)
    {
        try
        {
            var userId = GetUserId();
            var item = await _wishlistService.AddItemToWishlistAsync(id, userId, dto);
            var itemDto = MapToDto(item);

            return CreatedAtAction(nameof(GetWishlistItem),
                new { wishlistId = id, itemId = item.Id }, itemDto);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPatch("{wishlistId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateWishlistItem(
        Guid wishlistId,
        Guid itemId,
        UpdateWishlistItemDto dto)
    {
        var userId = GetUserId();
        var updated = await _wishlistService.UpdateWishlistItemAsync(itemId, userId, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{wishlistId:guid}/items/{itemId:guid}", Name = nameof(GetWishlistItem))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult GetWishlistItem(Guid wishlistId, Guid itemId)
    {
        return Ok();
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdString!);
    }

    private static WishlistDto MapToDto(Models.Wishlist wishlist) => new(
        wishlist.Id,
        wishlist.Title,
        wishlist.Description,
        wishlist.IsPublic,
        wishlist.CreatedAt,
        wishlist.UpdatedAt,
        wishlist.Items.Select(MapToDto).ToList()
    );

    private static WishlistItemDto MapToDto(Models.WishlistItem item) => new(
        item.Id,
        new ProductDto(
            item.Product.Id,
            item.Product.Name,
            item.Product.Description,
            item.Product.ImageUrl,
            item.Product.Brand,
            item.Product.SourceUrl,
            item.Product.StoreName,
            item.Product.LastPrice,
            item.Product.UsualPrice,
            item.Product.IsOnSale,
            item.Product.IsInStock,
            item.Product.CreatedAt
        ),
        item.GlobalTargetPrice,
        item.IsBought,
        item.CreatedAt
    );
}
