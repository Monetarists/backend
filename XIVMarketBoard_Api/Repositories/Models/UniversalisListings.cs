namespace XIVMarketBoard_Api.Repositories.Models
{

    public class UniversalisListings
    {
        public double lastReviewTime { get; set; }
        public double pricePerUnit { get; set; }
        public int quantity { get; set; }
        public bool hq { get; set; }
        public double total { get; set; }
        public string retainerName { get; set; } = "";
        public string retainerID { get; set; } = "";
        public string sellerID { get; set; } = "";
        public string listingID { get; set; } = "";
    }





}