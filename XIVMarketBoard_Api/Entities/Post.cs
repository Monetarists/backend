using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIVMarketBoard_Api.Entities
{
    public class Post
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual User? User { get; set; }
        public virtual Retainer? Retainer { get; set; }
        public string RetainerName { get; set; } = "";
        public string SellerId { get; set; } = "";

        public double Price { get; set; }
        public int Amount { get; set; }
        public double TotalAmount { get; set; }
        public bool HighQuality { get; set; }
        public DateTime LastReviewDate { get; set; }

    }
}
