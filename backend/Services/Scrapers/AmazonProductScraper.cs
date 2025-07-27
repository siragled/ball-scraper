using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Globalization;
using System.Text.RegularExpressions;
using Wishlist.Models;

namespace Wishlist.Services.Scrapers;

public class AmazonProductScraper : IProductScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AmazonProductScraper> _logger;
    private readonly IBrowsingContext _browsingContext;

    public int Priority => 10;

    public AmazonProductScraper(HttpClient httpClient, ILogger<AmazonProductScraper> logger, IBrowsingContext browsingContext)
    {
        _httpClient = httpClient;
        _logger = logger;
        _browsingContext = browsingContext;
    }

    public bool CanScrape(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Host.Contains("amazon.com") ||
                uri.Host.Contains("amazon.co.uk") ||
                uri.Host.Contains("amazon.de") ||
                uri.Host.Contains("amazon.nl"));
    }

    public async Task<ScrapedProductData?> ScrapeAsync(string url)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var document = await _browsingContext.OpenAsync(req => req.Content(html)) as IHtmlDocument;

            if (document == null)
            {
                _logger.LogWarning("AngleSharp could not parse the Amazon document from {Url}", url);
                return null;
            }

            return ParseAmazonPage(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while scraping Amazon URL: {Url}", url);
            return null;
        }
    }

    private ScrapedProductData? ParseAmazonPage(IHtmlDocument document)
    {
        var productData = new ScrapedProductData();

        productData.Name = document.QuerySelector("#productTitle")?.TextContent.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(productData.Name))
        {
            _logger.LogWarning("Could not find Amazon product title selector '#productTitle'.");
            return null;
        }

        productData.Brand = document.QuerySelector("#bylineInfo")?.TextContent.Replace("Visit the", "").Replace("Store", "").Trim();

        productData.ImageUrl = document.QuerySelector("#landingImage")?.GetAttribute("src");

        var featureBullets = document.QuerySelectorAll("#feature-bullets .a-list-item")
            .Select(e => e.TextContent.Trim());
        productData.Description = string.Join("\n", featureBullets);

        var availabilityEl = document.QuerySelector("#availability")?.TextContent.Trim().ToLowerInvariant();
        productData.IsInStock = !string.IsNullOrEmpty(availabilityEl) && !availabilityEl.Contains("currently unavailable");

        var priceText = document.QuerySelector(".a-price .a-offscreen")?.TextContent;
        productData.Price = ParsePrice(priceText);

        var usualPriceText = document.QuerySelector(".basisPrice .a-text-price .a-offscreen")?.TextContent;
        productData.UsualPrice = ParsePrice(usualPriceText);

        if (productData.Price.HasValue && productData.UsualPrice.HasValue && productData.Price < productData.UsualPrice)
        {
            productData.IsOnSale = true;
        }
        else
        {
            productData.IsOnSale = false;
            productData.UsualPrice = null;
        }

        return productData;
    }

    private decimal? ParsePrice(string? priceText)
    {
        if (string.IsNullOrWhiteSpace(priceText))
            return null;

        var sanitizedPrice = Regex.Replace(priceText, @"[^\d,.]", "").Trim();
        sanitizedPrice = sanitizedPrice.Replace(',', '.');

        if (decimal.TryParse(sanitizedPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
        {
            return price;
        }

        return null;
    }
}