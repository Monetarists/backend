using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api.Repositories;
using XIVMarketBoard_Api.Repositories.Models;

namespace XIVMarketBoard_Api.Controller
{
    public interface IUniversalisApiController
    {
        IEnumerable<MbPost> CreateMbPostEntries(IEnumerable<UniversalisListings> listings);
        IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> listings);
        UniversalisEntry CreateUniversalisEntry(UniversalisResponseItems responseItem, World world, Item item);
        Task<string> ImportForAllItemsOnWorld(World world);
        Task<string> ImportPostsForItems(IEnumerable<Item> itemList, World world);
        Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, int entries, int listings);
        Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, int entries, int listings);
    }

    public class UniversalisApiController : IUniversalisApiController
    {
        private readonly IDbController _dbController;
        private readonly IUniversalisApiRepository _universalisApiRepository;

        public UniversalisApiController(IDbController dbController, IUniversalisApiRepository universalisApiRepositiorry)
        {
            _dbController = dbController;
            _universalisApiRepository = universalisApiRepositiorry;
        }

        public async Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, int entries, int listings)
        {

            var response = await _universalisApiRepository.GetUniversalisEntryForItems(new string[] { item.Id.ToString() }, "", world.Name, listings, entries);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var parsedResult = JsonConvert.DeserializeObject<UniversalisResponseItems>(await response.Content.ReadAsStringAsync());
                    if(parsedResult == null)
                    {
                        throw new NullReferenceException();
                    }
                    var uniEntry = CreateUniversalisEntry(parsedResult, world, item);
                    uniEntry = await _dbController.GetOrCreateUniversalisQuery(uniEntry);
                    return uniEntry;
                }
                catch (Exception e)
                {
                    throw new Exception("Unhandeled exception white importing universalisdata " + e.Message);
                }

            }
            else throw new Exception("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
        }
        public async Task<string> ImportForAllItemsOnWorld(World world)
        {
            var itemList = _dbController.GetAllItems();
            await foreach (var item in itemList)
            {
                var result = await _universalisApiRepository.GetUniversalisEntryForItems(new List<string>() { item.Id.ToString() }, "", world.Name, 5, 5);
            }


            return "";
        }
        public async Task<string> ImportPostsForItems(IEnumerable<Item> itemList, World world)
        {
            foreach (var item in itemList)
            {
                var result = await _universalisApiRepository.GetUniversalisEntryForItems(new List<string>() { item.Id.ToString() }, "", world.Name, 5, 5);
            }


            return "";
        }
        public async Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, int entries, int listings)
        {
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
                Item = item, // item.item;
                World = world,
                LastUploadDate = _universalisApiRepository.UnixTimeStampToDateTime(responseItem.lastUploadTime),
                QueryDate = DateTime.Now,
                Posts = CreateMbPostEntries(responseItem.listings).ToList(), // list listings
                SaleHistory = CreateSaleHistoryEntries(responseItem.recentHistory).ToList(), //list recentHistory
                CurrentAveragePrice = responseItem.currentAveragePrice,
                CurrentAveragePrinceNQ = responseItem.currentAveragePrinceNQ,
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
                Id = i.listingID,
                RetainerName = i.retainerName,
                SellerId = i.sellerID,
                Price = i.pricePerUnit,
                Amount = i.quantity,
                TotalAmount = i.total,
                LastReviewDate = _universalisApiRepository.UnixTimeStampToDateTime(i.lastReviewTime)
            });


        public IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> listings) =>
            listings.Select(i =>
                new SaleHistory ()
                {
                    Quantity = i.quantity,
                    SaleDate = _universalisApiRepository.UnixTimeStampToDateTime(i.timestamp),
                    Total = i.total,
                    BuyerName = i.buyerName,
                    HighQuality = i.hq
                });
            
        
    }
}
