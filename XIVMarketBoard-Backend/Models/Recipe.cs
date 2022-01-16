namespace XIVMarketBoard_Backend.Models
{
    public class Recipe
    {
        public virtual List<Item> Ingredients { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; }
        public int ItemId { get; set; }

        public virtual Job job { get; set; }


    }
}

