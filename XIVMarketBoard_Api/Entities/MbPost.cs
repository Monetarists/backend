using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
        
namespace XIVMarketBoard_Api.Entities
{
    public class MbPost
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual User? User { get; set; }
        public virtual Retainer? Retainer { get; set; }
        public virtual Item Item { get; set; }
        public string RetainerName { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public int TotalAmount { get; set; }
        public bool HighQuality { get; set; }
        public DateTime PostedDate { get; set; }       
        public World World { get; set; }
        public DateTime QueryDate { get; set; }
    }
}
