using Microsoft.EntityFrameworkCore;
using Wishlist.Data;
using Wishlist.Models;
using Wishlist.Models.DTOs;

namespace Wishlist.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Brand = dto.Brand,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> SearchProductsAsync(string? query, int skip = 0, int take = 20)
    {
        var queryable = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(p =>
                EF.Functions.ILike(p.Name, $"%{query}%") ||
                EF.Functions.ILike(p.Brand ?? "", $"%{query}%")
            );
        }

        return await queryable
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
