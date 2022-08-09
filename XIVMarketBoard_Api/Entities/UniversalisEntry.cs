using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class UniversalisEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Item Item { get; set; } = new Item();
        public World World { get; set; } = new World();
        public DateTime LastUploadDate { get; set; }
        public DateTime QueryDate { get; set; }
        public virtual ICollection<MbPost> Posts { get; set; } = new List<MbPost>();
        public virtual ICollection<SaleHistory> SaleHistory { get; set; } = new List<SaleHistory>();
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
        public int? NqSaleCount { get; set; }
        public int? HqSaleCount { get; set; }
    }
}
