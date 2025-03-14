namespace TCGCardScraper.Tcgplayer.Models;
internal class TcgplayerCard
{
    internal TcgplayerCardData? Name { get; set; }
    internal TcgplayerSellerData? Seller { get; set; }
    internal TcgplayerListingData? Listing { get; set; }
    internal TcgplayerShippingData? Shipping { get; set; }
}
