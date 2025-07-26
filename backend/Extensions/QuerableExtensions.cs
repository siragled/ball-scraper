using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wishlist.Models.Common;

namespace Wishlist.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Skip = request.Skip,
            Take = request.Take
        };
    }

    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !searchProperties.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var property in searchProperties)
        {
            var propertyAccess = Expression.Invoke(property, parameter);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var searchExpression = Expression.Call(
                propertyAccess,
                containsMethod,
                Expression.Constant(searchTerm, typeof(string)));

            combinedExpression = combinedExpression == null
                ? searchExpression
                : Expression.OrElse(combinedExpression, searchExpression);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sortBy,
        SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = typeof(T).GetProperty(sortBy);

        if (property == null)
            return query;

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var methodName = sortDirection == SortDirection.Asc ? "OrderBy" : "OrderByDescending";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.PropertyType },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}