namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseUniversalisPost
    {
        public string RetainerName { get; set; } = "";
        public double Price { get; set; }
        public int Amount { get; set; }
        public double TotalAmount { get; set; }
        public bool HighQuality { get; set; }
        public DateTime LastReviewDate { get; set; }
    }
}
