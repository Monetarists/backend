using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Helpers;
using Newtonsoft.Json;
using XIVMarketBoard_Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Reactive.Linq;



namespace XIVMarketBoard_Api.Controller
{
    public interface IMarketBoardController
    {
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId);
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName);

        Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> entryList);
        Task<IEnumerable<UniversalisEntry?>> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName);
        Task<IEnumerable<UniversalisEntry>> GetLatestUniversalisQueryForItems(List<Item> itemList, World world);
    }

    public class MarketBoardController : IMarketBoardController
    {
        private List<MbPost> currentPosts = new List<MbPost>();
        private List<UniversalisEntry> currentEntries = new List<UniversalisEntry>();
        private List<Item> currentItems = new List<Item>();
        private List<World> currentWorlds = new List<World>();
        private readonly XivDbContext _xivContext;
        public MarketBoardController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }


        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10)).Include(c => c.Item)
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Id == worldId && i.Item.Id == itemId);
        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts).Include(b => b.SaleHistory).Include(c => c.Item)
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Name == worldName && i.Item.Name_en == itemName);

        public async Task<IEnumerable<UniversalisEntry>> GetLatestUniversalisQueryForItems(List<Item> itemList, World world)
        {
            var results = from itemId in _xivContext.UniversalisEntries.Where(i => itemList.Contains(i.Item)).Select(x => x.Item.Id).Distinct()
                          from universalisEntry in _xivContext.UniversalisEntries
                          .Where(x => x.Item.Id == itemId && world.Id == x.World.Id)
                          .Include(x => x.Item)
                          .OrderByDescending(e => e.QueryDate)
                          .Take(1)
                          select universalisEntry;
            return await results.ToListAsync();
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


        public async Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> entryList)
        {

            currentPosts = _xivContext.Posts.ToList();
            currentEntries = _xivContext.UniversalisEntries.ToList();
            currentItems = _xivContext.Items.ToList();
            currentWorlds = _xivContext.Worlds.ToList();
            var returnList = new List<UniversalisEntry>();

            var resultList = new List<UniversalisEntry>();

            foreach (var entry in entryList)
            {
                if (entry.Item is null)
                {
                    throw new AppException("Item Id is null when creating universalis query " + JsonConvert.SerializeObject(entry));
                }
                var universlisEntry = currentEntries.FirstOrDefault(r =>
                        r.Item.Id == entry.Item.Id &&
                        r.World.Id == entry.World.Id &&
                        r.LastUploadDate == entry.LastUploadDate);
                if (universlisEntry is null)
                {
                    resultList.Add(SetUniversalisVariables(entry));
                    continue;
                }
                returnList.Add(universlisEntry);

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
        public UniversalisEntry SetUniversalisVariables(UniversalisEntry entry)
        {
            var tempList = new List<MbPost>();
            foreach (var p in entry.Posts)
            {
                tempList.Add(currentPosts.FirstOrDefault(r => r.Id == p.Id) ?? p);

                if (!currentPosts.Contains(p)) { currentPosts.Add(p); }
            }
            entry.Posts = tempList;
            entry.Item = currentItems.FirstOrDefault(r => r.Id == entry.Item.Id) ?? entry.Item;
            entry.World = currentWorlds.FirstOrDefault(r => r.Id == entry.World.Id) ?? entry.World;
            if (!currentItems.Contains(entry.Item)) { currentItems.Add(entry.Item); }
            if (!currentEntries.Contains(entry)) { currentEntries.Add(entry); }
            return entry;

        }
    }
}
