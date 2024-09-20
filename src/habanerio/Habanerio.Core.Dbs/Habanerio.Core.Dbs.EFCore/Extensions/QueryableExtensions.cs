using Habanerio.Core.DBs.EFCore.Expressions;

namespace Habanerio.Core.DBs.EFCore.Extensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> ApplyOrderBy<T>(this IQueryable<T> query, OrderByExpression<T>? orderByExpression = null) where T : class
    {
        var orderBys = orderByExpression?.GetOrderBys() ?? OrderByExpression<T>.Empty();
        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var (keySelector, isDescending) in orderBys)
        {
            if (orderedQuery == null)
            {
                orderedQuery = isDescending
                    ? query.OrderByDescending(keySelector)
                    : query.OrderBy(keySelector);
            }
            else
            {
                orderedQuery = isDescending
                    ? orderedQuery.ThenByDescending(keySelector)
                    : orderedQuery.ThenBy(keySelector);
            }
        }

        // Default order if no expressions
        return orderedQuery ?? query.OrderBy(x => 1);
    }
}