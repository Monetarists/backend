using AutoMapper;
using XIVMarketBoard_Api.Controller;
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models;
using XIVMarketBoard_Api.Repositories.Models.ResponseDto;

namespace XIVMarketBoard_Api.Tools
{
    public interface ICalculateCraftingCost
    {
        Task<List<ResponseCraftingCostDto>> GetCraftingCostRecipes(World world, List<Recipe> recipeList, List<Item> itemList);
        ResponseCraftingCostDto MapCraftingCostResult(List<UniversalisEntry> universalisEntries, double craftingCost, Recipe recipe);
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
        public async Task<List<ResponseCraftingCostDto>> GetCraftingCostRecipes(World world, List<Recipe> recipeList, List<Item> itemList)
        {
            //var universalisList = await _marketBoardController.GetLatestUniversalisQueryForItems(itemList, world);
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
            if (queryList.Count() > 0)
            {
                resultUniversalisList.AddRange(
                    await _universalisController.ImportUniversalisDataForItemListAndWorldQueueSave(queryList.Where(x => x.IsMarketable ?? false).ToList(), world, 5, 5));
            }
            var returnList = new List<ResponseCraftingCostDto>();
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
        public ResponseCraftingCostDto MapCraftingCostResult(List<UniversalisEntry> universalisEntries, double craftingCost, Recipe recipe)
        {
            var returnResult = new ResponseCraftingCostDto();
            //returnResult.UniversalisEntry = universalisEntries.Where(x => x.Item.Id == recipe.Item.Id).First();
            _mapper.Map(universalisEntries.Where(x => x.Item.Id == recipe.Item.Id).First(), returnResult.UniversalisEntry);
            returnResult.UniversalisEntry.Item = null;
            returnResult.UniversalisEntry.Posts = null;
            returnResult.UniversalisEntry.SaleHistory = null;

            returnResult.RecipeId = recipe.Id;
            if (universalisEntries
                .Where(x => x.Item.Id != recipe.Item.Id &&
                x.MinPrice == 0)
                .Count() > 0)
            {
                returnResult.message = "Recipe contains one or more ingredients with minprice = 0";
                returnResult.CraftingCost = 0;
                return returnResult;
            }
            returnResult.CraftingCost = universalisEntries
                .Where(x => recipe.Ingredients
                    .Select(i => i.Item.Id)
                    .Contains(x.Item.Id))
                .Sum(x => x.MinPrice);


            return returnResult;
        }

    }

}
