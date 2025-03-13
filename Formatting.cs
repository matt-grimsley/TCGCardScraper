namespace TCGCardScraper;

using Microsoft.Playwright;
using TCGCardScraper.Models;

internal static class Formatting
{
    private static readonly Configuration Config = Configuration.Instance;
    internal static string GenerateCardSummary(Card card)
    {
        var cardInfo = $"{card.Name?.FriendlyName} ({card.Name?.SetCode}) - {card.Listing?.ShortCondition}";
        var cardPrice = $"{card.Listing?.Price:F2}".PadLeft(6);

        return $"${cardPrice} - {cardInfo}";
    }

    internal static string GenerateTotalPrice(decimal price)
    {
        var totalPrice = $"{price:F2}".PadLeft(6);

        return $"Total: ${totalPrice}";
    }

    internal static string GenerateFileSeparator() => new('-', Config.ResultsFileCharacterWidth);

    internal static string GenerateFileSeparator(int width) => new('-', width);

    internal static string GenerateFileSeparator(string caption)
    {
        var padding = Config.ResultsFileCharacterWidth - caption.Length - 4;
        var leftPadding = padding / 2;
        var rightPadding = padding - leftPadding;

        return $"{new string('-', leftPadding)}  {caption}  {new string('-', rightPadding)}";
    }

    internal static string GenerateNoResultsSummary(IEnumerable<Card> cards)
    {
        var distinctCardCount = cards
            .Select(card => card.Name?.FriendlyName)
            .Distinct()
            .Count();

        if (distinctCardCount < 2)
        {
            return $"StoreMatchAnalyzer produced no results.  Need at least two cards to compare, found {distinctCardCount}.";
        }

        var cardSummary = string.Join(
            Environment.NewLine,
            cards.Select(card => $"{card.Name?.FriendlyName} ({card.Name?.SetCode}) - {card.Listing?.ShortCondition}").Distinct()
        );

        return $"StoreMatchAnalyzer produced no results.  No sellers had listings for at least two of the {distinctCardCount} requested cards:{Environment.NewLine}{cardSummary}";
    }


    internal static async Task<decimal> FormatListingItemPriceData(IElementHandle element)
    {
        if (element is not null)
        {
            var innerText = await element.InnerTextAsync();

            if (decimal.TryParse(innerText.Replace('$', ' '), out var result))
            {
                return result;
            }
        }

        return 0.0m;
    }
}
