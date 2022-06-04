using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
namespace XIVMarketBoard_Api
{
    public class UniversalisApiController
    {
        public static async Task<UniversalisEntry> ImportUniversalisDataForItemAndWorld(Item item, World world, string entries, string listings)
        {
            List<UniversalisEntry> list = new List<UniversalisEntry>();
            var response = await UniversalisApiModel.GetCurrentListings(new List<string>() { item.Id.ToString() }, "", world.Name, listings, entries);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var res = await response.Content.ReadAsStringAsync();
                    var parsedResult = JsonConvert.DeserializeObject<UniversalisApiModel.responseItems>(await response.Content.ReadAsStringAsync());
                    var uniquery = createUniversalisEntry(parsedResult, world, item);
                    uniquery = await DbController.GetOrCreateUniversalisQuery(uniquery);
                    return uniquery;
                }
                catch (Exception e)
                {
                    throw new Exception("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
                }

            }
            else throw new Exception("Callout failed " + response.StatusCode + await response.Content.ReadAsStringAsync());
        }
        public static async Task<string> ImportPostsForItems(List<Item> itemList, World world, string entries, string listings)
        {
            List<UniversalisEntry> list = new List<UniversalisEntry>();
            foreach (var item in itemList)
            {
                var result = await UniversalisApiModel.GetCurrentListings(new List<string>() { item.Id.ToString() }, "", world.Name, listings, entries);
            }


            return "";
        }
        public static async Task<string> ImportPostsForRecipeAndComponents(Recipe recipe, World world, string entries, string listings)
        {
            /*List<UniversalisQuery> list = new List<UniversalisQuery>();
            foreach (var item in itemList)
            {
                var result = await UniversalisApiModel.GetCurrentListings(new List<string>() { item.Id.ToString() }, "", world.Name, listings, entries);
            }


            return list;*/
            return "";
        }
        public static UniversalisEntry createUniversalisEntry(UniversalisApiModel.responseItems responseItem, World world, Item item)
        {

                UniversalisEntry uniEntry = new UniversalisEntry();
                uniEntry.Item = item; // item.item;
                uniEntry.World = world;
                uniEntry.LastUploadDate = UniversalisApiModel.UnixTimeStampToDateTime(responseItem.lastUploadTime);
                uniEntry.QueryDate = DateTime.Now;
                uniEntry.Posts = createMbPostEntries(responseItem.listings); // list listings
                uniEntry.SaleHistory = createSaleHistoryEntries(responseItem.recentHistory); //list recentHistory
                uniEntry.CurrentAveragePrice = responseItem.currentAveragePrice;
                uniEntry.CurrentAveragePrinceNQ = responseItem.currentAveragePrinceNQ;
                uniEntry.CurrentAveragePriceHQ = responseItem.currentAveragePriceHQ;
                uniEntry.RegularSaleVelocity = responseItem.regularSaleVelocity;
                uniEntry.NqSaleVelocity = responseItem.nqSaleVelocity;
                uniEntry.HqSaleVelocity = responseItem.hqSaleVelocity;
                uniEntry.AveragePrice = responseItem.averagePrice;
                uniEntry.AveragePriceNQ = responseItem.averagePriceNQ;
                uniEntry.AveragePriceHQ = responseItem.averagePriceHQ;
                uniEntry.MinPrice = responseItem.minPrice;
                uniEntry.MinPriceNQ = responseItem.minPriceNQ;
                uniEntry.MinPriceHQ = responseItem.minPriceHQ;
                uniEntry.MaxPrice = responseItem.maxPrice;
                uniEntry.MaxPriceNQ = responseItem.maxPriceNQ;
                uniEntry.MaxPriceHQ = responseItem.maxPriceHQ;
            return uniEntry;


        }
        public static List<MbPost> createMbPostEntries(List<UniversalisApiModel.Listings> listings)
        {
            List<MbPost> postList = new List<MbPost>();
            foreach (var i in listings)
            {
                MbPost post = new MbPost();
                post.Id = i.listingID;
                post.RetainerName = i.retainerName;
                post.SellerId = i.sellerID;
                //post.RetainerId = i.retainerID;
                post.Price = i.pricePerUnit;
                post.Amount = i.quantity;
                post.TotalAmount = i.total;
                post.LastReviewDate = UniversalisApiModel.UnixTimeStampToDateTime(i.lastReviewTime);
                //post.QueryDate = DateTime.Now;
                postList.Add(post);
            }
            return postList;
        }
        /*

                 public int Id { get; set; }
                public virtual User? User { get; set; }
                public virtual Retainer? Retainer { get; set; }
                public virtual Item Item { get; set; }
                public string RetainerName { get; set; }
                public int Price { get; set; }
                public int Amount { get; set; }
                public int TotalAmount { get; set; }
                public bool HighQuality { get; set; }
                public DateTime LastReviewDate { get; set; }       
                public World World { get; set; }

         */
        public static List<SaleHistory> createSaleHistoryEntries(List<UniversalisApiModel.RecentHistory> listings)
        {
            List<SaleHistory> saleList = new List<SaleHistory>();
            foreach (var i in listings)
            {
                SaleHistory sale = new SaleHistory();
                sale.Quantity = i.quantity;
                sale.SaleDate = UniversalisApiModel.UnixTimeStampToDateTime(i.timestamp);
                sale.Total = i.total;
                sale.BuyerName = i.buyerName;
                sale.HighQuality = i.hq;
                saleList.Add(sale);
            }
            return saleList;
        }
    }
}
 