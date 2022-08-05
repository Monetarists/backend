using AutoMapper;
using XIVMarketBoard_Api.Controller;
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models;
using XIVMarketBoard_Api.Repositories.Models.ResponseDto;

namespace XIVMarketBoard_Api.Tools
{
    public interface ICalculateCraftingCost
    {
        Task<List<ResponseRecipe>> GetCraftingCostRecipes(World world, List<Recipe> recipeList, List<Item> itemList);
        ResponseRecipe MapCraftingCostResult(List<UniversalisEntry> universalisEntries, double craftingCost, Recipe recipe);
    }
    public class CalculateCraftingCost : ICalculateCraftingCost
    {
        private readonly IMarketBoardController _marketBoardController;
        private readonly IUniversalisApiController _universalisController;
        private readonly IMapper _mapper;
        public CalculateCraftingCost(IMarketBoardController marketBoardController, IUniversalisApiController universalisController, IMapper mapper)
        {
            _marketBoardController = marketBoardController;
            _universalisController = universalisController;
            _mapper = mapper;
        }
        public async Task<List<ResponseRecipe>> GetCraftingCostRecipes(World world, List<Recipe> recipeList, List<Item> itemList)
        {
            var universalisList = await _marketBoardController.GetLatestUniversalisQueryForItems(itemList, world);
            var queryList = itemList.Where(x => !universalisList.Select(x => x.Item.Id).Contains(x.Id)).ToList();
            var resultUniversalisList = new List<UniversalisEntry>();
            foreach (var query in universalisList)
            {
                if (DateTime.Now.AddHours(-6) > query.LastUploadDate &&
                   DateTime.Now.AddHours(-1) > query.QueryDate)
                {
                    queryList.Add(query.Item);
                    continue;
                }
                resultUniversalisList.Add(query);
            }
            if (queryList.Any())
            {
                resultUniversalisList.AddRange(
                    await _universalisController.ImportUniversalisDataForItemListAndWorld(queryList.Where(x => x.IsMarketable ?? false).ToList(), world, 5, 5));
            }
            var returnList = new List<ResponseRecipe>();
            foreach (Recipe r in recipeList)
            {
                var recipeItemIds = r.Ingredients.Select(x => x.Item.Id);
                var tempUniversalisEntries = resultUniversalisList.Where(x =>
                    recipeItemIds.Contains(x.Item.Id) ||
                    r.Item.Id == x.Item.Id).ToList();
                var craftingCost = tempUniversalisEntries
                    .Where(x => x.Item.Id != r.Item.Id)
                    .Sum(x => x.MinPrice);
                returnList.Add(MapCraftingCostResult(tempUniversalisEntries.ToList(), craftingCost, r));
            }

            return returnList;
        }
        public ResponseRecipe MapCraftingCostResult(List<UniversalisEntry> universalisEntries, double craftingCost, Recipe recipe)
        {


            var returnResult = _mapper.Map(recipe, new ResponseRecipe());
            returnResult.UniversalisEntry = _mapper.Map(universalisEntries.First(x => x.Item.Id == recipe.Item.Id), new ResponseUniversalisEntry());
            returnResult.Job = null;
            returnResult.Ingredients = null;
            returnResult.UniversalisEntry.Posts = null;
            returnResult.UniversalisEntry.SaleHistory = null;

            if (universalisEntries.Any(x => x.Item.Id != recipe.Item.Id && x.MinPrice == 0))
            {
                returnResult.UniversalisEntry.Message = "Recipe contains one or more ingredients with minprice = 0";
                returnResult.UniversalisEntry.CraftingCost = 0;
                return returnResult;
            }
            returnResult.UniversalisEntry.CraftingCost = universalisEntries
                .Where(x => recipe.Ingredients
                    .Select(i => i.Item.Id)
                    .Contains(x.Item.Id))
                .Sum(x => x.MinPrice);


            return returnResult;
        }

    }

}
