using Microsoft.EntityFrameworkCore;
using Wishlist.Data;
using Wishlist.Models;
using Wishlist.Models.DTOs;

namespace Wishlist.Services;

public class WishlistService
{
    private readonly ApplicationDbContext _context;

    public WishlistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Wishlist> CreateWishlistAsync(Guid userId, CreateWishlistDto dto)
    {
        var wishlist = new Models.Wishlist
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
            UserId = userId
        };

        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();

        return wishlist;
    }

    public async Task<List<Models.Wishlist>> GetUserWishlistsAsync(Guid userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(w => w.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Models.Wishlist?> GetWishlistByIdAsync(Guid id, Guid userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
    }

    public async Task<WishlistItem> AddItemToWishlistAsync(Guid wishlistId, Guid userId, AddWishlistItemDto dto)
    {
        var wishlist = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

        if (wishlist == null)
            throw new UnauthorizedAccessException("Wishlist not found or access denied");

        var existingItem = await _context.WishlistItems
            .FirstOrDefaultAsync(i => i.WishlistId == wishlistId && i.ProductId == dto.ProductId);

        if (existingItem != null)
            throw new InvalidOperationException("Product already exists in wishlist");

        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            WishlistId = wishlistId,
            ProductId = dto.ProductId,
            GlobalTargetPrice = dto.GlobalTargetPrice
        };

        _context.WishlistItems.Add(item);
        await _context.SaveChangesAsync();

        return await _context.WishlistItems
            .Include(i => i.Product)
            .FirstAsync(i => i.Id == item.Id);
    }

    public async Task<bool> UpdateWishlistItemAsync(Guid itemId, Guid userId, UpdateWishlistItemDto dto)
    {
        var item = await _context.WishlistItems
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.Wishlist.UserId == userId);

        if (item == null)
            return false;

        if (dto.GlobalTargetPrice.HasValue)
            item.GlobalTargetPrice = dto.GlobalTargetPrice;

        if (dto.IsBought.HasValue)
            item.IsBought = dto.IsBought.Value;

        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}