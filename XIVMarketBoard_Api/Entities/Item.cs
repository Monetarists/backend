using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Name_en { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
        public ItemSearchCategory? ItemSearchCategory { get; set; }
        public ItemUICategory ItemUICategory { get; set; } = new ItemUICategory();
        public bool? CanBeCrafted { get; set; }
        public bool CanBeHq { get; set; }
        public bool? IsMarketable { get; set; }
    }
}
