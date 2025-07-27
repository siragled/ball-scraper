using AngleSharp;
using AngleSharp.Dom;
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
        var config = Configuration.Default.WithDefaultLoader();
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

            if (document == null)
            {
                _logger.LogWarning("AngleSharp could not parse the document from {Url}", url);
                return null;
            }

            var productData = new ScrapedProductData();
            ExtractSchemaOrgData(document, productData);
            return string.IsNullOrEmpty(productData.Name) ? null : productData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request for scraping {Url} failed with status {StatusCode}", url, ex.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while scraping {Url}", url);
            return null;
        }
    }

    private void ExtractSchemaOrgData(IHtmlDocument document, ScrapedProductData finalProductData)
    {
        var scripts = document.QuerySelectorAll("script[type='application/ld+json']");
        foreach (var script in scripts)
        {
            try
            {
                var json = script.TextContent;
                if (string.IsNullOrWhiteSpace(json)) continue;

                using var doc = JsonDocument.Parse(json);
                if (FindProductInJsonGraph(doc.RootElement, finalProductData))
                    break;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse JSON-LD script from page.");
            }
        }
    }

    private bool FindProductInJsonGraph(JsonElement element, ScrapedProductData finalProductData)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("@type", out var typeProperty))
            {
                var type = typeProperty.ValueKind == JsonValueKind.Array
                    ? typeProperty.EnumerateArray().FirstOrDefault(t => t.GetString() is "Product" or "ProductGroup").GetString()
                    : typeProperty.GetString();

                switch (type)
                {
                    case "Product":
                        var product = ParseProductElement(element);
                        if (product != null)
                        {
                            UpdateFinalProductData(finalProductData, product);
                            return true;
                        }
                        break;
                    case "ProductGroup":
                        var cheapestVariant = FindCheapestVariant(element);
                        if (cheapestVariant != null)
                        {
                            UpdateFinalProductData(finalProductData, cheapestVariant);
                            return true;
                        }
                        break;
                }
            }

            foreach (var property in element.EnumerateObject())
            {
                if (FindProductInJsonGraph(property.Value, finalProductData))
                    return true;
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (FindProductInJsonGraph(item, finalProductData))
                    return true;
            }
        }
        return false;
    }

    private ScrapedProductData? FindCheapestVariant(JsonElement productGroupElement)
    {
        if (!productGroupElement.TryGetProperty("hasVariant", out var variantsElement) || variantsElement.ValueKind != JsonValueKind.Array)
            return null;

        ScrapedProductData? cheapestVariant = null;
        foreach (var variantElement in variantsElement.EnumerateArray())
        {
            var currentVariant = ParseProductElement(variantElement);
            if (currentVariant?.Price == null) continue;
            if (cheapestVariant == null || currentVariant.Price < cheapestVariant.Price)
            {
                cheapestVariant = currentVariant;
            }
        }
        return cheapestVariant;
    }

    private ScrapedProductData? ParseProductElement(JsonElement productElement)
    {
        if (productElement.ValueKind != JsonValueKind.Object || !productElement.TryGetProperty("@type", out _))
            return null;

        var productData = new ScrapedProductData();

        if (productElement.TryGetProperty("name", out var name))
            productData.Name = WebUtility.HtmlDecode(name.GetString() ?? "");

        if (string.IsNullOrEmpty(productData.Name)) return null;

        if (productElement.TryGetProperty("description", out var desc))
            productData.Description = WebUtility.HtmlDecode(desc.GetString());

        if (productElement.TryGetProperty("brand", out var brand))
            productData.Brand = brand.ValueKind == JsonValueKind.Object && brand.TryGetProperty("name", out var brandName)
                ? WebUtility.HtmlDecode(brandName.GetString())
                : WebUtility.HtmlDecode(brand.GetString());

        if (productElement.TryGetProperty("image", out var image))
            productData.ImageUrl = image.ValueKind switch
            {
                JsonValueKind.Array => image.EnumerateArray().Select(e => e.GetString()).FirstOrDefault(s => !string.IsNullOrEmpty(s)),
                JsonValueKind.String => image.GetString(),
                JsonValueKind.Object when image.TryGetProperty("url", out var url) => url.GetString(),
                _ => null
            };

        if (productElement.TryGetProperty("sku", out var sku))
            productData.Sku = sku.GetString();

        if (productElement.TryGetProperty("offers", out var offers))
            ExtractOfferData(offers, productData);

        return productData;
    }

    private void ExtractOfferData(JsonElement offers, ScrapedProductData productData)
    {
        var offer = offers.ValueKind == JsonValueKind.Array
            ? offers.EnumerateArray().FirstOrDefault(o => o.TryGetProperty("@type", out var type) && type.GetString() == "Offer")
            : offers;

        if (offer.ValueKind != JsonValueKind.Object) return;

        if (offer.TryGetProperty("price", out var priceElement) && TryGetDecimal(priceElement, out var priceValue))
            productData.Price = priceValue;
        else if (offer.TryGetProperty("lowPrice", out var lowPriceElement) && TryGetDecimal(lowPriceElement, out var lowPriceValue))
            productData.Price = lowPriceValue;

        if (offer.TryGetProperty("priceCurrency", out var currency))
            productData.Currency = currency.GetString();

        if (offer.TryGetProperty("priceSpecification", out var priceSpec) && priceSpec.ValueKind == JsonValueKind.Object)
            if (priceSpec.TryGetProperty("price", out var salePriceEle) && TryGetDecimal(salePriceEle, out var salePrice))
                if (productData.Price > salePrice)
                {
                    productData.UsualPrice = productData.Price;
                    productData.Price = salePrice;
                }

        if (productData.Price < productData.UsualPrice)
            productData.IsOnSale = true;

        if (offer.TryGetProperty("availability", out var availability))
            productData.IsInStock = (availability.GetString() ?? string.Empty).EndsWith("InStock");
    }

    private bool TryGetDecimal(JsonElement element, out decimal value)
    {
        if (element.ValueKind == JsonValueKind.Number)
            return element.TryGetDecimal(out value);
        if (element.ValueKind == JsonValueKind.String)
            return decimal.TryParse(element.GetString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);

        value = 0;
        return false;
    }

    private void UpdateFinalProductData(ScrapedProductData finalData, ScrapedProductData sourceData)
    {
        finalData.Name = sourceData.Name;
        finalData.Description = sourceData.Description;
        finalData.ImageUrl = sourceData.ImageUrl;
        finalData.Price = sourceData.Price;
        finalData.Currency = sourceData.Currency;
        finalData.Brand = sourceData.Brand;
        finalData.Sku = sourceData.Sku;
        finalData.UsualPrice = sourceData.UsualPrice;
        finalData.IsOnSale = sourceData.IsOnSale;
        finalData.IsInStock = sourceData.IsInStock;
    }
}