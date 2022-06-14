using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace XIVMarketBoard_Api.Repositories
{
    public interface IXivApiRepository
    {
        Task<HttpResponseMessage> GetAllWorldsAsync();
        Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems);
        Task<HttpResponseMessage> GetRecipesAsync(int start, int amount);
        Task<HttpResponseMessage> GetWorldDetailsAsync(int Id);
    }

    public class XivApiRepository : IXivApiRepository
    {
        private const string baseAddress = "https://xivapi.com/";

        private const string stringstring = @"""body"": {""query"": {""bool"": {""must"": [{""wildcard"": {""Name_en"": ""*""}}]}}";
        private const string recipeColumns = "ID,ClassJob.Name_en,ClassJob.Abbreviation_en,ClassJob.ID,Name,UrlType,AmountResult," +
            "AmountIngredient0,AmountIngredient1,AmountIngredient2,AmountIngredient3,AmountIngredient4,AmountIngredient5," +
            "AmountIngredient6,AmountIngredient7,AmountIngredient8,AmountIngredient9,ItemIngredient0.Name,ItemIngredient0.ID,ItemIngredient1.Name,ItemIngredient1.ID," +
            "ItemIngredient2.Name,ItemIngredient2.ID,ItemIngredient3.Name,ItemIngredient3.ID,ItemIngredient4.Name,ItemIngredient4.ID," +
            "ItemIngredient5.Name,ItemIngredient5.ID,ItemIngredient6.Name,ItemIngredient6.ID,ItemIngredient7.Name,ItemIngredient7.ID," +
            "ItemIngredient8.Name,ItemIngredient8.ID,ItemIngredient9.Name,ItemIngredient9.ID,ItemResult.ID,ItemResult.Name,ItemResult.AlwaysCollectable";
        private const string apikey = Credentials.XIVApiKey;

        private static readonly HttpClient client = new HttpClient();


        public async Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems)
        {

            var response = await SendRequestAsync(BuildJsonRequestString(startNumber, amountOfItems, "items", recipeColumns), "search");

            return response;

        }
        public async Task<HttpResponseMessage> GetRecipesAsync(int start, int amount)
        {

            var response = await SendRequestAsync(
                 BuildJsonRequestString(start, amount, "recipe", recipeColumns), "search");
            return response;

        }

        public async Task<HttpResponseMessage> GetAllWorldsAsync()
        {
            return await SendRequestAsync("", "world?limit=3000");
        }
        public async Task<HttpResponseMessage> GetWorldDetailsAsync(int Id)
        {
            return await SendRequestAsync("", "world/" + Id);
        }



        private string BuildJsonRequestString(int from, int size, string index, string columns)
        {
            var anonObj = new
            {
                indexes = index,
                columns,
                body = new
                {
                    from,
                    size,
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
        private async Task<HttpResponseMessage> SendRequestAsync(string body, string endpoint)
        {

            HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = await client.SendAsync(rM);
            Console.Write(result);
            return result;
        }
        private HttpResponseMessage SendRequest(string body, string endpoint)
        {
            HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, baseAddress + endpoint);
            client.DefaultRequestHeaders.Add("private_key", apikey);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = client.Send(rM);
            Console.Write(result);
            return result;

        }
    }
}
