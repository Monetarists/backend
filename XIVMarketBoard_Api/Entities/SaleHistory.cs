using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIVMarketBoard_Api.Entities
{
    public class SaleHistory
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleHistoryId { get; set; }
        public Item Item { get; set; }
        public bool HighQuality { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
