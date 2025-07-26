using AngleSharp;
using AngleSharp.Html.Dom;
using System.Net;
using System.Text.Json;
using Wishlist.Models;

namespace Wishlist.Services.Scrapers;

public class GenericProductScraper : IProductScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GenericProductScraper> _logger;
    private readonly IBrowsingContext _browsingContext;

    public int Priority => 1;

    public GenericProductScraper(HttpClient httpClient, ILogger<GenericProductScraper> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var config = Configuration.Default;
        _browsingContext = BrowsingContext.New(config);
    }

    public bool CanScrape(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == "http" || uri.Scheme == "https");

    public async Task<ScrapedProductData?> ScrapeAsync(string url)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"138\", \"Google Chrome\";v=\"138\"");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"macOS\"");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var document = await _browsingContext.OpenAsync(req => req.Content(html)) as IHtmlDocument;

            if (document == null) return null;

            var productData = new ScrapedProductData();

            ExtractSchemaOrgData(document, productData);
            if (string.IsNullOrEmpty(productData.Name))
                ExtractOpenGraphData(document, productData);

            if (!string.IsNullOrEmpty(productData.Name))
                _logger.LogInformation("Found product: {Name}", productData.Name);

            return string.IsNullOrEmpty(productData.Name) ? null : productData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scraping failed: {Message}", ex.Message);
            return null;
        }
    }

    private void ExtractSchemaOrgData(IHtmlDocument document, ScrapedProductData productData)
    {
        var scripts = document.QuerySelectorAll("script[type='application/ld+json']");

        foreach (var script in scripts)
        {
            try
            {
                var json = script.TextContent;
                if (string.IsNullOrWhiteSpace(json)) continue;

                using var doc = JsonDocument.Parse(json);
                if (TryExtractFromJsonLd(doc.RootElement, productData))
                    break;
            }
            catch { }
        }
    }

    private bool TryExtractFromJsonLd(JsonElement element, ScrapedProductData productData)
    {
        if (element.ValueKind == JsonValueKind.Array)
            return element.EnumerateArray().Any(item => TryExtractFromJsonLd(item, productData));

        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty("@type", out var type) ||
            type.GetString() != "Product")
            return false;

        if (element.TryGetProperty("name", out var name))
            productData.Name = name.GetString() ?? "";

        if (element.TryGetProperty("description", out var desc))
            productData.Description = desc.GetString();

        if (element.TryGetProperty("brand", out var brand))
        {
            productData.Brand = brand.ValueKind == JsonValueKind.Object &&
                               brand.TryGetProperty("name", out var brandName)
                ? brandName.GetString()
                : brand.GetString();
        }

        if (element.TryGetProperty("image", out var image))
        {
            productData.ImageUrl = image.ValueKind switch
            {
                JsonValueKind.Array => image.EnumerateArray().FirstOrDefault().GetString(),
                JsonValueKind.String => image.GetString(),
                JsonValueKind.Object when image.TryGetProperty("url", out var url) => url.GetString(),
                _ => null
            };
        }

        if (element.TryGetProperty("sku", out var sku))
            productData.Sku = sku.GetString();

        if (element.TryGetProperty("offers", out var offers))
            ExtractPriceFromOffers(offers, productData);

        return !string.IsNullOrEmpty(productData.Name);
    }

    private void ExtractPriceFromOffers(JsonElement offers, ScrapedProductData productData)
    {
        var offer = offers.ValueKind == JsonValueKind.Array
            ? offers.EnumerateArray().FirstOrDefault()
            : offers;

        if (offer.TryGetProperty("price", out var price) &&
            decimal.TryParse(price.GetString(), out var priceValue))
            productData.Price = priceValue;

        if (offer.TryGetProperty("priceCurrency", out var currency))
            productData.Currency = currency.GetString();

        if (offer.TryGetProperty("lowPrice", out var lowPrice) &&
            decimal.TryParse(lowPrice.GetString(), out var lowPriceValue))
            productData.Price = lowPriceValue;
    }

    private void ExtractOpenGraphData(IHtmlDocument document, ScrapedProductData productData)
    {
        var metas = document.QuerySelectorAll("meta[property], meta[name]");

        foreach (var meta in metas)
        {
            var property = (meta.GetAttribute("property") ?? meta.GetAttribute("name") ?? "").ToLower();
            var content = meta.GetAttribute("content") ?? "";

            if (string.IsNullOrEmpty(content)) continue;

            switch (property)
            {
                case "og:title" or "twitter:title" when string.IsNullOrEmpty(productData.Name):
                    productData.Name = content;
                    break;
                case "og:description" or "twitter:description" or "description" when string.IsNullOrEmpty(productData.Description):
                    productData.Description = content;
                    break;
                case "og:image" or "twitter:image" when string.IsNullOrEmpty(productData.ImageUrl):
                    productData.ImageUrl = content;
                    break;
                case "product:brand" when string.IsNullOrEmpty(productData.Brand):
                    productData.Brand = content;
                    break;
                case "product:price:amount" when !productData.Price.HasValue && decimal.TryParse(content, out var price):
                    productData.Price = price;
                    break;
                case "product:price:currency" when string.IsNullOrEmpty(productData.Currency):
                    productData.Currency = content;
                    break;
            }
        }
    }
}