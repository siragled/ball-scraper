using Microsoft.AspNetCore.Identity;

namespace Wishlist.Models;

public class AppUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
