using XIVMarketBoard_Api.Entities;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Http.Headers;

namespace XIVMarketBoard_Api
{
    public class XivApiImport
    {
        private const string baseAddress = "xivapi.com/";
        private const string recipeRequestBody = "{\"indexes\": \"recipe\",\"page\": \"1\",\"columns\":\"ID,Name,UrlType,ItemIngredient1.Name,ItemIngredient1.ID,ItemIngredient2.Name,ItemIngredient2.ID,ItemIngredient3.Name,ItemIngredient3.ID," +
            "ItemIngredient4.Name,ItemIngredient4.ID,ItemIngredient5.Name,ItemIngredient5.ID,ItemIngredient6.Name,ItemIngredient6.ID,ItemIngredient7.Name,ItemIngredient7.ID," +
            "ItemIngredient8.Name,ItemIngredient8.ID,ItemIngredient9.Name,ItemIngredient9.ID,ItemResult.Id,ItemResult.Name\"," +
            "\"body\": {\"query\": {\"bool\": {\"must\": [{\"wildcard\": {\"Name_en\": \"*\"}}]}},from\": 2,\"size\": 1}} ";
        private const string apikey = "e8e13efbf65441a0a5c51cb0c1415b35d3f603394b3a40dab6378a21c87d7767";
        private static readonly HttpClient client = new HttpClient();
        public List<Item> getAllItems()
        {

            return null;
        }
        public async void getAllRecipies()
        {
            var limiter = new TaskLimiter(15, TimeSpan.FromSeconds(1));
            await sendRequest(recipeRequestBody, "search");
            //return null;
        }
        private static async Task sendRequest(string body, string endpoint)
        {
            HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var result = await client.SendAsync(rM);

            Console.Write(result);
        }

        }
        

    }

}

/*
 * to use a tasklimiter
 public static void Main()
    {
        RunAsync().Wait();
    }

    public static async Task RunAsync()
    {
        var limiter = new TaskLimiter(10, TimeSpan.FromSeconds(1));

        // create 100 tasks 
        var tasks = Enumerable.Range(1, 100)
           .Select(e => limiter.LimitAsync(() => DoSomeActionAsync(e)));
        // wait unitl all 100 tasks are completed
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    static readonly Random _rng = new Random();

    public static async Task DoSomeActionAsync(int i)
    {
        await Task.Delay(150 + _rng.Next(150)).ConfigureAwait(false);
        Console.WriteLine("Completed Action {0}", i);
    }

 
 
 */
/*
 {\"indexes\": \"recipe\",\"page\": \"1\",\"columns\":\"ID,Name,UrlType,ItemIngredient1.Name,ItemIngredient1.ID,ItemIngredient2.Name,ItemIngredient2.ID,ItemIngredient3.Name,ItemIngredient3.ID,
ItemIngredient4.Name,ItemIngredient4.ID,ItemIngredient5.Name,ItemIngredient5.ID,ItemIngredient6.Name,ItemIngredient6.ID,ItemIngredient7.Name,ItemIngredient7.ID,
ItemIngredient8.Name,ItemIngredient8.ID,ItemIngredient9.Name,ItemIngredient9.ID,ItemResult.Id,ItemResult.Name\",
  \"body\": {\"query\": {\"bool\": {\"must\": [{\"wildcard\": {\"Name_en\": \"*\"}}]}},from\": 2,\"size\": 1}} 
 
 */