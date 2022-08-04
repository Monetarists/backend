namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseUniversalisHistory
    {
        public bool HighQuality { get; set; }
        public DateTime SaleDate { get; set; }
        public int Quantity { get; set; }
        public string? BuyerName { get; set; } = "";
        public double Total { get; set; }
    }
}
