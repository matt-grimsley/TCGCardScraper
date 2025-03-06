namespace TCGStoreScraper.Models
{
    public class SellerInfo
    {
        public string Name { get; set; } = string.Empty;
        public bool IsDirectSeller { get; set; }
        public bool IsCertifiedHobbyShop { get; set; }
        public bool IsGoldStarSeller { get; set; }
        public string Rating { get; set; } = string.Empty;
        public string Sales { get; set; } = string.Empty;

        public List<string> Data => [
            $"* {Name}",
            $"TCG Direct: {IsDirectSeller}",
            $"Certified Hobby Shop: {IsCertifiedHobbyShop}",
            $"Gold Star Seller: {IsGoldStarSeller}",
            $"Rating: {Rating}",
            $"Sales: {Sales}" ];
    }
}
