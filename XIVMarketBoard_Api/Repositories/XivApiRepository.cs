using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace XIVMarketBoard_Api.Repositories
{

    public interface IXivApiRepository
    {

        Task<HttpResponseMessage> GetAllWorldsAsync();
        Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems);
        Task<HttpResponseMessage> GetRecipesAsync(int start, int amount);
        Task<HttpResponseMessage> GetWorldDetailsAsync(int Id);
        Task<string> testsemaphore();
    }

    public class XivApiRepository : IXivApiRepository
    {
        private const string baseAddress = "https://xivapi.com/";
        private readonly IConfiguration configuration;

        private readonly IHttpClientFactory _httpClientFactory;

        public XivApiRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }



        public async Task<HttpResponseMessage> GetItemsAsync(int startNumber, int amountOfItems)
        {

            var response = await SendRequestAsync(BuildJsonRequestString(startNumber, amountOfItems, "items", getRecipeColumns()), "search", _httpClientFactory);

            return response;

        }
        public async Task<HttpResponseMessage> GetRecipesAsync(int start, int amount)
        {

            var response = await SendRequestAsync(
                 BuildJsonRequestString(start, amount, "recipe", getRecipeColumns()), "search" + "?private_key=" + configuration.GetSection("ApiKey:XivApiKey").Value, _httpClientFactory);
            return response;

        }

        public async Task<HttpResponseMessage> GetAllWorldsAsync()
        {
            return await SendRequestAsync("", "world?limit=3000" + "&private_key=" + configuration.GetSection("ApiKey:XivApiKey").Value, _httpClientFactory);
        }
        public async Task<HttpResponseMessage> GetWorldDetailsAsync(int Id)
        {
            return await SendRequestAsync("", "world/" + Id + "?private_key=" + configuration.GetSection("ApiKey:XivApiKey").Value, _httpClientFactory);
        }
        public async Task<string> testsemaphore()
        {
            return await SendRequestAsyncdothing();
        }
        private static async Task<HttpResponseMessage> SendRequestAsync(string body, string endpoint, IHttpClientFactory _httpClientFactory)
        {
            var semaphore = new SemaphoreSlim(30);
            var client = _httpClientFactory.CreateClient();
            HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, baseAddress + endpoint);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            rM.Content = content;
            var result = await client.SendAsync(rM);
            Console.Write(result);
            return result;
        }
        private static async Task<string> SendRequestAsyncdothing()
        {
            Debug.WriteLine("starting");
            var semaphore = new SemaphoreSlim(5);

            await semaphore.WaitAsync();
            Debug.WriteLine("entered");
            Thread.Sleep(10000);
            Debug.WriteLine("exit");
            semaphore.Release();
            return "abc";

        }
        private static string BuildJsonRequestString(int from, int size, string index, string columns)
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
        private static string getRecipeColumns()
        {
            StringBuilder bld = new StringBuilder();
            bld.Append("ID,Name_de,Name_en,Name_fr,Name_ja,IconID,AmountResult,IsExpert,IsSpecializationRequired" +
                ",ClassJob.ID,ClassJob.Abbreviation,ClassJob.Name_de,ClassJob.Name_en,ClassJob.Name_fr,ClassJob.Name_ja" +
                ",ClassJob.Icon,ClassJob.ClassJobCategoryTargetID,ClassJob.DohDolJobIndex" +
                ",ItemResult.ID,ItemResult.Name_de,ItemResult.Name_en,ItemResult.Name_fr,ItemResult.Name_ja,ItemResult.IconID,ItemResult.IsUntradable,ItemResult.CanBeHq" +
                ",ItemResult.ItemSearchCategory.ID,ItemResult.ItemSearchCategory.Name_de,ItemResult.ItemSearchCategory.Name_en,ItemResult.ItemSearchCategory.Name_fr,ItemResult.ItemSearchCategory.Name_ja" +
                ",ItemResult.ItemUICategory.ID,ItemResult.ItemUICategory.Name_de,ItemResult.ItemUICategory.Name_en,ItemResult.ItemUICategory.Name_fr,ItemResult.ItemUICategory.Name_ja");
            for (int i = 0; i <= 9; i++)
            {
                bld.Append(",AmountIngredient" + i +
                    ",ItemIngredient" + i + ".ID,ItemIngredient" + i + ".Name_de,ItemIngredient" + i +
                    ".Name_en,ItemIngredient" + i + ".Name_fr,ItemIngredient" + i + ".Name_ja" +
                    ",ItemIngredient" + i + ".IconID,ItemIngredient" + i + ".IsUntradable,ItemIngredient" + i + ".CanBeHq" +
                    ",ItemIngredient" + i + ".ItemSearchCategory.ID,ItemIngredient" + i + ".ItemSearchCategory.Name_de,ItemIngredient" + i +
                    ".ItemSearchCategory.Name_en,ItemIngredient" + i + ".ItemSearchCategory.Name_fr,ItemIngredient" + i + ".ItemSearchCategory.Name_ja" +
                    ",ItemIngredient" + i + ".ItemUICategory.ID,ItemIngredient" + i + ".ItemUICategory.Name_de,ItemIngredient" + i +
                    ".ItemUICategory.Name_en,ItemIngredient" + i + ".ItemUICategory.Name_fr,ItemIngredient" + i + ".ItemUICategory.Name_ja");

            }
            return bld.ToString();
        }
    }
}
