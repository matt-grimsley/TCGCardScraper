namespace TCGCardScraper;
internal static class LinqExtensions
{
    internal static IEnumerable<T> Tap<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var t in source)
        {
            action(t);
            yield return t;
        }
    }
}
