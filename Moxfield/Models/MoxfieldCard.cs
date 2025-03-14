namespace TCGCardScraper.Moxfield.Models;
public class MoxfieldCard
{
    public string Name { get; set; } = string.Empty;
    public string SetCode { get; set; } = string.Empty;
    public string CollectorNumber { get; set; } = string.Empty;
    public bool IsFoil { get; set; }
    public string TcgplayerId { get; set; } = "0";

    public Uri GetTcgplayerSearchUri()
    {
        var foilParam = IsFoil
            ? "&Printing=Foil"
            : string.Empty;

        return new Uri($"https://www.tcgplayer.com/product/{TcgplayerId}?Language=English&Condition=Near+Mint|Lightly+Played{foilParam}");
    }
}
