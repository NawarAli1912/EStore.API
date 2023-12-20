namespace Presentation.Common.Models.Paging;

public class PagedList<T>
{
    private PagedList(
        List<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Items { get; set; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;

    public static PagedList<T> Create(List<T> items, int page, int pageSize, int totalCount)
    {
        return new(items, page, pageSize, totalCount);
    }
}
