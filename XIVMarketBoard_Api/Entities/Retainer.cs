using System.ComponentModel.DataAnnotations.Schema;
namespace XIVMarketBoard_Api.Entities
{
    public class Retainer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RetainerId { get; set; }
        public string RetainerName { get; set; }
        public virtual User User { get; set; }
        public virtual World World { get; set; }
        public string? Description { get; set; }

    }
}
