using Esendex.TokenBucket;
using System.Threading;
namespace XIVMarketBoard_Api.Repositories
{
    public interface IUniversalisApiRepository
    {
        Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string world);
        DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp);
        DateTime UnixTimeStampToDateTimeMilliSeconds(double unixTimeStamp);
        Task<HttpResponseMessage> GetUniversalisListMarketableItems();
    }

    public class UniversalisApiRepository : IUniversalisApiRepository
    {
        //listings = number of current posts
        //entries = number of history
        public const string baseAddress = "https://universalis.app/api/";
        //private static readonly HttpClient client = new HttpClient();
        //24h in s
        private static int entriesWithinSeconds = 604800;
        private static int nrOfEntries = 300;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenBucket _bucket;
        private readonly SemaphoreSlim _semaphore;
        public UniversalisApiRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _bucket = _bucket = TokenBuckets.Construct()
              .WithCapacity(35)
              .WithFixedIntervalRefillStrategy(25, TimeSpan.FromSeconds(1))
              .Build();
            _semaphore = new SemaphoreSlim(8);
        }

        public async Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string world)
        {

            var idString = string.Join(",", idList);
            //removed listings to get all listgings from the api. 
            var requestAddress = baseAddress + world + "/" + idString + "?" + "entries=" + nrOfEntries + "&entriesWithin=" + entriesWithinSeconds;
            var response = await SendRequestAsync(requestAddress);
            return response;
        }
        public async Task<HttpResponseMessage> GetUniversalisListMarketableItems()
        {
            var requestAddress = baseAddress + "marketable";
            var response = await SendRequestAsync(requestAddress);
            return response;
        }

        public DateTime UnixTimeStampToDateTimeMilliSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp);
            return dateTime;
        }
        public DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime;
        }
        private async Task<HttpResponseMessage> SendRequestAsync(string endpoint)
        {
            while (true)
            {
                var client = _httpClientFactory.CreateClient();
                HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, endpoint);
                _semaphore.Wait();
                _bucket.Consume(1);
                var response = await client.SendAsync(rM);
                _semaphore.Release();
                return response;
            }

        }
    }



}
