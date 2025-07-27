namespace Wishlist.Models;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Brand { get; set; }
    public required string SourceUrl { get; set; }
    public string? StoreName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public decimal LastPrice { get; set; } = 0;
    public decimal? UsualPrice { get; set; }
    public bool IsOnSale { get; set; }
    public bool IsInStock { get; set; }

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public virtual ICollection<ProductSnapshot> Snapshots { get; set; } = new List<ProductSnapshot>();
}