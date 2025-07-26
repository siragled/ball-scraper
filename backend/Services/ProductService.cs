using Microsoft.EntityFrameworkCore;
using Wishlist.Data;
using Wishlist.Models;
using Wishlist.Models.DTOs;
using Wishlist.Models.Common;
using Wishlist.Extensions;
using Wishlist.Services.Scrapers;

namespace Wishlist.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ScraperService _scraperService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        ApplicationDbContext context,
        ScraperService scraperService,
        ILogger<ProductService> logger)
    {
        _context = context;
        _scraperService = scraperService;
        _logger = logger;
    }

    public async Task<Product> CreateProductFromUrlAsync(string sourceUrl)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.SourceUrl == sourceUrl);

        if (existingProduct != null)
        {
            return existingProduct;
        }

        var scrapedData = await _scraperService.ScrapeProductAsync(sourceUrl);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = scrapedData?.Name,
            Description = scrapedData?.Description.Truncate(1000),
            ImageUrl = scrapedData?.ImageUrl,
            Brand = scrapedData?.Brand,
            SourceUrl = sourceUrl,
            StoreName = "", //TODO: fetch based on scraper
            LastPrice = scrapedData?.Price ?? 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<PagedResult<Product>> GetProductsAsync(ProductsRequest request)
    {
        var query = _context.Products.AsQueryable();

        query = query.ApplySearch(request.Search,
            p => p.Name,
            p => p.Brand ?? "",
            p => p.Description ?? "");

        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            query = query.Where(p => EF.Functions.ILike(p.Brand ?? "", $"%{request.Brand}%"));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.LastPrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.LastPrice <= request.MaxPrice.Value);
        }

        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? nameof(Product.Name) : request.SortBy;
        query = query.ApplySort(sortBy, request.SortDirection);

        return await query.ToPagedResultAsync(request);
    }
}