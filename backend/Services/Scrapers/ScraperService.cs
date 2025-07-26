using Wishlist.Models;

namespace Wishlist.Services.Scrapers;

public class ScraperService
{
    private readonly IEnumerable<IProductScraper> _scrapers;
    private readonly ILogger<ScraperService> _logger;

    public ScraperService(IEnumerable<IProductScraper> scrapers, ILogger<ScraperService> logger)
    {
        _scrapers = scrapers.OrderByDescending(s => s.Priority).ToList();
        _logger = logger;
    }

    public async Task<ScrapedProductData?> ScrapeProductAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var scraper = _scrapers.FirstOrDefault(s => s.CanScrape(url));
        if (scraper == null)
        {
            _logger.LogWarning("No scraper found for URL: {Url}", url);
            return null;
        }

        try
        {
            _logger.LogInformation("Scraping {Url} with {ScraperType}", url, scraper.GetType().Name);
            return await scraper.ScrapeAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping {Url} with {ScraperType}", url, scraper.GetType().Name);
            return null;
        }
    }
}