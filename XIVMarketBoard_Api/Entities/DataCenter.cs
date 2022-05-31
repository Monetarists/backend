using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class DataCenter
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }
}
