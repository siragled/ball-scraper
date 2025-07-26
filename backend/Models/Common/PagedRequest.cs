using System.ComponentModel.DataAnnotations;

namespace Wishlist.Models.Common;

public record PagedRequest
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; } = 0;
    [Range(1, 100)]
    public int Take { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}

public enum SortDirection
{
    Asc,
    Desc
}