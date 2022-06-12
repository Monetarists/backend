using Newtonsoft.Json;

namespace XIVMarketBoard_Api.Repositories
{
    public interface IUniversalisApiRepository
    {
        Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string hq, string world, int listings, int entries);
        DateTime UnixTimeStampToDateTimeSeconds(double unixTimeStamp);
        DateTime UnixTimeStampToDateTimeMilliSeconds(double unixTimeStamp);
    }

    public class UniversalisApiRepository : IUniversalisApiRepository
    {
        //listings = number of current posts
        //entries = number of history
        public const string baseAddress = "https://universalis.app/api/";
        private static readonly HttpClient client = new HttpClient();
        public async Task<HttpResponseMessage> GetUniversalisEntryForItems(IEnumerable<string> idList, string hq, string world, int listings, int entries)
        {
            var idString = string.Join(",", idList);
            var requestAddress = baseAddress + world + "/" + idString + "?listings=" + listings + "&entries=" + entries + "&hq=" + hq;
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
