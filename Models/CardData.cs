namespace TCGCardScraper.Models;

internal partial class CardData
{
    private string? _friendlyName;
    internal string? FriendlyName
    {
        get => _friendlyName;
        set => _friendlyName = RegexParser.ParseCardFriendlyName(value!);
    }
    internal string? FullName { get; set; }
    internal string? Set { get; set; }
    internal string? SetCode { get; set; }

}
