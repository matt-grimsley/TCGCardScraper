namespace TCGStoreScraper.Models
{
    public class ItemInfo
    {
        public string Condition { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

        public List<string> Data => [
            $"Condition: {Condition}",
            $"Price: {Price}"];
    }
}
