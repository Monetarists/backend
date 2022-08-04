using XIVMarketBoard_Api.Entities;
namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class CraftingCostResult
    {
        public string message { get; set; }
        public double CraftingCost { get; set; }
        public double RecipeId { get; set; }
        public UniversalisEntry UniversalisEntry { get; set; }
    }
}
