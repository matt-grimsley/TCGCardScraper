namespace TCGCardScraper;

using System.Diagnostics;
using System.Text;
using TCGCardScraper.Models;

internal readonly struct StoreCardPairing(string store, Card card)
{
    internal string StoreName { get; } = store;
    internal Card Card { get; } = card;
}

internal readonly struct StoreCards(string store, IEnumerable<Card> cards)
{
    internal string StoreName { get; } = store;
    internal IEnumerable<Card> Cards { get; } = cards;
    internal decimal TotalPrice => Cards.Sum(card => card.Listing?.Price ?? 0m);
    internal int DistinctCards => Cards.Select(card => card.Name?.FriendlyName).Distinct().Count();
}

internal static class StoreMatchAnalyzer
{
    private static readonly Configuration Config = Configuration.Instance;
    private static readonly string ResultsFilePath = Path.Combine(Config.ResultsFilePath, Config.ResultsFileName);

    internal static async Task Analyze(IEnumerable<Card> scrapeResults)
    {
        Logger.Log(Logger.LogLevel.INFO, "StoreMatchAnalyzer analyzing...");

        var filteredResults = ApplyFilters(scrapeResults);
        var pairedResults = PairResults(filteredResults);
        var groupedResults = GroupResults(pairedResults);

        await WriteResultsFileAsync(groupedResults, scrapeResults);
    }

    internal static void OpenResultsFile() => Process.Start("notepad.exe", ResultsFilePath);

    private static IEnumerable<Card> ApplyFilters(IEnumerable<Card> scrapeResults) =>
        scrapeResults
            .Where(card => !Config.ExcludeDirectSellers || card.Seller?.IsDirectSeller == false)
            .Where(card => !Config.ForceFreeShipping || card.Shipping?.FreeOverMinimum == true);

    private static IEnumerable<StoreCardPairing> PairResults(IEnumerable<Card> filteredResults) =>
        filteredResults
            .Select(card => new StoreCardPairing(card.Seller!.Name, card))
            .Tap(pair => Logger.Log(Logger.LogLevel.DEBUG, Formatting.GenerateCardSummary(pair.Card)));

    private static IEnumerable<StoreCards> GroupResults(IEnumerable<StoreCardPairing> pairs) =>
        pairs
            .GroupBy(pair => pair.StoreName)
            .Select(group => new StoreCards(group.Key, group.Select(pair => pair.Card)))
            .Where(storeCards => storeCards.DistinctCards > 1)
            .OrderByDescending(storeCards => storeCards.DistinctCards)
            .ThenBy(storeCards => storeCards.TotalPrice);

    private static async Task WriteResultsFileAsync(IEnumerable<StoreCards> results, IEnumerable<Card> originalScrapeResults)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(Formatting.GenerateFileSeparator())
                     .AppendLine(Formatting.GenerateFileSeparator("Store Match Analyzer Results"))
                     .AppendLine(Formatting.GenerateFileSeparator())
                     .AppendLine();

        var resultsGroupedByCards = results.Select(store => store.DistinctCards).Distinct().ToList();
        if (resultsGroupedByCards.Count == 0)
        {
            var summary = Formatting.GenerateNoResultsSummary(originalScrapeResults);

            await WriteToFile(summary);

            Logger.Log(Logger.LogLevel.WARNING, $"StoreMatchAnalyzer stopped. {summary}");

            return;
        }

        foreach (var numberOfCards in resultsGroupedByCards)
        {
            stringBuilder.AppendLine(Formatting.GenerateFileSeparator($"{numberOfCards} Listings"))
                         .AppendLine();

            var matchingStores = results.Where(store => store.DistinctCards == numberOfCards);

            foreach (var store in matchingStores)
            {
                stringBuilder.AppendLine(store.StoreName)
                             .AppendLine(Formatting.GenerateFileSeparator(store.StoreName.Length));

                foreach (var card in store.Cards)
                {
                    stringBuilder.AppendLine(Formatting.GenerateCardSummary(card));
                }

                var totalPrice = Formatting.GenerateTotalPrice(store.TotalPrice);

                stringBuilder.AppendLine(Formatting.GenerateFileSeparator(totalPrice.Length))
                             .AppendLine(totalPrice)
                             .AppendLine();
            }
        }

        await File.WriteAllTextAsync(ResultsFilePath, stringBuilder.ToString());

        Logger.Log(Logger.LogLevel.INFO, "StoreMatchAnalyzer complete!");
    }

    private static async Task WriteToFile(string content) => await File.WriteAllTextAsync(ResultsFilePath, content);

}
