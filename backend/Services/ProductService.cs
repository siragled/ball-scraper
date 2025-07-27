using Microsoft.EntityFrameworkCore;
using Wishlist.Data;
using Wishlist.Models;
using Wishlist.Models.Common;
using Wishlist.Models.DTOs;
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

    public async Task<Product> GetOrCreateProductFromUrlAsync(string sourceUrl)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.SourceUrl == sourceUrl);

        if (existingProduct != null)
        {
            return existingProduct;
        }

        var scrapedData = await _scraperService.ScrapeProductAsync(sourceUrl);
        if (scrapedData == null || string.IsNullOrWhiteSpace(scrapedData.Name))
        {
            throw new InvalidOperationException("Could not scrape product data from the provided URL.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = scrapedData.Name,
            Description = scrapedData.Description.Truncate(1000),
            ImageUrl = scrapedData.ImageUrl,
            Brand = scrapedData.Brand,
            SourceUrl = sourceUrl,
            StoreName = new Uri(sourceUrl).Host,
            CreatedAt = DateTime.UtcNow,
        };

        UpdateProductFromScrapedData(product, scrapedData);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateProductAsync(Guid productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return null;

        var scrapedData = await _scraperService.ScrapeProductAsync(product.SourceUrl);
        if (scrapedData == null || string.IsNullOrWhiteSpace(scrapedData.Name))
        {
            _logger.LogWarning("Failed to re-scrape product {ProductId} from {SourceUrl}", productId, product.SourceUrl);
            return null;
        }

        UpdateProductFromScrapedData(product, scrapedData);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated product {ProductId}", productId);
        return product;
    }

    private void UpdateProductFromScrapedData(Product product, ScrapedProductData scrapedData)
    {
        var priceChanged = product.LastPrice != (scrapedData.Price ?? 0m);
        var stockChanged = product.IsInStock != scrapedData.IsInStock;

        product.Name = scrapedData.Name;
        product.Description = scrapedData.Description.Truncate(1000);
        product.ImageUrl = scrapedData.ImageUrl;
        product.LastPrice = scrapedData.Price ?? 0m;
        product.UsualPrice = scrapedData.UsualPrice;
        product.IsOnSale = scrapedData.IsOnSale;
        product.IsInStock = scrapedData.IsInStock;
        product.UpdatedAt = DateTime.UtcNow;

        if (product.Snapshots.Count == 0 || priceChanged || stockChanged)
        {
            var snapshot = new ProductSnapshot
            {
                Price = scrapedData.Price ?? 0m,
                UsualPrice = scrapedData.UsualPrice,
                IsOnSale = scrapedData.IsOnSale,
                IsInStock = scrapedData.IsInStock,
                CreatedAt = DateTime.UtcNow
            };
            product.Snapshots.Add(snapshot);
        }
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Snapshots.OrderByDescending(s => s.CreatedAt))
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagedResult<ProductSnapshot>> GetProductSnapshotsAsync(Guid productId, ProductSnapshotsRequest request)
    {
        var query = _context.ProductSnapshots
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .OrderByDescending(s => s.CreatedAt);

        return await query.ToPagedResultAsync(request);
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