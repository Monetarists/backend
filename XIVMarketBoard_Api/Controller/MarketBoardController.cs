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
        IAsyncEnumerable<UniversalisEntry?> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName);
    }

    public class MarketBoardController : IMarketBoardController
    {
        private readonly XivDbContext _xivContext;
        public MarketBoardController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }


        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10))
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Id == worldId && i.Item.Id == itemId);
        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName) => await _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10))
            .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync(i => i.World.Name == worldName && i.Item.Name == itemName);
        public IAsyncEnumerable<UniversalisEntry?> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName) => _xivContext.UniversalisEntries
            .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10))
            .OrderByDescending(p => p.QueryDate)
            .Where(i => i.World.Name == worldName && itemNames.Contains(i.Item.Name)).AsAsyncEnumerable();


        public async Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList)
        {
            foreach (var q in qList)
            {
                var universlisEntry = await _xivContext.UniversalisEntries
                    .FirstOrDefaultAsync(r =>
                        r.Item.Id == q.Item.Id &&
                        r.World.Id == q.World.Id &&
                        r.LastUploadDate == q.LastUploadDate);
                if (universlisEntry == null)
                {
                    q.Item = await _xivContext.Items.FindAsync(q.Item.Id) ?? new();
                    q.World = await _xivContext.Worlds.FindAsync(q.World.Id) ?? new();
                    _xivContext.UniversalisEntries.Add(q);
                }
            }
            await _xivContext.SaveChangesAsync();
            return qList;


        }
    }
}
