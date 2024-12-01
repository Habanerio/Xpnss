using System.Linq.Expressions;

namespace Habanerio.Core.DBs.EFCore.Expressions;

/// <summary>
/// Helper for creating OrderBy expressions.
/// </summary>
/// <typeparam name="TDbEntity"></typeparam>
public class OrderByExpression<TDbEntity> where TDbEntity : class
{
    private readonly List<(Expression<Func<TDbEntity, object>> KeySelector, bool IsDescending)> _orderBys;

    public OrderByExpression()
    {
        _orderBys = [];
    }

    public OrderByExpression<TDbEntity> AddOrderBy(Expression<Func<TDbEntity, object>> keySelector)
    {
        _orderBys.Add((keySelector, false));
        return this;
    }

    public OrderByExpression<TDbEntity> AddOrderByDescending(Expression<Func<TDbEntity, object>> keySelector)
    {
        _orderBys.Add((keySelector, true));
        return this;
    }

    public List<(Expression<Func<TDbEntity, object>> KeySelector, bool IsDescending)> GetOrderBys()
    {
        return _orderBys;
    }

    public static List<(Expression<Func<TDbEntity, object>> KeySelector, bool IsDescending)> Empty()
    {
        return [];
    }
}