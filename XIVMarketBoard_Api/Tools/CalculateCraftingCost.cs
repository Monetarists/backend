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
                if (DateTime.UtcNow.AddHours(-6) > query.LastUploadDate &&
                   DateTime.UtcNow.AddHours(-1) > query.QueryDate)
                {
                    queryList.Add(query.Item);
                    continue;
                }
                resultUniversalisList.Add(query);
            }
            if (queryList.Where(x => x.IsMarketable ?? false).Any())
            {
                resultUniversalisList.AddRange(
                    await _universalisController.ImportUniversalisDataForItemListAndWorld(queryList.Where(x => x.IsMarketable ?? false).ToList(), world));
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
                    .Sum(x => x.MinPrice * r.Ingredients.First(y => y.Item.Id == x.Item.Id).Amount);
                returnList.Add(MapCraftingCostResult(tempUniversalisEntries.ToList(), craftingCost, r));
            }

            return returnList;
        }
        public ResponseRecipe MapCraftingCostResult(List<UniversalisEntry> universalisEntries, double craftingCost, Recipe recipe)
        {

            var mostOutdatedEntry = universalisEntries.OrderByDescending(x => x.LastUploadDate).First();
            var returnResult = _mapper.Map(recipe, new ResponseRecipe());
            returnResult.Job = null;
            returnResult.Ingredients = null;
            var resultingItem = universalisEntries.FirstOrDefault(x => x.Item.Id == recipe.Item.Id);
            //if universalis returns no data return error mess
            if (resultingItem == null)
            {
                returnResult.UniversalisEntry = new ResponseUniversalisEntry();
                returnResult.UniversalisEntry.Message = "no data";
                return returnResult;
            }

            returnResult.UniversalisEntry = _mapper.Map(resultingItem, new ResponseUniversalisEntry());
            returnResult.UniversalisEntry.Posts = null;
            returnResult.UniversalisEntry.SaleHistory = null;
            returnResult.OldestUploadDate = mostOutdatedEntry.LastUploadDate;
            returnResult.OldestQueryDate = mostOutdatedEntry.QueryDate;


            if (universalisEntries.Any(x => x.Item.Id != recipe.Item.Id && x.MinPrice == 0))
            {
                returnResult.UniversalisEntry.Message = "Recipe contains one or more ingredients with minprice = 0";
                returnResult.UniversalisEntry.CraftingCost = 0;
                return returnResult;
            }
            returnResult.UniversalisEntry.CraftingCost = craftingCost;


            return returnResult;
        }

    }

}
