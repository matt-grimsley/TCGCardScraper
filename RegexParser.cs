namespace TCGCardScraper;

using System.Text.RegularExpressions;
using TCGCardScraper.Models;

internal sealed partial class RegexParser
{
    #region RegEx
    [GeneratedRegex(@"\(([^)]+)\)")]
    private static partial Regex CardNameSetCodeRegex();

    [GeneratedRegex(@"[<>:""/\|?*]")]
    private static partial Regex CardFriendlyNameRegex();

    [GeneratedRegex(@"(\d{1,3},?\d{0,3})\+?\sListings")]
    private static partial Regex ListingsCountRegex();

    #endregion

    #region Maps
    private static readonly Dictionary<string, string> ConditionMap = new() {
        { "Near Mint", "NM" },
        { "Lightly Played", "LP" },
        { "Moderately Played", "MP" },
        { "Heavily Played", "HP" },
        { "Damaged", "D" }
    };

    private static readonly Dictionary<string, string> LanguageMap = new() {
        { "Spanish", "SP" },
        { "French", "FR" },
        { "German", "DE" },
        { "Italian", "IT" },
        { "Portugese", "PT" },
        { "Japanese", "JP" },
        { "Korean", "KR" },
        { "Russian", "RU" },
        { "Chinese (S)", "CS" },
        { "Chinese (T)", "CT" }
    };

    private static readonly Dictionary<string, string> FoilMap = new() { { "Foil", "*F*" } };
    #endregion

    internal static CardData ParseCardName(string fullname)
    {
        var parts = fullname.Split(" - ", 2, StringSplitOptions.TrimEntries);

        var name = parts[0];
        var set = parts.Length > 1 ? parts[1] : string.Empty;
        var setCode = CardNameSetCodeRegex().Match(set).Groups[1].Value;

        return new CardData()
        {
            FullName = fullname,
            FriendlyName = name,
            Set = set,
            SetCode = setCode,
        };
    }

    internal static string ParseCardFriendlyName(string fullname) => CardFriendlyNameRegex().Replace(fullname, string.Empty);

    internal static int ParseListingsCount(string text) => int.TryParse(ListingsCountRegex().Match(text).Groups[1].Value.Replace(",", ""), out var value) ? value : 0;

    internal static string ParseCondition(string condition)
    {
        var result = condition;

        foreach (var kvp in ConditionMap)
        {
            result = Regex.Replace(result, $@"\b{Regex.Escape(kvp.Key)}\b", kvp.Value);
        }

        foreach (var kvp in LanguageMap)
        {
            result = Regex.Replace(result, $@"\b{Regex.Escape(kvp.Key)}\b", kvp.Value);
        }

        foreach (var kvp in FoilMap)
        {
            result = Regex.Replace(result, $@"\b{Regex.Escape(kvp.Key)}\b", kvp.Value);
        }

        return result;
    }

}
