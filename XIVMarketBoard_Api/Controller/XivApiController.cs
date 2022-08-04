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
            if (responseResult == null) { throw new ArgumentNullException(nameof(responseResult), "JsonConvert returned null object"); }
            foreach (var server in responseResult.Results)
            {
                if (server.Name_en != "")
                {
                    var responseWd = await _xivApiRepository.GetWorldDetailsAsync(server.Id);
                    var apiResponse = JsonConvert.DeserializeObject<XivApiWorldDetailResult>(await responseWd.Content.ReadAsStringAsync());
                    if (apiResponse == null)
                    {
                        throw new JsonException("JsonConvert returned null object");
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
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new ArgumentException("xivApi responded with " + httpResponse.StatusCode);
                }
                contentString = await httpResponse.Content.ReadAsStringAsync();

                var responseResults = JsonConvert.DeserializeObject<XivApiResponeResults>(contentString);

                if (responseResults == null)
                {
                    throw new JsonException("JsonConvert returned null object");
                }
                resultList.AddRange(responseResults.Results);
                if (resultsTotal == 0) { resultsTotal = responseResults.Pagination.ResultsTotal; }
                await Task.Delay(100);


            }

            var recipeList = CreateRecipes(resultList);
            await _recipeController.GetOrCreateRecipes(recipeList);


            return resultString;

        }



        private static IEnumerable<Recipe> CreateRecipes(IEnumerable<XivApiResult> resultList) =>
            resultList.Select(r => new Recipe
            {
                Ingredients = CreateIngredientList(r).ToList(),
                job = new Job
                {
                    Id = r.ClassJob.Id,
                    Name_en = r.ClassJob.Name_en,
                    Name_fr = r.ClassJob.Name_fr,
                    Name_de = r.ClassJob.Name_de,
                    Name_ja = r.ClassJob.Name_ja,
                    Abbreviation = r.ClassJob.Abbreviation,
                    DohDolJobIndex = r.ClassJob.DohDolJobIndex,
                    ClassJobCategoryTargetID = r.ClassJob.ClassJobCategoryTargetID

                },
                Id = r.Id,
                Name_en = r.Name_en,
                Name_de = r.Name_de,
                Name_fr = r.Name_fr,
                Name_ja = r.Name_ja,
                Item = new Item
                {
                    Id = r.ItemResult.ID ?? 0,
                    Name_en = r.ItemResult.Name_en ?? "",
                    Name_de = r.ItemResult.Name_de ?? "",
                    Name_fr = r.ItemResult.Name_fr ?? "",
                    Name_ja = r.ItemResult.Name_ja ?? "",

                    ItemSearchCategory = r.ItemResult.ItemSearchCategory != null ? createItemSearchCategory(r.ItemResult.ItemSearchCategory) : null,
                    ItemUICategory = createItemUiCategory(r.ItemResult.ItemUICategory) ?? new ItemUICategory(),
                    CanBeHq = r.ItemResult.CanBeHq ?? false,
                },
                AmountResult = r.AmountResult
            });


        private static IEnumerable<Ingredient> CreateIngredientList(XivApiResult r)
        {
            if (r.ItemIngredient0.ID != null) yield return new Ingredient { Amount = r.AmountIngredient0, Item = CreateItem(r.ItemIngredient0) };
            if (r.ItemIngredient1.ID != null) yield return new Ingredient { Amount = r.AmountIngredient1, Item = CreateItem(r.ItemIngredient1) };
            if (r.ItemIngredient2.ID != null) yield return new Ingredient { Amount = r.AmountIngredient2, Item = CreateItem(r.ItemIngredient2) };
            if (r.ItemIngredient3.ID != null) yield return new Ingredient { Amount = r.AmountIngredient3, Item = CreateItem(r.ItemIngredient3) };
            if (r.ItemIngredient4.ID != null) yield return new Ingredient { Amount = r.AmountIngredient4, Item = CreateItem(r.ItemIngredient4) };
            if (r.ItemIngredient5.ID != null) yield return new Ingredient { Amount = r.AmountIngredient5, Item = CreateItem(r.ItemIngredient5) };
            if (r.ItemIngredient6.ID != null) yield return new Ingredient { Amount = r.AmountIngredient6, Item = CreateItem(r.ItemIngredient6) };
            if (r.ItemIngredient9.ID != null) yield return new Ingredient { Amount = r.AmountIngredient9, Item = CreateItem(r.ItemIngredient9) };
            if (r.ItemIngredient7.ID != null) yield return new Ingredient { Amount = r.AmountIngredient7, Item = CreateItem(r.ItemIngredient7) };
            if (r.ItemIngredient8.ID != null) yield return new Ingredient { Amount = r.AmountIngredient8, Item = CreateItem(r.ItemIngredient8) };
        }
        private static Item CreateItem(XivApiItem xivItem)
        {

            var item = new Item
            {
                Id = xivItem.ID ?? 0,
                Name_en = xivItem.Name_en ?? "",
                Name_de = xivItem.Name_de ?? "",
                Name_fr = xivItem.Name_fr ?? "",
                Name_ja = xivItem.Name_ja ?? "",
                ItemSearchCategory = xivItem.ItemSearchCategory != null ? createItemSearchCategory(xivItem.ItemSearchCategory) : null,
                ItemUICategory = xivItem.ItemUICategory != null ? createItemUiCategory(xivItem.ItemUICategory) : new ItemUICategory(),
                CanBeHq = xivItem.CanBeHq ?? false,
            };

            return item;
        }
        private static ItemSearchCategory createItemSearchCategory(XivApiItemSearchCategory tempSc) => new ItemSearchCategory
        {
            Id = tempSc.ID ?? 0,
            Name_en = tempSc.Name_en ?? "",
            Name_de = tempSc.Name_de ?? "",
            Name_fr = tempSc.Name_fr ?? "",
            Name_ja = tempSc.Name_ja ?? ""
        };
        private static ItemUICategory createItemUiCategory(XivApiItemUiCategory tempUc) => new ItemUICategory
        {
            Id = tempUc.ID ?? 0,
            Name_en = tempUc.Name_en ?? "",
            Name_de = tempUc.Name_de ?? "",
            Name_fr = tempUc.Name_fr ?? "",
            Name_ja = tempUc.Name_ja ?? ""
        };
        public string GetAllRecipies()
        {
            return "";
        }



    }

}
