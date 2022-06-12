using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public Item Item { get; set; } = new Item();
        public int AmountResult { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public string Name { get; set; } = "";
        public virtual Job job { get; set; } = new Job();
    }
}

