using System.ComponentModel.DataAnnotations;
namespace XIVMarketBoard_Api.Entities
{
    public class World
    {
        [Key]
        public string Name { get; set; }
        
        public string DataCenter { get; set; }

    }
}
