using System.ComponentModel.DataAnnotations;
namespace XIVMarketBoard_Api.Entities
{
    public class World
    {
        [Key]
        public string WorldName { get; set; }
        
        public string DataCenter { get; set; }

    }
}
