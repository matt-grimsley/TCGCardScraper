namespace TCGCardScraper.Models;

internal class Card
{
    internal CardData? Name { get; set; }
    internal SellerData? Seller { get; set; }
    internal ListingData? Listing { get; set; }
    internal ShippingData? Shipping { get; set; }
}
