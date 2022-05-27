using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class UniversalisQuery
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Item item { get; set; }
        public World world { get; set; }
        public DateTime lastUploadDate { get; set; }
        public DateTime QueryDate { get; set; }
        public List<MbPost> posts { get; set; }
        public List<SaleHistory> postHistory { get; set; }
        public int currentAveragePrice { get; set; }
        public int currentAveragePrinceNQ { get; set; }
        public int currentAveragePriceHQ { get; set; }
        public int regularSaleVelocity { get; set; }
        public int nqSaleVelocity { get; set; }
        public int hqSaleVelocity { get; set; }
        public int averagePrice { get; set; }
        public int averagePriceNQ { get; set; }
        public int averagePriceHQ { get; set; }
        public int minPrice { get; set; }
        public int minPriceNQ { get; set; }
        public int minPriceHQ { get; set; }
        public int maxPrice { get; set; }
        public int maxPriceNQ { get; set; }
        public int maxPriceHQ { get; set; }
    }
}
