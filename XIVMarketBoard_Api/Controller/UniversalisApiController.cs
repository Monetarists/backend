﻿using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api.Repositories;
using XIVMarketBoard_Api.Repositories.Models.Universalis;

using System;

namespace XIVMarketBoard_Api.Controller
{
    public interface IUniversalisApiController
    {
        IEnumerable<MbPost> CreateMbPostEntries(IEnumerable<UniversalisListings> listings);
        IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> listings);
        UniversalisEntry CreateUniversalisEntry(UniversalisResponseItems responseItem, World world, Item item);
        Task<string> ImportUniversalisDataForAllItemsOnWorld(World world);
        Task<string> ImportPostsForItems(IEnumerable<Item> itemList, World world);
        Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, int entries, int listings);
        Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, int entries, int listings);
        Task<string> ImportMarketableItems();
        Task<IEnumerable<UniversalisEntry>> ImportUniversalisDataForItemListAndWorld(List<Item> itemList, World world, int entries, int listings);
    }

    public class UniversalisApiController : IUniversalisApiController
    {
        private int calloutSize = 100;
        private readonly IMarketBoardController _marketBoardApiController;
        private readonly IUniversalisApiRepository _universalisApiRepository;
        private readonly IRecipeController _recipeController;

        public UniversalisApiController(IMarketBoardController dbController, IUniversalisApiRepository universalisApiRepositiory, IRecipeController recipeController)
        {
            _marketBoardApiController = dbController;
            _universalisApiRepository = universalisApiRepositiory;
            _recipeController = recipeController;
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
        public async Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, int entries, int listings)
        {
            var result = new UniversalisEntry();
            var response = await _universalisApiRepository.GetUniversalisEntryForItems(new string[] { item.Id.ToString() }, "", world.Name, listings, entries);
            if (response.IsSuccessStatusCode)
            {

                var parsedResult = JsonConvert.DeserializeObject<UniversalisResponseItems>(await response.Content.ReadAsStringAsync()) ?? throw new ArgumentNullException("response from universalis is null");

                var uniEntry = CreateUniversalisEntry(parsedResult, world, item);
                var resultList = await _marketBoardApiController.GetOrCreateUniversalisQueries(new List<UniversalisEntry>() { uniEntry });
                return resultList.First();

            }
            throw new HttpRequestException("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
        }
        public async Task<IEnumerable<UniversalisEntry>> ImportUniversalisDataForItemListAndWorld(List<Item> itemList, World world, int entries, int listings)
        {
            List<UniversalisEntry> uniList = new List<UniversalisEntry>();
            //TODO fix an get marketable items only

            for (var amount = 0; itemList.Count >= amount; amount += calloutSize)
            {

                var itemColl = itemList.Skip(amount).Take(calloutSize);
                var response = await _universalisApiRepository.GetUniversalisEntryForItems(itemColl.Select(i => i.Id.ToString()), "", world.Name, 5, 5);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
                }
                var test = await response.Content.ReadAsStringAsync();
                var parsedResult = JsonConvert.DeserializeObject<UniversalisResponse>(await response.Content.ReadAsStringAsync()) ?? throw new ArgumentNullException("response from universalis is null");
                if (parsedResult.items.Count() == 0)
                {
                    var item = JsonConvert.DeserializeObject<UniversalisResponseItems>(await response.Content.ReadAsStringAsync()) ?? throw new ArgumentNullException("response from universalis is null");
                    parsedResult.items = new List<UniversalisResponseItems>() { item };
                }
                uniList.AddRange(parsedResult.items.Select(i => CreateUniversalisEntry(i, world, itemColl.FirstOrDefault(r => r.Id.ToString() == i.itemId) ?? throw new ArgumentNullException("item is null"))));
                //wait for api ratelimiting
                await Task.Delay(80);

            }
            var resultList = await _marketBoardApiController.GetOrCreateUniversalisQueries(uniList);

            return resultList;
        }
        public async Task<string> ImportUniversalisDataForAllItemsOnWorld(World world)
        {
            var uniList = new List<UniversalisEntry>();
            //TODO fix an get marketable items only
            var itemList = _recipeController.GetAllItems().ToListAsync().Result.Where(r => (bool)r.IsMarketable).ToList();
            var universalisResults = await ImportUniversalisDataForItemListAndWorld(itemList, world, 5, 5);
            return "Imported all items on world " + world.Name;
        }
        public async Task<string> ImportPostsForItems(IEnumerable<Item> itemList, World world)
        {
            //TODO figure out if needed
            foreach (var item in itemList)
            {
                var result = await _universalisApiRepository.GetUniversalisEntryForItems(new List<string>() { item.Id.ToString() }, "", world.Name, 5, 5);
            }


            return "";
        }
        public async Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, int entries, int listings)
        {
            //TODO figure out if needed

            /*List<UniversalisQuery> list = new List<UniversalisQuery>();
            foreach (var item in itemList)
            {
                var result = await UniversalisApiModel.GetCurrentListings(new List<string>() { item.Id.ToString() }, "", world.Name, listings, entries);
            }


            return list;*/
            return "";
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
                 MaxPriceHQ = responseItem.maxPriceHQ
             };



        public IEnumerable<MbPost> CreateMbPostEntries(IEnumerable<UniversalisListings> listings) =>

            listings.Select(i => new MbPost()
            {
                Id = i.listingID ?? Guid.NewGuid().ToString(),
                RetainerName = i.retainerName,
                SellerId = i.sellerID,
                Price = i.pricePerUnit,
                Amount = i.quantity,
                TotalAmount = i.total,
                LastReviewDate = _universalisApiRepository.UnixTimeStampToDateTimeSeconds(i.lastReviewTime)
            });


        public IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> listings) =>
            listings.Select(i =>
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
