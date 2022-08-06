using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIVMarketBoard_Api.Entities
{
    public class SaleHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool HighQuality { get; set; }
        public DateTime SaleDate { get; set; }
        public int Quantity { get; set; }
        public string? BuyerName { get; set; } = "";
        public double Total { get; set; }
    }
}
