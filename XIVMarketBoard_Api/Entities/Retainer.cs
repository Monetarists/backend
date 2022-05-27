using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class Retainer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User User { get; set; }
        public virtual World World { get; set; }
        public string? Description { get; set; }

    }
}
