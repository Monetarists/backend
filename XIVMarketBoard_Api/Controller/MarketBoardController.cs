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
        Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList);
        IAsyncEnumerable<UniversalisEntry?> GetLatestUniversalisQueryForItems(IEnumerable<string> itemNames, string worldName);
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(string itemName, string worldName);
    }

    public class MarketBoardController : IMarketBoardController
    {
        private readonly XivDbContext _xivContext;
        public MarketBoardController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }
        #region getFromContext


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

        /*public static async Task<MbPost> getMbPostFromItemId(int itemId)
{

        return await _xivContext.Posts.FirstOrDefaultAsync(r => r.Item.Id == itemId);

   TODO: Rewrite to fetch universalisquery 
}*/

        #endregion


        /*public static async Task<List<MbPost>> getMbPostsForItem(int itemId)
        {

                return await _xivContext.Posts.Where(r => r.Item.Id == itemId).ToListAsync();

         rewrite to fetch latest universalisquery for item
        }*/







        #region universalis
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
                    /* obsolete due to bug in universalis api
                    List<MbPost> mbPostList = new List<MbPost>();
                    foreach (MbPost mbPost in q.Posts)
                    {
                        mbPostList.Add(await CreateMbPost(mbPost, _xivContext));
                    }
                    q.Posts = mbPostList;*/
                    _xivContext.UniversalisEntries.Add(q);
                }
            }
            await _xivContext.SaveChangesAsync();
            return qList;


        }

        //obsolete due to bug in universalis api
        public async Task<MbPost> GetOrCreateMbPost(MbPost tempMbPost, XivDbContext _xivContext)
        {

            var mbPost = await _xivContext.Posts.FirstOrDefaultAsync(r => r.Id == tempMbPost.Id);
            if (mbPost == null)
            {
                _xivContext.Add(tempMbPost);
                await _xivContext.SaveChangesAsync();
                return tempMbPost;
            }
            return mbPost;


        }



        #endregion



    }
}
