using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using XIVMarketBoard_Api.Repositories;
using XIVMarketBoard_Api.Repositories.Models;

namespace XIVMarketBoard_Api.Controller
{
    public interface IXivApiController
    {
        string GetAllRecipies();
        Task<string> ImportAllWorldsAndDataCenters();
        Task<string> ResetAndImportRecipiesAndItems();
    }

    public class XivApiController : IXivApiController
    {
        private readonly IXivApiRepository _xivApiRepository;
        private readonly IDbController _dbController;
        public XivApiController(IXivApiRepository xivApiRepository, IDbController dbController)
        {
            _xivApiRepository = xivApiRepository;
            _dbController = dbController;
        }

        public async Task<string> ImportAllWorldsAndDataCenters()
        {
            try
            {
                var response = await _xivApiRepository.GetAllWorldsAsync();
                if (response.IsSuccessStatusCode)
                {

                    var responseResult = JsonConvert.DeserializeObject<XivApiResponeResults>(await response.Content.ReadAsStringAsync());
                    if(responseResult == null)
                    {
                        throw new Exception("JsonConvert returned null object");
                    }
                    var worldList = new List<World>();
                    var dcList = new List<DataCenter>();

                    foreach (var server in responseResult.Results)
                    {
                        if (server.Name != "")
                        {
                            var responseWd = await _xivApiRepository.GetWorldDetailsAsync(server.Id);
                            var resp = await responseWd.Content.ReadAsStringAsync();
                            var worldContent = JsonConvert.DeserializeObject<XivApiWorldDetailResult>(await responseWd.Content.ReadAsStringAsync());
                            if (worldContent == null)
                            {
                                throw new Exception("JsonConvert returned null object");
                            }
                            if (worldContent.InGame == true && worldContent.Name_en != "")
                            {
                                var dcEntity = dcList.FirstOrDefault(p => p.Id == worldContent.DataCenter.Id);
                                if (dcEntity == null)
                                {
                                    dcEntity = new DataCenter();
                                    dcEntity.Name = worldContent.DataCenter.Name_en;
                                    dcEntity.Id = worldContent.DataCenter.Id;
                                    dcEntity.Region = worldContent.DataCenter.Region;
                                    dcList.Add(dcEntity);
                                }

                                var world = new World();
                                world.Id = worldContent.Id;
                                world.Name = worldContent.Name;
                                world.DataCenter = dcEntity;
                                worldList.Add(world);
                            }
                            await Task.Delay(100);
                        }

                    }
                    var dcListDistinct = dcList.Distinct().ToList();
                    var dcResult = await _dbController.SaveDataCenters(dcList);
                    var worldResult = await _dbController.SaveWorlds(worldList);
                    return "import of worlds successful";
                }
                else
                {
                    return "error";
                }

            }
            catch (Exception e)
            {
                return "error: " + e.Message;
            }





        }
        public async Task<string> ResetAndImportRecipiesAndItems()
        {
            int start = 0;
            int amount = 500;
            int responseAmount = 1;
            int resultsNumber = 0;
            string resultString = "";
            var resultList = new List<XivApiResult>();
            string contentString;

            while (responseAmount > resultsNumber)
            {
                var httpResponse = await _xivApiRepository.GetRecipesAsync(start, amount);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    contentString = await httpResponse.Content.ReadAsStringAsync();
                    var responseResults = JsonConvert.DeserializeObject<XivApiResponeResults>(contentString);
                    if(responseResults != null)
                    {
                        resultList.AddRange(responseResults.Results);
                        if (resultsNumber == 0)
                        {
                            resultsNumber = responseResults.Pagination.Results;
                        }
                        start += amount;
                        await Task.Delay(100);
                    }
                }

                else
                {
                    return "error" + httpResponse.StatusCode;
                }


            }
            try
            {
                var recipeList = CreateRecipes(resultList);
                await _dbController.GetOrCreateRecipies(recipeList);

            }
            catch (Exception e) { return "error" + e.Message; }
            return resultString;

        }



        private IEnumerable<Recipe> CreateRecipes(IEnumerable<XivApiResult> resultList) =>
            resultList.Select(r => new Recipe
            {
                Ingredients = CreateIngredientList(r).ToList(),
                job = new Job { Id = r.ClassJob.Id, Name = r.ClassJob.Name_en },
                Id = r.Id,
                Name = r.Name,
                Item = new Item { Id = r.ItemResult.Id, Name = r.ItemResult.Name },
                AmountResult = r.AmountResult
            });

        private static IEnumerable<Ingredient> CreateIngredientList(XivApiResult r)
        {
            if (r.AmountIngredient0 > 0) yield return (new Ingredient { Amount = r.AmountIngredient0, Item = new Item { Id = r.ItemIngredient0.Id, Name = r.ItemIngredient0.Name } });
            if (r.AmountIngredient1 > 0) yield return (new Ingredient { Amount = r.AmountIngredient1, Item = new Item { Id = r.ItemIngredient1.Id, Name = r.ItemIngredient1.Name } });
            if (r.AmountIngredient2 > 0) yield return (new Ingredient { Amount = r.AmountIngredient2, Item = new Item { Id = r.ItemIngredient2.Id, Name = r.ItemIngredient2.Name } });
            if (r.AmountIngredient3 > 0) yield return (new Ingredient { Amount = r.AmountIngredient3, Item = new Item { Id = r.ItemIngredient3.Id, Name = r.ItemIngredient3.Name } });
            if (r.AmountIngredient4 > 0) yield return (new Ingredient { Amount = r.AmountIngredient4, Item = new Item { Id = r.ItemIngredient4.Id, Name = r.ItemIngredient4.Name } });
            if (r.AmountIngredient5 > 0) yield return (new Ingredient { Amount = r.AmountIngredient5, Item = new Item { Id = r.ItemIngredient5.Id, Name = r.ItemIngredient5.Name } });
            if (r.AmountIngredient6 > 0) yield return (new Ingredient { Amount = r.AmountIngredient6, Item = new Item { Id = r.ItemIngredient6.Id, Name = r.ItemIngredient6.Name } });
            if (r.AmountIngredient7 > 0) yield return (new Ingredient { Amount = r.AmountIngredient7, Item = new Item { Id = r.ItemIngredient7.Id, Name = r.ItemIngredient7.Name } });
            if (r.AmountIngredient8 > 0) yield return (new Ingredient { Amount = r.AmountIngredient8, Item = new Item { Id = r.ItemIngredient8.Id, Name = r.ItemIngredient8.Name } });
            if (r.AmountIngredient9 > 0) yield return (new Ingredient { Amount = r.AmountIngredient9, Item = new Item { Id = r.ItemIngredient9.Id, Name = r.ItemIngredient9.Name } });
        }

        public string GetAllRecipies()
        {
            return "";
        }



    }

}
