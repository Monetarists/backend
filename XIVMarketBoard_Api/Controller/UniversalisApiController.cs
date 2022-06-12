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
        Task<string> ImportUniversalisDataForAllItemsOnWorld(World world);
        Task<string> ImportPostsForItems(IEnumerable<Item> itemList, World world);
        Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, int entries, int listings);
        Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, int entries, int listings);
        Task<string> ImportMarketableItems();

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
        public async Task<string> ImportMarketableItems()
        {
            
            var response = await _universalisApiRepository.GetUniversalisListMarketableItems();
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var items = _dbController.GetItemsByIds(JsonConvert.DeserializeObject<List<int>>(res)).ToListAsync().Result;
                    items.ForEach(item => item.IsMarketable = true);

                    return await _dbController.UpdateItems(items);
                }
                catch (Exception e)
                {
                    throw new Exception("Unhandeled exception white importing universalisdata " + e.Message);
                }

            }
            else throw new Exception("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
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
        public async Task<string> ImportUniversalisDataForAllItemsOnWorld(World world)
        {
            try
            {
                List<UniversalisEntry> uniList = new List<UniversalisEntry>();
                var itemList = _dbController.GetAllItems().ToListAsync().Result;
                
                for(var amount = 0;  itemList.Count() >= amount; amount += 100)
                {

                    var itemColl =  itemList.Skip(amount).Take(100);
                    var response = await _universalisApiRepository.GetUniversalisEntryForItems(itemColl.Select(i => i.Id.ToString()), "", world.Name, 5, 5);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var res = await response.Content.ReadAsStringAsync();
                            var parsedResult = JsonConvert.DeserializeObject<UniversalisResponse>(await response.Content.ReadAsStringAsync());
                            if (parsedResult == null)
                            {
                                throw new NullReferenceException();
                            }
                            foreach(var i in parsedResult.items)
                            {
                                var a = itemColl.FirstOrDefault(b => b.Id.ToString() == i.itemId);
                                uniList.Add(CreateUniversalisEntry(i, world, a));
                            }
                            //uniList.AddRange(itemColl.Select(i => CreateUniversalisEntry(parsedResult.items.Where(a => a.itemId == i.Id.ToString()).First(), world, i)));

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Unhandeled exception white importing universalisdata " + e.Message);
                        }

                    }
                    else throw new Exception("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
                    await Task.Delay(80);
                    amount += 100;
                }

                var result = await _dbController.GetOrCreateUniversalisQueries(uniList);
                //uniList.ForEach(async i => i = await _dbController.GetOrCreateUniversalisQuery(i));

                return "Imported all items on world " + world.Name;
                
            }
            
            catch (Exception e)
            {
                throw new Exception("Unhandeled exception white importing universalisdata " + e.Message);
            }


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
                LastUploadDate = _universalisApiRepository.UnixTimeStampToDateTimeMilliSeconds(responseItem.lastUploadTime),
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
                LastReviewDate = _universalisApiRepository.UnixTimeStampToDateTimeSeconds(i.lastReviewTime)
            });


        public IEnumerable<SaleHistory> CreateSaleHistoryEntries(IEnumerable<UniversalisRecentHistory> listings) =>
            listings.Select(i =>
                new SaleHistory ()
                {
                    Quantity = i.quantity,
                    SaleDate = _universalisApiRepository.UnixTimeStampToDateTimeSeconds(i.timestamp),
                    Total = i.total,
                    BuyerName = i.buyerName,
                    HighQuality = i.hq
                });
            
        
    }
}
