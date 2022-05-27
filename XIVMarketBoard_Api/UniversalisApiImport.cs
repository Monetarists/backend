namespace XIVMarketBoard_Api
{
    public class UniversalisApiImport
    {
        public const string baseAddress = "https://universalis.app/api/";
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetCurrentListings(List<string> idList, string hq, string world, string listings, string entries)
        {
            var idString = String.Join(",", idList);
            var requestAddress = baseAddress + world + "/" + idString + "?listings=" + listings + "?entries=" + entries + hq == null ? "" : "?" + hq;
            var result = await SendRequestAsync(requestAddress);
            return result;
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        private static async Task<String> SendRequestAsync(string endpoint)
        {

            HttpRequestMessage rM = new HttpRequestMessage(System.Net.Http.HttpMethod.Get,endpoint);
            var result = await client.SendAsync(rM);
            var resultString = await result.Content.ReadAsStringAsync();
            Console.Write(result);
            return resultString;
        }
        public class Result
        {
            public List<string> itemIDs { get; set; }
            public List<Items> items { get; set; }
            public string worldID { get; set; }
            public string WorldName { get; set; }
        }
        public class Items
        {
            public string itemId { get; set; }
            public string worldId { get; set; }
            public int lastUploadTime { get; set; }
            public List<Listings> listings { get; set; }
            public List<RecentHistory> recentHistory { get; set; }
            public int currentAveragePrice { get; set; }
            public int currentAveragePrinceNQ { get; set; }
            public int currentAveragePriceHQ { get; set; }
            public int regularSaleVelocity { get; set; }
            public int nqSaleVelocity { get; set; }
            public int hqSaleVelocity { get; set; }
            public int averagePrice { get;set;}
            public int averagePriceNQ { get;set;}
            public int averagePriceHQ { get;set;}
            public int minPrice { get;set;}
            public int minPriceNQ { get; set; }
            public int minPriceHQ { get;set;}
            public int maxPrice { get;set;}
            public int maxPriceNQ { get; set; }
            public int maxPriceHQ { get; set; }
        }
        public class Listings
        {
            public int lastReviewTime { get; set; }
            public int pricePerUnit { get; set; }
            public int quantity { get; set; }
            public bool hq { get; set; }
            public int total { get; set; }
        }
        public class RecentHistory
        {
            public int pricePerUnit { get; set; }
            public int quantity { get; set; }
            public int timestamp { get; set; }
            public int total { get; set; }
        }
    }


    //https://universalis.app/api/Twintania/37347?listings=1&entries=5&hq=false
    /*
     listings = nr of posts up on the marketboard
    entries = nr of recent history posts to fetch
    hq =false only take nq items
    hq = true only take hq items
    hq = null take both

     
     */

}
/*
 response from universalis
  "itemIDs": [ 37347, 37348 ],
  "items": [
    {
      "itemID": 37347,
      "worldID": 33,
      "lastUploadTime": 1653675006936,
      "listings": [
        {
          "lastReviewTime": 1653663786,
          "pricePerUnit": 52467,
          "quantity": 1,
          "stainID": 0,
          "creatorName": "",
          "creatorID": "fe7e4a689de50ceed1fff7efd94530294a015ab38097512716bb8536cd1661c9",
          "hq": false,
          "isCrafted": true,
          "listingID": null,
          "materia": [],
          "onMannequin": false,
          "retainerCity": 12,
          "retainerID": "bb7a4597e4591f5e9e25fc9f0d87299e4bf868567f6d951d6cbb47469b7b1583",
          "retainerName": "Kayteleen",
          "sellerID": "fe7e4a689de50ceed1fff7efd94530294a015ab38097512716bb8536cd1661c9",
          "total": 52467
        }
      ],
      "recentHistory": [
        {
          "hq": false,
          "pricePerUnit": 49958,
          "quantity": 1,
          "timestamp": 1653661455,
          "buyerName": "Lady Wiezel",
          "total": 49958
        }
      ],
      "currentAveragePrice": 65636,
      "currentAveragePriceNQ": 65636,
      "currentAveragePriceHQ": 0,
      "regularSaleVelocity": 2,
      "nqSaleVelocity": 2,
      "hqSaleVelocity": 0,
      "averagePrice": 54784.74,
      "averagePriceNQ": 54784.74,
      "averagePriceHQ": 0,
      "minPrice": 52467,
      "minPriceNQ": 52467,
      "minPriceHQ": 0,
      "maxPrice": 77438,
      "maxPriceNQ": 77438,
      "maxPriceHQ": 0,
      "stackSizeHistogram": { "1": 27 },
      "stackSizeHistogramNQ": { "1": 27 },
      "stackSizeHistogramHQ": {},
      "worldName": "Twintania"
    },
    {
      "itemID": 37348,
      "worldID": 33,
      "lastUploadTime": 1653674551613,
      "listings": [
        {
          "lastReviewTime": 1653664000,
          "pricePerUnit": 47240,
          "quantity": 1,
          "stainID": 0,
          "creatorName": "",
          "creatorID": "0d2ea0158b5e120512cfb33ff2fc6eaeb578799ff06dd31c306b8596cfa0c8ca",
          "hq": false,
          "isCrafted": true,
          "listingID": null,
          "materia": [],
          "onMannequin": false,
          "retainerCity": 2,
          "retainerID": "c2f00687c42ec79c62379bd8a6f52fbfe9be6fb946f1aeb87c38b43503056634",
          "retainerName": "Meirya",
          "sellerID": "0d2ea0158b5e120512cfb33ff2fc6eaeb578799ff06dd31c306b8596cfa0c8ca",
          "total": 47240
        }
      ],
      "recentHistory": [
        {
          "hq": false,
          "pricePerUnit": 44990,
          "quantity": 1,
          "timestamp": 1653665883,
          "buyerName": "Naju Targaryen",
          "total": 44990
        }
      ],
      "currentAveragePrice": 56579.13,
      "currentAveragePriceNQ": 56579.13,
      "currentAveragePriceHQ": 0,
      "regularSaleVelocity": 2.857143,
      "nqSaleVelocity": 2.857143,
      "hqSaleVelocity": 0,
      "averagePrice": 48610.5,
      "averagePriceNQ": 48610.5,
      "averagePriceHQ": 0,
      "minPrice": 47240,
      "minPriceNQ": 47240,
      "minPriceHQ": 0,
      "maxPrice": 157500,
      "maxPriceNQ": 157500,
      "maxPriceHQ": 0,
      "stackSizeHistogram": { "1": 24 },
      "stackSizeHistogramNQ": { "1": 24 },
      "stackSizeHistogramHQ": {},
      "worldName": "Twintania"
    }
  ],
  "worldID": 33,
  "unresolvedItems": [],
  "worldName": "Twintania"
}
 */