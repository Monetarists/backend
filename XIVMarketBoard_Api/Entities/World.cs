using System.ComponentModel.DataAnnotations;
namespace XIVMarketBoard_Api.Entities
{
    public class World
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string DataCenter { get; set; }

    }
}
