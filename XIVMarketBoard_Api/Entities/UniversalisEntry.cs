﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class UniversalisEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Item Item { get; set; }
        public World World { get; set; }
        public DateTime LastUploadDate { get; set; }
        public DateTime QueryDate { get; set; }
        public List<MbPost> Posts { get; set; }
        public List<SaleHistory> SaleHistory { get; set; }
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
    }
}
