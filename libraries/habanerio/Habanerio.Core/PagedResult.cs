using System.Collections.ObjectModel;

namespace Habanerio.Core;

public class PagedResults<TCollection>
{
    /// <summary>
    /// The items for the current pageNo of results.
    /// </summary>
    public ReadOnlyCollection<TCollection> Items { get; set; }

    /// <summary>
    /// The current pageNo number
    /// </summary>
    public int PageNo { get; set; }

    /// <summary>
    /// The number of items per pageNo.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages for the query.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// The total number of items in the query.
    /// </summary>
    public int TotalCount { get; set; }

    public PagedResults(IEnumerable<TCollection>? items, int pageNo, int pageSize, int totalCount)
    {
        Items = new ReadOnlyCollection<TCollection>(items?.ToArray() ?? Array.Empty<TCollection>());
        PageNo = pageNo;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PagedResults<TCollection> Empty()
    {
        return new PagedResults<TCollection>(Enumerable.Empty<TCollection>(), 0, 0, 0);
    }
}