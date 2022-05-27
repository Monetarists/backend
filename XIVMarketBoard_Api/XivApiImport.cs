using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
namespace XIVMarketBoard_Api
{
    public class XivApiImport
    {
        private const string baseAddress = "https://xivapi.com/";

        private const string stringstring = @"""body"": {""query"": {""bool"": {""must"": [{""wildcard"": {""Name_en"": ""*""}}]}}"; 
        private const string recipeColumns = "ID,ClassJob.Name_en,ClassJob.Abbreviation_en,ClassJob.ID,Name,UrlType,AmountResult," +
            "AmountIngredient0,AmountIngredient1,AmountIngredient2,AmountIngredient3,AmountIngredient4,AmountIngredient5," +
            "AmountIngredient6,AmountIngredient7,AmountIngredient8,AmountIngredient9,ItemIngredient0.Name,ItemIngredient0.ID,ItemIngredient1.Name,ItemIngredient1.ID," +
            "ItemIngredient2.Name,ItemIngredient2.ID,ItemIngredient3.Name,ItemIngredient3.ID,ItemIngredient4.Name,ItemIngredient4.ID," +
            "ItemIngredient5.Name,ItemIngredient5.ID,ItemIngredient6.Name,ItemIngredient6.ID,ItemIngredient7.Name,ItemIngredient7.ID," +
            "ItemIngredient8.Name,ItemIngredient8.ID,ItemIngredient9.Name,ItemIngredient9.ID,ItemResult.ID,ItemResult.Name";
        private const string apikey = "e8e13efbf65441a0a5c51cb0c1415b35d3f603394b3a40dab6378a21c87d7767";

        private static readonly HttpClient client = new HttpClient();
        public List<Item> getAllItems()
        {
            
            return null;
        }
        public static /*List<Recipe>*/ string getAllRecipes()
        {

            //var a = getAllRecipesAsync();
           // RequestString request = new RequestString("recipe", recipeColumns, queryJson);


            //var limiter = new TaskLimiter(15, TimeSpan.FromSeconds(1));
            //sendelasti(JsonConvert.SerializeObject(request), "Search");

            //var requestString = @" {""indexes"": ""recipe"",""page"": ""1"",""columns"": "+ recipeColumns + @",""body"": {"+ stringstring + @"},""from"": 2,""size"": 1}}";
            var anonObj =  new { indexes = "recipe", columns = recipeColumns, 
                body = new 
                {
                    from = 0,
                    size = 10,
                    query = new 
                    {
                        wildcard = new
                        {
                            Name_en = "*"
                        }
                    }
                }
            };
            var requestString = JsonConvert.SerializeObject(anonObj);
            var result = SendRequest(requestString, "Search");

            string contentString;
            if (result.IsSuccessStatusCode == true)
            {
                 contentString = result.Content.ReadAsStringAsync().Result;
                var responseResults = JsonConvert.DeserializeObject<ResponeResults>(contentString);
            }
            else
            {
                contentString = "error" + result.Content.ReadAsStringAsync().Result;
            }






            return contentString;
        }
        public static async Task<List<Result>> GetAllItemsAsync()
        {

            var result = SendRequest(BuildJsonRequestString(0, 100, "item", recipeColumns), "Search");
            List<Result> resultList = new List<Result>();
            string contentString;
            if (result.IsSuccessStatusCode == true)
            {
                contentString = result.Content.ReadAsStringAsync().Result;
                var responseResults = JsonConvert.DeserializeObject<ResponeResults>(contentString);
                int resultsNumber = responseResults.Pagination.Results - 1;
                resultList.AddRange(responseResults.Results);
                List<Task<String>> taskList = new List<Task<String>>();

                while (responseResults.Pagination.ResultsTotal > resultsNumber)
                {
                    taskList.Add(SendRequestAsync(
                        BuildJsonRequestString(resultsNumber, 500, "recipe", recipeColumns), "search"));
                    resultsNumber += 500;
                    await Task.Delay(1000);

                }
                // wait unitl all 100 tasks are completed
                var responseList = await Task.WhenAll(taskList).ConfigureAwait(false);

                foreach (var response in responseList)
                {
                    resultList.AddRange(JsonConvert.DeserializeObject<ResponeResults>(response).Results);
                    var a = resultList.Count();

                }

            }
            else
            {
                contentString = "error" + result.Content.ReadAsStringAsync().Result;
            }
            return resultList;

        }
        public static async Task<string> ImportAllRecipesAsync()
        {
            string resultString = "";
            var result = SendRequest(BuildJsonRequestString(0,100,"recipe",recipeColumns), "Search");
            var resultList = new List<Result>();
            string contentString;
            if (result.IsSuccessStatusCode == true)
            {
                contentString = result.Content.ReadAsStringAsync().Result;
                var responseResults = JsonConvert.DeserializeObject<ResponeResults>(contentString);
                int resultsNumber = responseResults.Pagination.Results;
                resultList.AddRange(responseResults.Results);


                while(responseResults.Pagination.ResultsTotal > resultsNumber)
                {
                    string responseString = await SendRequestAsync(
                    //BuildJsonRequestString(resultsNumber, 5, "recipe", recipeColumns), "search");
                    BuildJsonRequestString(resultsNumber, 500, "recipe", recipeColumns), "search");
                    resultList.AddRange(JsonConvert.DeserializeObject<ResponeResults>(responseString).Results);
                    //resultsNumber += 5;
                    resultsNumber += 500;
                    await Task.Delay(100);

                }
            

                var Recipes = await CreateRecipes(resultList);

                using (var dbContext = new XivDbContext())
                {

                    dbContext.Database.ExecuteSqlRaw(@"SET FOREIGN_KEY_CHECKS = 0; Truncate table Recipes;Truncate table Ingredients;Truncate table Items; SET FOREIGN_KEY_CHECKS = 1");

                    try
                    {
                        foreach (var recipe in Recipes)
                        {
                            foreach (var ingredient in recipe.Ingredients)
                            {
                                ingredient.Item = await GetOrCreateItem(ingredient.Item, dbContext);
                            }
                            recipe.Item = await GetOrCreateItem(recipe.Item, dbContext);
                            recipe.job = await GetOrCreateJob(recipe.job, dbContext);
                            dbContext.Add(recipe);
                        }

                        await dbContext.SaveChangesAsync();
                        resultString = "successfully saved " + Recipes.Count + "recipies";
                    }
                    catch (Exception ex)
                    {
                        resultString = ex.Message;
                    }

                }

                
            }
            else
            {
                resultString = "error" + result.Content.ReadAsStringAsync().Result;
            }
            return resultString;

        }
        private static async Task<List<Recipe>> CreateRecipes(List<Result> resultList)
        {
            var recipeList = new List<Recipe>();
            foreach(var r in resultList)
            { 
                Recipe recipe = new Recipe();
                recipe.Ingredients = CreateIngredientList(r);
                recipe.job = new Job { Id = Int32.Parse(r.ClassJob.ID), Name = r.ClassJob.Name_en };
                recipe.Id = r.Id;
                recipe.Name = r.Name;
                recipe.Item = new Item {Id = r.ItemResult.Id, Name = r.ItemResult.Name };
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
        private static async Task<Item> GetOrCreateItem(Item item, XivDbContext dbContext)
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
        private static async Task<Job> GetOrCreateJob(Job job, XivDbContext dbContext)
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

        private static string BuildJsonRequestString(int from, int size, string index, string columns)
        {
            var anonObj = new
            {
                indexes = index,
                columns = columns,
                body = new
                {
                    from = from,
                    size = size,
                    query = new
                    {
                        wildcard = new
                        {
                            Name_en = "*"
                        }
                    }
                }
            };
            return JsonConvert.SerializeObject(anonObj);
        }
        private static async Task<String> SendRequestAsync(string body, string endpoint)
        {

            HttpRequestMessage rM = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = await client.SendAsync(rM);
            var resultString = await result.Content.ReadAsStringAsync();
            Console.Write(result);
            return resultString;
        }
        private static HttpResponseMessage SendRequest(string body, string endpoint)
        {
            HttpRequestMessage rM = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = client.Send(rM);
            Console.Write(result);
            return result;
            
        }


    }
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
