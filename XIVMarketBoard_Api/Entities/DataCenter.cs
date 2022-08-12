using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class DataCenter
    {

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Region { get; set; } = "";
        public ICollection<World> Worlds { get; set; } = new List<World>();
    }
}
