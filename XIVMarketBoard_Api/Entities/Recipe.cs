using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public Item Item { get; set; }
        public int AmountResult { get; set; }
        public virtual List<Ingredient> Ingredients { get; set; }
        public string Name { get; set; }
        public virtual Job job { get; set; }
    }
}

