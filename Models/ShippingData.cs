namespace TCGCardScraper.Models;

internal class ShippingData
{
    internal string Price { get; set; } = string.Empty;
    internal bool Included { get; set; }
    internal bool FreeOverMinimum { get; set; }
    internal bool FreeDirect { get; set; }
}
