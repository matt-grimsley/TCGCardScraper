namespace TCGCardScraper.Models;

internal class ListingData
{
    internal string Condition { get; set; } = string.Empty;

    internal string ShortCondition => RegexParser.ParseCondition(Condition);

    internal decimal Price { get; set; } = 0.0m;
}
