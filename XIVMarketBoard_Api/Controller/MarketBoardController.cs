using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;


namespace XIVMarketBoard_Api.Controller
{
    public interface IMarketBoardController
    {
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId);
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName);

        Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList);
        Task<IEnumerable<UniversalisEntry?>> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName);
        Task<IEnumerable<UniversalisEntry>> GetLatestUniversalisQueryForItems(List<Item> itemList, World world);
    }

    public class MarketBoardController : IMarketBoardController
    {
        private readonly XivDbContext _xivContext;
        public MarketBoardController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }


        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10)).Include(c => c.Item)
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Id == worldId && i.Item.Id == itemId);
        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10)).Include(c => c.Item)
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Name == worldName && i.Item.Name_en == itemName);

        /*public async Task<IEnumerable<UniversalisEntry?>> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName) => await _xivContext.UniversalisEntries
            .Include(a => a.Item).Include(a => a.Posts.Take(10)).Include(a => a.SaleHistory.Take(10))
            .OrderByDescending(p => p.QueryDate)
            .Where(i => i.World.Name == worldName && itemNames.Contains(i.Item.Name)).ToListAsync();*/
        public async Task<IEnumerable<UniversalisEntry>> GetLatestUniversalisQueryForItems(List<Item> itemList, World world)
        {
            List<UniversalisEntry> result = new List<UniversalisEntry>();
            foreach (var item in itemList)
            {
                var entry = await _xivContext.UniversalisEntries.Include(x => x.Item).Where(x => x.Item.Id == item.Id && x.World == world).FirstOrDefaultAsync();
                if (entry != null) result.Add(entry);
            }
            return result;
        }

        public async Task<IEnumerable<UniversalisEntry?>> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName)
        {
            var resultList = new List<UniversalisEntry?>();
            foreach (var itemName in itemNames)
            {
                resultList.Add(await _xivContext.UniversalisEntries
                            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10)).Include(c => c.Item)
                            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Name == worldName && i.Item.Name_en == itemName));
            }
            return resultList;
        }



        //TODO rename and use only for large datasets. Smaller datasets should probobly not load the entire database into memory 
        //and can just doublecheck with _xivContext.firstordefault per item
        //TODO refactor to match xivapi's get or create and use states for checking if the query exists or if it is detached
        public async Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList)
        {

            var currentPosts = _xivContext.Posts.ToList();
            var currentEntries = _xivContext.UniversalisEntries.ToList();
            var currentItems = _xivContext.Items.ToList();
            var currentWorlds = _xivContext.Worlds.ToList();
            var returnList = new List<UniversalisEntry>();

            var resultList = new List<UniversalisEntry>();
            foreach (var q in qList)
            {

                var universlisEntry = currentEntries.FirstOrDefault(r =>
                        r.Item.Id == q.Item.Id &&
                        r.World.Id == q.World.Id &&
                        r.LastUploadDate == q.LastUploadDate);
                if (universlisEntry is null)
                {
                    var tempList = new List<MbPost>();
                    foreach (var p in q.Posts)
                    {
                        tempList.Add(currentPosts.FirstOrDefault(r => r.Id == p.Id) ?? p);

                        if (!currentPosts.Contains(p)) { currentPosts.Add(p); }
                    }
                    q.Posts = tempList;
                    q.Item = currentItems.FirstOrDefault(r => r.Id == q.Item.Id) ?? q.Item;
                    q.World = currentWorlds.FirstOrDefault(r => r.Id == q.World.Id) ?? q.World;
                    if (!currentItems.Contains(q.Item)) { currentItems.Add(q.Item); }
                    if (!currentEntries.Contains(q)) { currentEntries.Add(q); }
                    resultList.Add(q);
                }
                else
                {
                    returnList.Add(universlisEntry);
                }
            }
            //Planetscale db errors when too many recipes are saved at once
            for (var i = 0; resultList.Count > i; i += 1000)
            {
                var tempList = resultList.Skip(i).Take(1000).ToList();
                _xivContext.AddRange(tempList);

                await _xivContext.SaveChangesAsync();
                returnList.AddRange(tempList);
            }
            return returnList;


        }
    }
}
