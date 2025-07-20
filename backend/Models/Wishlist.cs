namespace Wishlist.Models;

public class Wishlist
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public virtual AppUser User { get; set; } = null!;
    public virtual ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}