namespace XIVMarketBoard_Api.Repositories.Models;

public class UniversalisRecentHistory
{
    public double pricePerUnit { get; set; }
    public int quantity { get; set; }
    public double timestamp { get; set; }
    public double total { get; set; }
    public string buyerName { get; set; } = "";
    public bool hq { get; set; }
}
