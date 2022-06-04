using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
namespace XIVMarketBoard_Api
{
    public class DbController
    {

        /*public static async Task<List<MbPost>> getMbPostsForItem(int itemId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Posts.Where(r => r.Item.Id == itemId).ToListAsync();
            }
         rewrite to fetch latest universalisquery for item
        }*/



        public static async Task<List<DataCenter>> SaveDataCenters(List<DataCenter> dcList)
        {
            using (var xivContext = new XivDbContext())
            {
                foreach (var dc in dcList)
                {
                    var dcToSave = await xivContext.DataCenters.FirstOrDefaultAsync(r => r.Name == dc.Name);
                    if (dcToSave == null)
                    {
                        xivContext.DataCenters.Add(dc);
                    }
                }
                
                await xivContext.SaveChangesAsync();
                return dcList;
            }
        }
        public static async Task<List<World>> SaveWorlds(List<World> worldList)
        {
            using (var xivContext = new XivDbContext())
            {
                foreach (var world in worldList)
                {
                    var worldToSave = await xivContext.Worlds.FirstOrDefaultAsync(r => r.Id == world.Id);
                    var datacenter = await xivContext.DataCenters.FirstOrDefaultAsync(r => r.Id == world.DataCenter.Id);
                    if (worldToSave == null)
                    {
                        if(datacenter != null)
                        {
                            world.DataCenter = datacenter;
                        }
                        xivContext.Worlds.Add(world);
                    }
                }
                await xivContext.SaveChangesAsync();
                return worldList;
            }
        }
        public static async Task<string> ResetAndSaveRecipiesToDb(List<Recipe> RecipeList)
        {
            using(XivDbContext xivContext = new XivDbContext()) { 
            xivContext.Database.ExecuteSqlRaw(@"SET FOREIGN_KEY_CHECKS = 0; Truncate table Recipes;Truncate table Ingredients;Truncate table Items; SET FOREIGN_KEY_CHECKS = 1");
            List<Ingredient> ingredientList = new List<Ingredient>();
            foreach (var recipe in RecipeList)
            {
                foreach (var ingredient in recipe.Ingredients)
                {
                    ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item, xivContext);
                }
                recipe.Item.CanBeCrafted = true;
                recipe.Item = await GetOrCreateItemFromContext(recipe.Item, xivContext);
                recipe.job = await GetOrCreateJobFromContext(recipe.job, xivContext);
                xivContext.Add(recipe);
            }

            await xivContext.SaveChangesAsync();
            }
            return "successfully saved " + RecipeList.Count + "recipies";

        }
        public static async Task<string> GetOrCreateRecipies(List<Recipe> RecipeList)
        {
            using (XivDbContext xivContext = new XivDbContext())
            {
                foreach (var recipe in RecipeList)
                {
                    var currentRecipe = await xivContext.Recipes.FirstOrDefaultAsync(r => r.Id == recipe.Id);
                    if(currentRecipe == null)
                    {
                        foreach (var ingredient in recipe.Ingredients)
                        {
                            ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item, xivContext);
                        }
                        recipe.Item.CanBeCrafted = true;
                        recipe.Item = await GetOrCreateItemFromContext(recipe.Item, xivContext);
                        recipe.job = await GetOrCreateJobFromContext(recipe.job, xivContext);
                        xivContext.Add(recipe);
                    }
                    else
                    {
                        //add check for ingredients here later
                    }

                    
                }

                await xivContext.SaveChangesAsync();
            }
            return "successfully saved " + RecipeList.Count + "recipies";

        }
        private static async Task<Item> GetOrCreateItemFromContext(Item item, XivDbContext dbContext)
        {

            var tempItem = await dbContext.Items.FindAsync(item.Id);
            if (tempItem == null)
            {
                dbContext.Add(item);
                await dbContext.SaveChangesAsync();
                return item;
            }

            return tempItem;
        }

        private static async Task<Job> GetOrCreateJobFromContext(Job job, XivDbContext dbContext)
        {
            var tempJob = await dbContext.Jobs.FindAsync(job.Id);
            if (tempJob == null)
            {
                dbContext.Add(job);
                await dbContext.SaveChangesAsync();
                return job;
            }

            return tempJob;
        }


        #region universalis
        public static async Task<UniversalisEntry> GetOrCreateUniversalisQuery(UniversalisEntry q)
        {
            using (var xivContext = new XivDbContext())
            {
                var universalisQuery = await xivContext.UniversalisEntries
                    .FirstOrDefaultAsync(r => (
                        r.Item.Id == q.Item.Id &&
                        r.World.Id == q.World.Id &&
                        r.LastUploadDate == q.LastUploadDate));
                if (universalisQuery == null)
                {
                    List<MbPost> mbPostList = new List<MbPost>();
                    foreach(MbPost mbPost in q.Posts)
                    {
                        mbPostList.Add(await GetOrCreateMbPost(mbPost, xivContext));
                    }
                    q.Posts = mbPostList;
                    xivContext.UniversalisEntries.Add(q);
                    await xivContext.SaveChangesAsync();
                    return q; 
                }
                return universalisQuery;
            }

        }
        public static async Task<MbPost> GetOrCreateMbPost(MbPost tempMbPost, XivDbContext xivContext)
        {

                var mbPost = await xivContext.Posts.FirstOrDefaultAsync(r => r.Id == tempMbPost.Id);
                if(mbPost == null)
                {
                    xivContext.Add(tempMbPost);
                    await xivContext.SaveChangesAsync();
                    return tempMbPost;
                }
                return mbPost;
            
            
        }
        public static async Task<UniversalisEntry> GetLatestUniversalisQueryForItem(int itemId, int worldId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.UniversalisEntries
                    .Include(a => a.Posts.Take(10)).Include(b => b.SaleHistory.Take(10))
                    .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync();
            }
        }
        #endregion


        #region getFromContext
        public static World GetWorldFromName(string worldName)
        {
            using (var xivContext = new XivDbContext())
            {
                return xivContext.Worlds.FirstOrDefault(r => r.Name == worldName);
            }
        }
        public static async Task<Recipe> GetRecipeFromName(string recipeName)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Recipes.FirstOrDefaultAsync(r => r.Name == recipeName);
            }

        }
        public static async Task<Item> GetItemFromNameAsync(string itemName)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Items.FirstOrDefaultAsync(r => r.Name == itemName);
            }

        }
        public static Item GetItemFromId(int itemId)
        {
            using (var xivContext = new XivDbContext())
            {
                return xivContext.Items.FirstOrDefault(r => r.Id == itemId);
            }

        }

        /*public static async Task<MbPost> getMbPostFromItemId(int itemId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Posts.FirstOrDefaultAsync(r => r.Item.Id == itemId);
            }
           TODO: Rewrite to fetch universalisquery 
        }*/
        public static async Task<List<Recipe>> GetRecipiesById(List<int> recipeId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Recipes.Where(r => recipeId.Contains(r.Id)).ToListAsync();

            }
        }
        public static async Task<List<Recipe>> GetAllRecipies()
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Set<Recipe>().ToListAsync();
            }
        }
        public static async Task<List<Item>> GetAllItems()
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Set<Item>().ToListAsync();
            }

        }
        #endregion
    }
}
