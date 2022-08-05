using Newtonsoft.Json;

namespace XIVMarketBoard_Api.Repositories
{
    public interface IUniversalisApiRepository
    {
        Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string world, int entriesWithinSeconds);
        DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp);
        DateTime UnixTimeStampToDateTimeMilliSeconds(double unixTimeStamp);
        Task<HttpResponseMessage> GetUniversalisListMarketableItems();
    }

    public class UniversalisApiRepository : IUniversalisApiRepository
    {
        //listings = number of current posts
        //entries = number of history
        public const string baseAddress = "https://universalis.app/api/";
        private static readonly HttpClient client = new HttpClient();
        //24h in s
        private static int entriesWithinSeconds = 86400;
        private static int nrOfEntries = 300;
        public async Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string world, int overrideEntriesWithinSeconds)
        {
            if (overrideEntriesWithinSeconds != 0)
            {
                entriesWithinSeconds = overrideEntriesWithinSeconds;
            }
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
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        private async Task<HttpResponseMessage> SendRequestAsync(string endpoint)
        {

            HttpRequestMessage rM = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var response = await client.SendAsync(rM);
            return response;
        }
    }



}
