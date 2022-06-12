using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class Retainer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public virtual User User { get; set; } = new User();
        public virtual World World { get; set; } = new World();
        public string? Description { get; set; }

    }
}
