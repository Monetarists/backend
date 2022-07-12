using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using XIVMarketBoard_Api.Repositories;
using System.Reactive.Linq;
using XIVMarketBoard_Api.Repositories.Models.XivApi;

namespace XIVMarketBoard_Api.Controller
{
    public interface IXivApiController
    {
        string GetAllRecipies();
        Task<string> ImportWorldsDataCentres();
        Task<string> ImportRecipiesAndItems();
    }

    public class XivApiController : IXivApiController
    {
        private readonly IXivApiRepository _xivApiRepository;
        private readonly IDataCentreController _dataCentreController;
        private readonly IRecipeController _recipeController;
        public XivApiController(IXivApiRepository xivApiRepository, IDataCentreController dataCentreController, IRecipeController recipeController)
        {
            _xivApiRepository = xivApiRepository;
            _dataCentreController = dataCentreController;
            _recipeController = recipeController;
        }

        public async Task<string> ImportWorldsDataCentres()
        {
            var worldList = new List<World>();
            var dataCenterList = new List<DataCenter>();
            var response = await _xivApiRepository.GetAllWorldsAsync();
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode + " " + await response.Content.ReadAsStringAsync();
            }

            var responseResult = JsonConvert.DeserializeObject<XivApiResponeResults>(await response.Content.ReadAsStringAsync());
            if (responseResult == null) { throw new ArgumentNullException("JsonConvert returned null object"); }
            foreach (var server in responseResult.Results)
            {
                if (server.Name != "")
                {
                    var responseWd = await _xivApiRepository.GetWorldDetailsAsync(server.Id);
                    var resp = await responseWd.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<XivApiWorldDetailResult>(await responseWd.Content.ReadAsStringAsync());
                    if (apiResponse == null)
                    {
                        throw new ArgumentNullException("JsonConvert returned null object");
                    }

                    if (apiResponse.InGame && apiResponse.Name_en != "")
                    {
                        var dcEntity = dataCenterList.FirstOrDefault(p => p.Id == apiResponse.DataCenter.Id);
                        if (dcEntity == null)
                        {
                            dcEntity = new DataCenter();
                            dcEntity.Name = apiResponse.DataCenter.Name_en;
                            dcEntity.Id = apiResponse.DataCenter.Id;
                            dcEntity.Region = apiResponse.DataCenter.Region;
                            dataCenterList.Add(dcEntity);
                        }

                        var world = new World();
                        world.Id = apiResponse.Id;
                        world.Name = apiResponse.Name;
                        world.DataCenter = dcEntity;
                        worldList.Add(world);
                    }
                    await Task.Delay(100);
                }

            }
            var distintcDataCenters = dataCenterList.Distinct().ToList();
            await _dataCentreController.SaveDataCenters(distintcDataCenters);
            await _dataCentreController.SaveWorlds(worldList);
            return "import of worlds successful";
        }

        public async Task<string> ImportRecipiesAndItems()
        {
            int amount = 500;
            int resultsTotal = 0;
            string resultString = "";
            var resultList = new List<XivApiResult>();
            string contentString;

            for (int i = 0; resultsTotal >= i; i += amount)
            {
                var httpResponse = await _xivApiRepository.GetRecipesAsync(i, amount);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    contentString = await httpResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var responseResults = JsonConvert.DeserializeObject<XivApiResponeResults>(contentString);

                        if (responseResults != null)
                        {
                            resultList.AddRange(responseResults.Results);
                            if (resultsTotal == 0)
                            {
                                resultsTotal = responseResults.Pagination.ResultsTotal;
                            }
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception e)
                    {
                        return "error: " + e.Message;
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
                await _recipeController.GetOrCreateRecipes(recipeList);


            }
            catch (Exception e) { return "error" + e.Message; }
            return resultString;

        }



        private static IEnumerable<Recipe> CreateRecipes(IEnumerable<XivApiResult> resultList) =>
            resultList.Select(r => new Recipe
            {
                Ingredients = CreateIngredientList(r).ToList(),
                job = new Job { Id = r.ClassJob.Id, Name = r.ClassJob.Name_en },
                Id = r.Id,
                Name = r.Name,
                Item = new Item { Id = r.ItemResult.Id.Value, Name = r.ItemResult.Name },
                AmountResult = r.AmountResult
            });

        private static IEnumerable<Ingredient> CreateIngredientList(XivApiResult r)
        {
            if (r.ItemIngredient0.Id != null && r.ItemIngredient0.Name != null) yield return new Ingredient { Amount = r.AmountIngredient0, Item = new Item { Id = r.ItemIngredient0.Id.Value, Name = r.ItemIngredient0.Name } };
            if (r.ItemIngredient1.Id != null && r.ItemIngredient1.Name != null) yield return new Ingredient { Amount = r.AmountIngredient1, Item = new Item { Id = r.ItemIngredient1.Id.Value, Name = r.ItemIngredient1.Name } };
            if (r.ItemIngredient2.Id != null && r.ItemIngredient2.Name != null) yield return new Ingredient { Amount = r.AmountIngredient2, Item = new Item { Id = r.ItemIngredient2.Id.Value, Name = r.ItemIngredient2.Name } };
            if (r.ItemIngredient3.Id != null && r.ItemIngredient3.Name != null) yield return new Ingredient { Amount = r.AmountIngredient3, Item = new Item { Id = r.ItemIngredient3.Id.Value, Name = r.ItemIngredient3.Name } };
            if (r.ItemIngredient4.Id != null && r.ItemIngredient4.Name != null) yield return new Ingredient { Amount = r.AmountIngredient4, Item = new Item { Id = r.ItemIngredient4.Id.Value, Name = r.ItemIngredient4.Name } };
            if (r.ItemIngredient5.Id != null && r.ItemIngredient5.Name != null) yield return new Ingredient { Amount = r.AmountIngredient5, Item = new Item { Id = r.ItemIngredient5.Id.Value, Name = r.ItemIngredient5.Name } };
            if (r.ItemIngredient6.Id != null && r.ItemIngredient6.Name != null) yield return new Ingredient { Amount = r.AmountIngredient6, Item = new Item { Id = r.ItemIngredient6.Id.Value, Name = r.ItemIngredient6.Name } };
            if (r.ItemIngredient7.Id != null && r.ItemIngredient7.Name != null) yield return new Ingredient { Amount = r.AmountIngredient7, Item = new Item { Id = r.ItemIngredient7.Id.Value, Name = r.ItemIngredient7.Name } };
            if (r.ItemIngredient8.Id != null && r.ItemIngredient8.Name != null) yield return new Ingredient { Amount = r.AmountIngredient8, Item = new Item { Id = r.ItemIngredient8.Id.Value, Name = r.ItemIngredient8.Name } };
            if (r.ItemIngredient9.Id != null && r.ItemIngredient9.Name != null) yield return new Ingredient { Amount = r.AmountIngredient9, Item = new Item { Id = r.ItemIngredient9.Id.Value, Name = r.ItemIngredient9.Name } };
        }

        public string GetAllRecipies()
        {
            return "";
        }



    }

}
