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
        public async Task<string> resetAndImportRecipiesAndItems()
        {
            int start = 0;
            int amount = 500;
            int responseAmount = 1;
            int resultsNumber = 0;
            string resultString = "";
            var resultList = new List<Result>();
            string contentString;

            while (responseAmount > resultsNumber)
            {
                var httpResponse = await XivApiModel.getRecipesAsync(start, amount);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    contentString = await httpResponse.Content.ReadAsStringAsync();
                    var responseResults = JsonConvert.DeserializeObject<ResponeResults>(contentString);
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
                using (var xivContext = new XivDbContext())
                {

                    xivContext.Database.ExecuteSqlRaw(@"SET FOREIGN_KEY_CHECKS = 0; Truncate table Recipes;Truncate table Ingredients;Truncate table Items; SET FOREIGN_KEY_CHECKS = 1");
                    await SaveRecipiesToDb(xivContext, recipeList);
                }
            }
            catch (Exception e) { return "error" + e.Message; }
            return resultString;

        }

        private static async Task<List<Recipe>> CreateRecipes(List<Result> resultList)
        {
            var recipeList = new List<Recipe>();
            foreach (var r in resultList)
            {
                Recipe recipe = new Recipe();
                recipe.Ingredients = CreateIngredientList(r);
                recipe.job = new Job { Id = Int32.Parse(r.ClassJob.ID), Name = r.ClassJob.Name_en };
                recipe.Id = r.Id;
                recipe.Name = r.Name;
                recipe.Item = new Item { Id = r.ItemResult.Id, Name = r.ItemResult.Name };
                recipe.AmountResult = r.AmountResult;
                recipeList.Add(recipe);

            }

            return recipeList;
        }
        private static List<Ingredient> CreateIngredientList(Result r)
        {
            var ingredientList = new List<Ingredient>();

            if (r.AmountIngredient0 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient0, Item = new Item { Id = Int32.Parse(r.ItemIngredient0.ID), Name = r.ItemIngredient0.Name } });
            if (r.AmountIngredient1 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient1, Item = new Item { Id = Int32.Parse(r.ItemIngredient1.ID), Name = r.ItemIngredient1.Name } });
            if (r.AmountIngredient2 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient2, Item = new Item { Id = Int32.Parse(r.ItemIngredient2.ID), Name = r.ItemIngredient2.Name } });
            if (r.AmountIngredient3 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient3, Item = new Item { Id = Int32.Parse(r.ItemIngredient3.ID), Name = r.ItemIngredient3.Name } });
            if (r.AmountIngredient4 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient4, Item = new Item { Id = Int32.Parse(r.ItemIngredient4.ID), Name = r.ItemIngredient4.Name } });
            if (r.AmountIngredient5 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient5, Item = new Item { Id = Int32.Parse(r.ItemIngredient5.ID), Name = r.ItemIngredient5.Name } });
            if (r.AmountIngredient6 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient6, Item = new Item { Id = Int32.Parse(r.ItemIngredient6.ID), Name = r.ItemIngredient6.Name } });
            if (r.AmountIngredient7 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient7, Item = new Item { Id = Int32.Parse(r.ItemIngredient7.ID), Name = r.ItemIngredient7.Name } });
            if (r.AmountIngredient8 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient8, Item = new Item { Id = Int32.Parse(r.ItemIngredient8.ID), Name = r.ItemIngredient8.Name } });
            if (r.AmountIngredient9 > 0) ingredientList.Add(new Ingredient { Amount = r.AmountIngredient9, Item = new Item { Id = Int32.Parse(r.ItemIngredient9.ID), Name = r.ItemIngredient9.Name } });

            return ingredientList;
        }
        public async Task<string> SaveRecipiesToDb(XivDbContext xivContext, List<Recipe> RecipeList)
        {

            foreach (var recipe in RecipeList)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item, xivContext);
                }
                recipe.Item.CanBeCrafted = true;
                recipe.Item = await GetOrCreateItemFromContext(recipe.Item, xivContext);
                recipe.job = await GetOrCreateJobFromContext(recipe.job, xivContext);
                xivContext.Add(recipe);
            }

            await xivContext.SaveChangesAsync();
            return "successfully saved " + RecipeList.Count + "recipies";

        }
        private static async Task<Job> GetOrCreateJobFromContext(Job job, XivDbContext dbContext)
        {
            var tempJob = await dbContext.Jobs.FindAsync(job.Id);
            if (tempJob == null)
            {
                dbContext.Add(job);
                await dbContext.SaveChangesAsync();
                return job;
            }

            return tempJob;
        }
        private static async Task<Item> GetOrCreateItemFromContext(Item item, XivDbContext dbContext)
        {
            var tempItem = await dbContext.Items.FindAsync(item.Id);
            if (tempItem == null)
            {
                dbContext.Add(item);
                await dbContext.SaveChangesAsync();
                return item;
            }

            return tempItem;
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
        public class ResponeResults
        {
            public Pagination Pagination { get; set; }
            public Result[] Results { get; set; }
        }
        public class Pagination
        {

            public int Results;
            public int ResultsTotal;
        }

        public class Result
        {
            public int Id;
            public string Name;
            public ClassJob ClassJob;
            public ItemResult ItemResult;
            public int AmountResult;
            public int AmountIngredient0;
            public int AmountIngredient1;
            public int AmountIngredient2;
            public int AmountIngredient3;
            public int AmountIngredient4;
            public int AmountIngredient5;
            public int AmountIngredient6;
            public int AmountIngredient7;
            public int AmountIngredient8;
            public int AmountIngredient9;
            public ItemIngredient ItemIngredient0;
            public ItemIngredient ItemIngredient1;
            public ItemIngredient ItemIngredient2;
            public ItemIngredient ItemIngredient3;
            public ItemIngredient ItemIngredient4;
            public ItemIngredient ItemIngredient5;
            public ItemIngredient ItemIngredient6;
            public ItemIngredient ItemIngredient7;
            public ItemIngredient ItemIngredient8;
            public ItemIngredient ItemIngredient9;

        }
        public class ItemIngredient
        {
            public string ID;
            public string Name;
        }
        public class ClassJob
        {
            public string ID;
            public string Abbreviation_en;
            public string Name_en;
        }
        public class ItemResult
        {
            public int Id;
            public string Name;
        }

    }

}
