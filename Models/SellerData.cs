namespace TCGCardScraper.Models;

internal class SellerData
{
    internal string Name { get; set; } = string.Empty;
    internal bool IsDirectSeller { get; set; }
    internal bool IsCertifiedHobbyShop { get; set; }
    internal bool IsGoldStarSeller { get; set; }
    internal string Rating { get; set; } = string.Empty;
    internal string Sales { get; set; } = string.Empty;
}
