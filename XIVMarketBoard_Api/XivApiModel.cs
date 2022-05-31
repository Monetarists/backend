using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
namespace XIVMarketBoard_Api
{
    public class XivApiModel
    {
        private const string baseAddress = "https://xivapi.com/";

        private const string stringstring = @"""body"": {""query"": {""bool"": {""must"": [{""wildcard"": {""Name_en"": ""*""}}]}}";
        private const string recipeColumns = "ID,ClassJob.Name_en,ClassJob.Abbreviation_en,ClassJob.ID,Name,UrlType,AmountResult," +
            "AmountIngredient0,AmountIngredient1,AmountIngredient2,AmountIngredient3,AmountIngredient4,AmountIngredient5," +
            "AmountIngredient6,AmountIngredient7,AmountIngredient8,AmountIngredient9,ItemIngredient0.Name,ItemIngredient0.ID,ItemIngredient1.Name,ItemIngredient1.ID," +
            "ItemIngredient2.Name,ItemIngredient2.ID,ItemIngredient3.Name,ItemIngredient3.ID,ItemIngredient4.Name,ItemIngredient4.ID," +
            "ItemIngredient5.Name,ItemIngredient5.ID,ItemIngredient6.Name,ItemIngredient6.ID,ItemIngredient7.Name,ItemIngredient7.ID," +
            "ItemIngredient8.Name,ItemIngredient8.ID,ItemIngredient9.Name,ItemIngredient9.ID,ItemResult.ID,ItemResult.Name";
        private const string apikey = Credentials.XIVApiKey;

        private static readonly HttpClient client = new HttpClient();


        public static async Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems)
        {

            var response = await SendRequestAsync(BuildJsonRequestString(startNumber, amountOfItems, "items", recipeColumns), "search");

            return response;

        }
        public static async Task<HttpResponseMessage> getRecipesAsync(int start, int amount)
        {

            var response = await SendRequestAsync(
                 BuildJsonRequestString(start, amount, "recipe", recipeColumns), "search");
            return response;

        }

        public static async Task<HttpResponseMessage> getAllWorldsAsync()
        {
            return await SendRequestAsync("", "world?limit=3000");
        }
        public static async Task<HttpResponseMessage> getWorldDetailsAsync(int Id)
        {
            return await SendRequestAsync("", "world/" + Id);
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
        private static async Task<HttpResponseMessage> SendRequestAsync(string body, string endpoint)
        {

            HttpRequestMessage rM = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = await client.SendAsync(rM);
            Console.Write(result);
            return result;
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
            public Item ItemResult;
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
            public Item ItemIngredient0;
            public Item ItemIngredient1;
            public Item ItemIngredient2;
            public Item ItemIngredient3;
            public Item ItemIngredient4;
            public Item ItemIngredient5;
            public Item ItemIngredient6;
            public Item ItemIngredient7;
            public Item ItemIngredient8;
            public Item ItemIngredient9;

        }
        public class Item
        {
            public int Id;
            public string Name;
        }
        public class ClassJob
        {
            public int Id;
            public string Abbreviation_en;
            public string Name_en;
        }

        public class WorldDetailResult
        {
            public int Id;
            public string Name;
            public string Name_en;
            public bool InGame;
            public DataCenterResult DataCenter;
        }
        public class DataCenterResult
        {
            public int Id;
            public string Name;
            public string Name_en;
            public string Region;
        }

    }
}
