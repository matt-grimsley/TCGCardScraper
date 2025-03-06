namespace TCGStoreScraper.Models
{
    public class ShippingInfo
    {
        public string Price { get; set; } = string.Empty;
        public bool Included { get; set; }
        public bool FreeOverMinimum { get; set; }
        public bool FreeDirect { get; set; }

        public List<string> Data => [
            $"Shipping Price: {Price}",
            $"Shipping Included: {Included}",
            $"Free over $5: {FreeOverMinimum}",
            $"Free Direct: {FreeDirect}"];
    }
}
