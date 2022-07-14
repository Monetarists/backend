namespace XIVMarketBoard_Api.Repositories.Models.Universalis;

public class UniversalisResponseItems
{
    public string itemId { get; set; } = "";
    public string worldId { get; set; } = "";
    public double lastUploadTime { get; set; }
    public IEnumerable<UniversalisListings> listings { get; set; } = new UniversalisListings[] { };
    public IEnumerable<UniversalisRecentHistory> recentHistory { get; set; } = new UniversalisRecentHistory[] { };
    public double currentAveragePrice { get; set; }
    public double currentAveragePriceNQ { get; set; }
    public double currentAveragePriceHQ { get; set; }
    public double regularSaleVelocity { get; set; }
    public double nqSaleVelocity { get; set; }
    public double hqSaleVelocity { get; set; }
    public double averagePrice { get; set; }
    public double averagePriceNQ { get; set; }
    public double averagePriceHQ { get; set; }
    public double minPrice { get; set; }
    public double minPriceNQ { get; set; }
    public double minPriceHQ { get; set; }
    public double maxPrice { get; set; }
    public double maxPriceNQ { get; set; }
    public double maxPriceHQ { get; set; }
}
