using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace XIVMarketBoard_Api
{
    public class XIVApiController
    {
        public static async Task<string> ImportAllWorldsAndDataCenters()
        {
            try
            {
                var response = await XivApiModel.getAllWorldsAsync();
                if (response.IsSuccessStatusCode)
                {
                
                    var responseResult = JsonConvert.DeserializeObject<XivApiModel.ResponeResults>(await response.Content.ReadAsStringAsync());
                    var worldList = new List<World>();
                    var dcList = new List<DataCenter>();

                    foreach (var server in responseResult.Results)
                    {
                        if(server.Name != "")
                        {
                            var responseWd = await XivApiModel.getWorldDetailsAsync(server.Id);
                            var resp = await responseWd.Content.ReadAsStringAsync();
                            var worldContent = JsonConvert.DeserializeObject<XivApiModel.WorldDetailResult>(await responseWd.Content.ReadAsStringAsync());
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
                    var dcResult = await DbController.SaveDataCenters(dcList);
                    var worldResult = await DbController.SaveWorlds(worldList);
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


            return "should not get here";


        }
        public static async Task<string> resetAndImportRecipiesAndItems()
        {
            int start = 0;
            int amount = 500;
            int responseAmount = 1;
            int resultsNumber = 0;
            string resultString = "";
            var resultList = new List<XivApiModel.Result>();
            string contentString;
            
            while (responseAmount > resultsNumber)
            {
                var httpResponse = await XivApiModel.getRecipesAsync(start, amount);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    contentString = await httpResponse.Content.ReadAsStringAsync();
                    var responseResults = JsonConvert.DeserializeObject<XivApiModel.ResponeResults>(contentString);
                    resultList.AddRange(responseResults.Results);
                    if (resultsNumber == 0)
                    {
                        resultsNumber = responseResults.Pagination.Results;
                    }
                    start += amount;
                    await Task.Delay(100);
                }
                else
                {
                    return "error" + httpResponse.StatusCode;
                }


            }
            try
            {
                var recipeList = await CreateRecipes(resultList);
                await DbController.GetOrCreateRecipies(recipeList);
                
            }
            catch (Exception e) { return "error" + e.Message; }
            return resultString;

        }
        
        private static async Task<List<Recipe>> CreateRecipes(List<XivApiModel.Result> resultList)
        {
            var recipeList = new List<Recipe>();
            foreach (var r in resultList)
            {
                Recipe recipe = new Recipe();
                recipe.Ingredients = CreateIngredientList(r);
                recipe.job = new Job { Id = r.ClassJob.Id, Name = r.ClassJob.Name_en };
                recipe.Id = r.Id;
                recipe.Name = r.Name;
                recipe.Item = new Item { Id = r.ItemResult.Id, Name = r.ItemResult.Name };
                recipe.AmountResult = r.AmountResult;
                recipeList.Add(recipe);

            }
            
            return recipeList;
        }
        private static List<Ingredient> CreateIngredientList(XivApiModel.Result r)
        {
            var ingredientList = new List<Ingredient>();

            if (r.AmountIngredient0 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient0, Item = new Item { Id = r.ItemIngredient0.Id, Name = r.ItemIngredient0.Name } });
            if (r.AmountIngredient1 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient1, Item = new Item { Id = r.ItemIngredient1.Id, Name = r.ItemIngredient1.Name } });
            if (r.AmountIngredient2 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient2, Item = new Item { Id = r.ItemIngredient2.Id, Name = r.ItemIngredient2.Name } });
            if (r.AmountIngredient3 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient3, Item = new Item { Id = r.ItemIngredient3.Id, Name = r.ItemIngredient3.Name } });
            if (r.AmountIngredient4 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient4, Item = new Item { Id = r.ItemIngredient4.Id, Name = r.ItemIngredient4.Name } });
            if (r.AmountIngredient5 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient5, Item = new Item { Id = r.ItemIngredient5.Id, Name = r.ItemIngredient5.Name } });
            if (r.AmountIngredient6 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient6, Item = new Item { Id = r.ItemIngredient6.Id, Name = r.ItemIngredient6.Name } });
            if (r.AmountIngredient7 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient7, Item = new Item { Id = r.ItemIngredient7.Id, Name = r.ItemIngredient7.Name } });
            if (r.AmountIngredient8 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient8, Item = new Item { Id = r.ItemIngredient8.Id, Name = r.ItemIngredient8.Name } });
            if (r.AmountIngredient9 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient9, Item = new Item { Id = r.ItemIngredient9.Id, Name = r.ItemIngredient9.Name } });

            return ingredientList;
        }

        /*public string getAllItems() {
            List<Result> resultList = new List<Result>();
            string contentString;
            int start = 0;
            int amount = 500;
            int responseAmount = 1;
            int resultsNumber = 0;
            int 
            
            while (responseAmount > resultsNumber)
            {
                var result = XivApiModel.GetItemsAsync(start, amount)
                contentString = result.Content.ReadAsStringAsync().Result;
                resultList.AddRange(responseResults.Results);
                resultsNumber = responseResults.Pagination.Results - 1;
                await Task.Delay(1000);
                start += amount;
            }
            var responseList = await Task.WhenAll(taskList).ConfigureAwait(false);

            foreach (var response in responseList)
            {
                resultList.AddRange(JsonConvert.DeserializeObject<ResponeResults>(response).Results);
                var a = resultList.Count();

            }
            return "";
        }*/

        public string getAllRecipies()
        {
            return "";
        }
       

    }

}
