using System.Text;

namespace TCGStoreScraper.Models
{
    public class ListingItem
    {
        public string? Name { get; set; }
        public SellerInfo? Seller { get; set; }
        public ItemInfo? Info { get; set; }
        public ShippingInfo? Shipping { get; set; }
        public string Report()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine();

            if (Seller is not null)
                foreach (var rec in Seller.Data) 
                    stringBuilder.AppendLine(rec);

            if (Info is not null)
                foreach (var rec in Info.Data) 
                    stringBuilder.AppendLine(rec);

            if (Shipping is not null)
                foreach (var rec in Shipping.Data) 
                    stringBuilder.AppendLine(rec);

            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    } 

    


}
