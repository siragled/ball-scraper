namespace Wishlist.Models;

public class ProductSnapshot
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public decimal Price { get; set; }
    public decimal? UsualPrice { get; set; }
    public bool IsOnSale { get; set; }
    public bool IsInStock { get; set; }

    public virtual Product Product { get; set; } = null!;
}