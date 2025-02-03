using Amazon.Auth.AccessControlPolicy;

namespace Desnz.Chmm.Common.Extensions;

public static class ListExtensions
{
    public static DataPage<T> ToPage<T>(this List<T> data, int pageSize, int pageNumber)
    {
        var pageCount = (int)Math.Ceiling(data.Count() / (double)pageSize);

        var filtered = data.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        return new DataPage<T>(filtered, pageNumber, pageSize, pageCount > pageNumber);
    }
}

/// <summary>
/// Allows for paging of data
/// </summary>
public class DataPage<T>
{
    public DataPage(List<T> result, int pageNumber, int pageSize, bool hasNextPage)
    {
        Result = result;
        PageNumber = pageNumber;
        PageSize = pageSize;
        HasNextPage = hasNextPage;
    }

    /// <summary>
    /// The data for the request
    /// </summary>
    public List<T> Result { get; set; }

    /// <summary>
    /// Identifies if there's a next page
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Returns the current page numbser
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// How big is a page?
    /// </summary>
    public int PageSize { get; set; }
}
