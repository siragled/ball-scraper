namespace Wishlist.Models;

public class ScrapedProductData
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public string? Sku { get; set; }
    public string? Ean { get; set; }
    public string? Upc { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}