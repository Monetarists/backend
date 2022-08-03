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
        private const string apikey = Credentials.XIVApiKey;

        private static readonly HttpClient client = new HttpClient();

        private static string getRecipeColumns()
        {
            var columns = "ID,Name_de,Name_en,Name_fr,Name_ja,IconID,AmountResult,IsExpert,IsSpecializationRequired" +
                ",ClassJob.ID,ClassJob.Abbreviation,ClassJob.Name_de,ClassJob.Name_en,ClassJob.Name_fr,ClassJob.Name_ja" +
                ",ClassJob.Icon,ClassJob.ClassJobCategoryTargetID,ClassJob.DohDolJobIndex" +
                ",ItemResult.ID,ItemResult.Name_de,ItemResult.Name_en,ItemResult.Name_fr,ItemResult.Name_ja,ItemResult.IconID,ItemResult.IsUntradable,ItemResult.CanBeHq" +
                ",ItemResult.ItemSearchCategory.ID,ItemResult.ItemSearchCategory.Name_de,ItemResult.ItemSearchCategory.Name_en,ItemResult.ItemSearchCategory.Name_fr,ItemResult.ItemSearchCategory.Name_ja" +
                ",ItemResult.ItemUICategory.ID,ItemResult.ItemUICategory.Name_de,ItemResult.ItemUICategory.Name_en,ItemResult.ItemUICategory.Name_fr,ItemResult.ItemUICategory.Name_ja";
            for (int i = 0; i <= 9; i++)
            {
                columns = columns + ",AmountIngredient" + i +
                    ",ItemIngredient" + i + ".ID,ItemIngredient" + i + ".Name_de,ItemIngredient" + i +
                    ".Name_en,ItemIngredient" + i + ".Name_fr,ItemIngredient" + i + ".Name_ja" +
                    ",ItemIngredient" + i + ".IconID,ItemIngredient" + i + ".IsUntradable,ItemIngredient" + i + ".CanBeHq" +
                    ",ItemIngredient" + i + ".ItemSearchCategory.ID,ItemIngredient" + i + ".ItemSearchCategory.Name_de,ItemIngredient" + i +
                    ".ItemSearchCategory.Name_en,ItemIngredient" + i + ".ItemSearchCategory.Name_fr,ItemIngredient" + i + ".ItemSearchCategory.Name_ja" +
                    ",ItemIngredient" + i + ".ItemUICategory.ID,ItemIngredient" + i + ".ItemUICategory.Name_de,ItemIngredient" + i +
                    ".ItemUICategory.Name_en,ItemIngredient" + i + ".ItemUICategory.Name_fr,ItemIngredient" + i + ".ItemUICategory.Name_ja";

            }
            return columns;
        }
        public async Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems)
        {

            var response = await SendRequestAsync(BuildJsonRequestString(startNumber, amountOfItems, "items", getRecipeColumns()), "search");

            return response;

        }
        public async Task<HttpResponseMessage> GetRecipesAsync(int start, int amount)
        {

            var response = await SendRequestAsync(
                 BuildJsonRequestString(start, amount, "recipe", getRecipeColumns()), "search");
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
