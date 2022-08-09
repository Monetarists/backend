using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api.Repositories;
using XIVMarketBoard_Api.Repositories.Models.Universalis;
using XIVMarketBoard_Api.Events;
using XIVMarketBoard_Api.Helpers;
using Coravel.Queuing.Interfaces;

namespace XIVMarketBoard_Api.Controller
{
    public interface IUniversalisApiController
    {
        IEnumerable<MbPost> CreateMbPostEntries(IEnumerable<UniversalisListings> listings);
        IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> saleHistoryList);
        UniversalisEntry CreateUniversalisEntry(UniversalisResponseItems responseItem, World world, Item item);
        Task<string> ImportUniversalisDataForAllItemsOnWorld(World world);
        Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world);
        Task<string> ImportMarketableItems();
        Task<IEnumerable<UniversalisEntry>> ImportUniversalisDataForItemListAndWorld(List<Item> itemList, World world);
    }

    public class UniversalisApiController : IUniversalisApiController
    {
        private readonly int _calloutSize = 100;
        private readonly IMarketBoardController _marketBoardApiController;
        private readonly IUniversalisApiRepository _universalisApiRepository;
        private readonly IRecipeController _recipeController;
        private readonly IQueue _queue;

        public UniversalisApiController(IMarketBoardController dbController, IUniversalisApiRepository universalisApiRepositiory, IRecipeController recipeController, IQueue queue)
        {
            _marketBoardApiController = dbController;
            _universalisApiRepository = universalisApiRepositiory;
            _recipeController = recipeController;
            _queue = queue;
        }



        public async Task<string> ImportMarketableItems()
        {
            var response = await _universalisApiRepository.GetUniversalisListMarketableItems();
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<int>>(await response.Content.ReadAsStringAsync());
                if (res == null) return "";

                var items = await _recipeController.GetItemsByIds(res).ToListAsync();
                items.ForEach(item => item.IsMarketable = true);
                return await _recipeController.UpdateItems(items);
            }
            return "Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync();
        }
        public async Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world)
        {
            var response = await _universalisApiRepository.GetUniversalisEntryForItems(new string[] { item.Id.ToString() }, world.Name);
            if (response.IsSuccessStatusCode)
            {

                var parsedResult = JsonConvert.DeserializeObject<UniversalisResponseItems>(await response.Content.ReadAsStringAsync()) ??
                    throw new AppException("response from universalis is null " + item.Id + " " + item.Name_en);

                var uniEntry = CreateUniversalisEntry(parsedResult, world, item);
                var resultList = await _marketBoardApiController.GetOrCreateUniversalisQueries(new List<UniversalisEntry>() { uniEntry });
                return resultList.First();

            }
            throw new HttpRequestException("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
        }
        public async Task<IEnumerable<UniversalisEntry>> ImportUniversalisDataForItemListAndWorld(List<Item> itemList, World world)
        {
            List<UniversalisEntry> uniList = new List<UniversalisEntry>();

            for (var amount = 0; itemList.Count >= amount; amount += _calloutSize)
            {
                var itemColl = itemList.Skip(amount).Take(_calloutSize);
                var response = await _universalisApiRepository.GetUniversalisEntryForItems(itemColl.Select(i => i.Id.ToString()), world.Name);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
                }
                var parsedResult = JsonConvert.DeserializeObject<UniversalisResponse>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("response from universalis is null");
                if (!parsedResult.items.Any())
                {
                    var item = JsonConvert.DeserializeObject<UniversalisResponseItems>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("response from universalis is null");
                    parsedResult.items = new List<UniversalisResponseItems>() { item };
                }

                uniList.AddRange(parsedResult.items.Select(i => CreateUniversalisEntry(i, world, itemColl.FirstOrDefault(r => r.Id.ToString() == i.itemId) ?? throw new Exception("item is null"))));
                //wait for api ratelimiting
                await Task.Delay(80);

            }

            var resultList = await _marketBoardApiController.GetOrCreateUniversalisQueries(uniList);
            return resultList;
        }

        public async Task<string> ImportUniversalisDataForAllItemsOnWorld(World world)
        {
            var itemList = _recipeController.GetAllItems().Result.Where(r => r.IsMarketable.HasValue && r.IsMarketable.Value).ToList();
            await ImportUniversalisDataForItemListAndWorld(itemList, world);
            return "Imported all items on world " + world.Name;
        }


        public UniversalisEntry CreateUniversalisEntry(UniversalisResponseItems responseItem, World world, Item item) =>

             new UniversalisEntry
             {
                 Item = item,
                 World = world,
                 LastUploadDate = _universalisApiRepository.UnixTimeStampToDateTimeMilliSeconds(responseItem.lastUploadTime),
                 QueryDate = DateTime.Now,
                 Posts = CreateMbPostEntries(responseItem.listings).ToList(), // list listings
                 SaleHistory = CreateSaleHistoryEntries(responseItem.recentHistory).ToList(), //list recentHistory
                 CurrentAveragePrice = responseItem.currentAveragePrice,
                 CurrentAveragePrinceNQ = responseItem.currentAveragePriceNQ,
                 CurrentAveragePriceHQ = responseItem.currentAveragePriceHQ,
                 RegularSaleVelocity = responseItem.regularSaleVelocity,
                 NqSaleVelocity = responseItem.nqSaleVelocity,
                 HqSaleVelocity = responseItem.hqSaleVelocity,
                 AveragePrice = responseItem.averagePrice,
                 AveragePriceNQ = responseItem.averagePriceNQ,
                 AveragePriceHQ = responseItem.averagePriceHQ,
                 MinPrice = responseItem.minPrice,
                 MinPriceNQ = responseItem.minPriceNQ,
                 MinPriceHQ = responseItem.minPriceHQ,
                 MaxPrice = responseItem.maxPrice,
                 MaxPriceNQ = responseItem.maxPriceNQ,
                 MaxPriceHQ = responseItem.maxPriceHQ,
                 NqListingsCount = responseItem.listings.Count(x => !x.hq),
                 HqListingsCount = responseItem.listings.Count(x => x.hq),
                 NqSaleCount = responseItem.recentHistory.Where(s => _universalisApiRepository.UnixTimeStampToDateTimeSeconds(s.timestamp) > DateTime.UtcNow.AddDays(-1)).Count(s => !s.hq),
                 HqSaleCount = responseItem.recentHistory.Where(s => _universalisApiRepository.UnixTimeStampToDateTimeSeconds(s.timestamp) > DateTime.UtcNow.AddDays(-1)).Count(s => s.hq)

             };



        public IEnumerable<MbPost> CreateMbPostEntries(IEnumerable<UniversalisListings> listings) =>

            listings.Select(i => new MbPost()
            {
                //Id = i.listingID ?? Guid.NewGuid().ToString(),
                RetainerName = i.retainerName,
                SellerId = i.sellerID,
                Price = i.pricePerUnit,
                Amount = i.quantity,
                TotalAmount = i.total,
                LastReviewDate = _universalisApiRepository.UnixTimeStampToDateTimeSeconds(i.lastReviewTime)
            });


        public IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> saleHistoryList) =>
            saleHistoryList.Where(s => _universalisApiRepository.UnixTimeStampToDateTimeSeconds(s.timestamp) > DateTime.UtcNow.AddDays(-1)).Select(i =>
                new SaleHistory()
                {
                    Quantity = i.quantity,
                    SaleDate = _universalisApiRepository.UnixTimeStampToDateTimeSeconds(i.timestamp),
                    Total = i.total,
                    BuyerName = i.buyerName,
                    HighQuality = i.hq
                });
    }
}
