public static class StringExtensions
{
    public static string? Truncate(this string? s, int max)
        => s is null || s.Length <= max ? s : s[..max];
}