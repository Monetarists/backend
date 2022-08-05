namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseUniversalisEntry
    {
        public int Id { get; set; }
        public double? CraftingCost { get; set; }
        public string? Message { get; set; }
        public DateTime LastUploadDate { get; set; }
        public DateTime QueryDate { get; set; }
        public virtual ICollection<ResponseUniversalisPost>? Posts { get; set; }
        public virtual ICollection<ResponseUniversalisHistory>? SaleHistory { get; set; }

        public double CurrentAveragePrice { get; set; }
        public double CurrentAveragePrinceNQ { get; set; }
        public double CurrentAveragePriceHQ { get; set; }
        public double RegularSaleVelocity { get; set; }
        public double NqSaleVelocity { get; set; }
        public double HqSaleVelocity { get; set; }
        public double AveragePrice { get; set; }
        public double AveragePriceNQ { get; set; }
        public double AveragePriceHQ { get; set; }
        public double MinPrice { get; set; }
        public double MinPriceNQ { get; set; }
        public double MinPriceHQ { get; set; }
        public double MaxPrice { get; set; }
        public double MaxPriceNQ { get; set; }
        public double MaxPriceHQ { get; set; }
        public int? NqListingsCount { get; set; }
        public int? HqListingsCount { get; set; }
    }
}
