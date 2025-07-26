using Wishlist.Models;

namespace Wishlist.Services.Scrapers;

public interface IProductScraper
{
    /// <summary>
    /// Determines if this scraper can handle the given URL
    /// </summary>
    bool CanScrape(string url);

    /// <summary>
    /// Scrapes product data from the URL
    /// </summary>
    Task<ScrapedProductData?> ScrapeAsync(string url);

    /// <summary>
    /// Priority for scraper selection (higher = preferred)
    /// </summary>
    int Priority { get; }
}