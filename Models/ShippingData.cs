namespace TCGCardScraper.Models;

internal class ShippingData
{
    internal string Price { get; set; } = string.Empty;
    internal bool Included { get; set; }
    internal bool FreeOverMinimum { get; set; }
    internal bool FreeDirect { get; set; }

    internal List<string> Data =>
        [
            $"Shipping Price: {Price}",
            $"Shipping Included: {Included}",
            $"Free over $5: {FreeOverMinimum}",
            $"Free Direct: {FreeDirect}",
        ];
}
