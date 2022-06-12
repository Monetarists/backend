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
    public interface IDbController
    {
        IAsyncEnumerable<Item> GetAllItems();
        IAsyncEnumerable<Recipe> GetAllRecipies();
        Item? GetItemFromId(int itemId);
        Task<Item?> GetItemFromNameAsync(string itemName);
        Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId);
        Task<MbPost> GetOrCreateMbPost(MbPost tempMbPost, XivDbContext _xivContext);
        Task<string> GetOrCreateRecipies(IEnumerable<Recipe> RecipeList);
        Task<UniversalisEntry> GetOrCreateUniversalisQuery(UniversalisEntry q);
        Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList);
        Task<Recipe?> GetRecipeFromName(string recipeName);
        IAsyncEnumerable<Recipe> GetRecipiesByIds(List<int> recipeId);
        World? GetWorldFromName(string worldName);
        Task<string> ResetAndSaveRecipiesToDb(IEnumerable<Recipe> RecipeList);
        Task<IEnumerable<DataCenter>> SaveDataCenters(IEnumerable<DataCenter> dcList);
        Task<IEnumerable<World>> SaveWorlds(IEnumerable<World> worldList);
        IAsyncEnumerable<Recipe> GetAllRecipiesIncludeItems();
        Task<string> SetCraftableItemsFromRecipes();
        IAsyncEnumerable<Item> GetItemsByIds(List<int> itemIds);
        Task<string> UpdateItems(List<Item> iList);
    }

    public class DbController : IDbController
    {
        private readonly XivDbContext _xivContext;
        public DbController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }

        public async Task<string> UpdateItems(List<Item> iList)
        {
            try { 
                //_xivContext.Update(iList);
                await _xivContext.SaveChangesAsync();
                return "Success";
            }
            catch (Exception e) { return e.Message; }


        }

            /*public static async Task<List<MbPost>> getMbPostsForItem(int itemId)
            {

                    return await _xivContext.Posts.Where(r => r.Item.Id == itemId).ToListAsync();

             rewrite to fetch latest universalisquery for item
            }*/
            public async Task<string> SetCraftableItemsFromRecipes()
        {

            try
            {
                var recipeColl = GetAllRecipiesIncludeItems();
                //_xivContext.UpdateRange(recipeColl);
                await foreach (var r in recipeColl)
                {
                    r.Item.CanBeCrafted = true;
                    //todo crashes Cannot access a disposed object.
                }
                //var updatedRecipies = await recipies.ToListAsync();
                
                
                
                //await recipeColl.ForEachAsync(i => i.Item.CanBeCrafted = true);
                //_xivContext.Update(recipies);
                await _xivContext.SaveChangesAsync();
                return "updated recipies";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        
        }

        public async Task<IEnumerable<DataCenter>> SaveDataCenters(IEnumerable<DataCenter> dcList)
        {

            foreach (var dc in dcList)
            {
                var dcToSave = await _xivContext.DataCenters.FirstOrDefaultAsync(r => r.Name == dc.Name);
                if (dcToSave == null)
                {
                    _xivContext.DataCenters.Add(dc);
                }
            }

            await _xivContext.SaveChangesAsync();
            return dcList;

        }
        public async Task<IEnumerable<World>> SaveWorlds(IEnumerable<World> worldList)
        {

            foreach (var world in worldList)
            {
                var worldToSave = await _xivContext.Worlds.FirstOrDefaultAsync(r => r.Id == world.Id);
                var datacenter = await _xivContext.DataCenters.FirstOrDefaultAsync(r => r.Id == world.DataCenter.Id);
                if (worldToSave == null)
                {
                    if (datacenter != null)
                    {
                        world.DataCenter = datacenter;
                    }
                    _xivContext.Worlds.Add(world);
                }
            }
            await _xivContext.SaveChangesAsync();
            return worldList;

        }
        public async Task<string> ResetAndSaveRecipiesToDb(IEnumerable<Recipe> RecipeList)
        {

            _xivContext.Database.ExecuteSqlRaw(@"SET FOREIGN_KEY_CHECKS = 0; Truncate table Recipes;Truncate table Ingredients;Truncate table Items; SET FOREIGN_KEY_CHECKS = 1");
            List<Ingredient> ingredientList = new List<Ingredient>();
            foreach (var recipe in RecipeList)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item);
                }

                recipe.Item = await GetOrCreateItemFromContext(recipe.Item);
                recipe.Item.CanBeCrafted = true;
                recipe.job = await GetOrCreateJobFromContext(recipe.job);
                _xivContext.Add(recipe);
            }

            await _xivContext.SaveChangesAsync();

            return "successfully saved " + RecipeList.Count() + "recipies";

        }
        
        public async Task<string> GetOrCreateRecipies(IEnumerable<Recipe> RecipeList)
        {

            foreach (var recipe in RecipeList)
            {
                var currentRecipe = await _xivContext.Recipes.FirstOrDefaultAsync(r => r.Id == recipe.Id);
                if (currentRecipe == null)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item);
                    }
                    
                    recipe.Item = await GetOrCreateItemFromContext(recipe.Item);
                    if (recipe.Item.CanBeCrafted != true)
                    {
                        recipe.Item.CanBeCrafted = true;
                    }
                    recipe.job = await GetOrCreateJobFromContext(recipe.job);
                    _xivContext.Add(recipe);
                }
                else
                {
                    //add check for ingredients here later
                }


            }

            await _xivContext.SaveChangesAsync();

            return "successfully saved " + RecipeList.Count() + "recipies";

        }
        private async Task<Item> GetOrCreateItemFromContext(Item item)
        {

            var tempItem = await _xivContext.Items.FindAsync(item.Id);
            if (tempItem == null)
            {
                _xivContext.Add(item);
                await _xivContext.SaveChangesAsync();
                return item;
            }

            return tempItem;
        }

        private async Task<Job> GetOrCreateJobFromContext(Job job)
        {
            var tempJob = await _xivContext.Jobs.FindAsync(job.Id);
            if (tempJob == null)
            {
                _xivContext.Add(job);
                await _xivContext.SaveChangesAsync();
                return job;
            }

            return tempJob;
        }


        #region universalis
        public async Task<UniversalisEntry> GetOrCreateUniversalisQuery(UniversalisEntry q)
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
                    mbPostList.Add(await GetOrCreateMbPost(mbPost, _xivContext));
                }
                q.Posts = mbPostList;
                */
                _xivContext.UniversalisEntries.Add(q);
                await _xivContext.SaveChangesAsync();
                return q;
            }
            return universlisEntry;


        }
        public async Task<IEnumerable<UniversalisEntry>> GetOrCreateUniversalisQueries(List<UniversalisEntry> qList)
        {
            foreach (var q in qList) { 
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
        public async Task<UniversalisEntry?> GetLatestUniversalisQueryForItem(int itemId, int worldId) => await _xivContext.UniversalisEntries
                    .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10))
                    .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync();


        #endregion


        #region getFromContext
        public World? GetWorldFromName(string worldName) => _xivContext.Worlds.FirstOrDefault(r => r.Name == worldName);


        public async Task<Recipe?> GetRecipeFromName(string recipeName) => await _xivContext.Recipes.FirstOrDefaultAsync(r => r.Name == recipeName);



        public async Task<Item?> GetItemFromNameAsync(string itemName) => await _xivContext.Items.FirstOrDefaultAsync(r => r.Name == itemName);



        public Item? GetItemFromId(int itemId) => _xivContext.Items.Find(itemId);

        /*public static async Task<MbPost> getMbPostFromItemId(int itemId)
        {

                return await _xivContext.Posts.FirstOrDefaultAsync(r => r.Item.Id == itemId);
            
           TODO: Rewrite to fetch universalisquery 
        }*/
        
        public IAsyncEnumerable<Recipe> GetRecipiesByIds(List<int> recipeId) => _xivContext.Recipes.Where(r => recipeId.Contains(r.Id)).AsAsyncEnumerable();


        public IAsyncEnumerable<Recipe> GetAllRecipies() => _xivContext.Set<Recipe>().AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetAllRecipiesIncludeItems() => _xivContext.Set<Recipe>().Include(i => i.Item).AsAsyncEnumerable();
        public IAsyncEnumerable<Item> GetItemsByIds(List<int> itemIds) => _xivContext.Items.Where(r => itemIds.Contains(r.Id)).AsAsyncEnumerable();

        public IAsyncEnumerable<Item> GetAllItems() => _xivContext.Set<Item>().AsAsyncEnumerable();

        

        #endregion
    }
}
