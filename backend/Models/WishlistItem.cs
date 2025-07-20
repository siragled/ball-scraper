namespace Wishlist.Models;

public class WishlistItem
{
    public Guid Id { get; set; }
    public decimal? GlobalTargetPrice { get; set; }
    public bool IsBought { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid WishlistId { get; set; }
    public Guid ProductId { get; set; }

    public virtual Wishlist Wishlist { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}