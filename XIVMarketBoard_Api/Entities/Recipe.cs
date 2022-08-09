using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Name_en { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
        public Item Item { get; set; } = new Item();
        public int AmountResult { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public virtual Job Job { get; set; } = new Job();
        public bool IsExpert { get; set; }
        public bool IsSpecializationRequired { get; set; }
    }
}

