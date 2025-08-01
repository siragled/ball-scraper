namespace Wishlist.Models.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool HasNext => Skip + Take < TotalCount;
    public bool HasPrevious => Skip > 0;
}