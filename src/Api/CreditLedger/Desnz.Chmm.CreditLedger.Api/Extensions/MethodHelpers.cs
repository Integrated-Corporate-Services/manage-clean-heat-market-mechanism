using System.Collections.Concurrent;

/// <summary>
/// Static class for extension methods
/// </summary>
public static class MethodHelpers 
{
    /// <summary>
    /// Mimics adding multiple entries to a ConcurrentBag
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="toAdd"></param>
    public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
    {
        foreach (var element in toAdd)
        {
            @this.Add(element);
        }
    }
}