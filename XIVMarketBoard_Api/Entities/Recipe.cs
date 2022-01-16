using System.ComponentModel.DataAnnotations;

namespace XIVMarketBoard_Api.Entities
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public int ItemId { get; set; }
        public virtual List<Item> Ingredients { get; set; }
        public string Name { get; set; }
        public virtual Job job { get; set; }


    }
}

