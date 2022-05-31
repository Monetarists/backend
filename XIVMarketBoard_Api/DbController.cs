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

        public static async Task<Recipe> getRecipeFromName(string recipeName)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Recipes.FirstOrDefaultAsync(r => r.Name == recipeName);
            }
            
        }
        public static async Task<Item> getItemFromName(String itemName)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Items.FirstOrDefaultAsync(r => r.Name == itemName);
            }            

        }

        public static async Task<MbPost> getMbPostFromItemId(int itemId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.MbPosts.FirstOrDefaultAsync(r => r.Item.Id == itemId);
            }
            
        }
        public static async Task<List<Recipe>> getRecipiesById(List<int> recipeId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Recipes.Where(r => recipeId.Contains(r.Id)).ToListAsync();
                
            }
        }
        public static async Task<List<Recipe>> getAllRecipies()
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Set<Recipe>().ToListAsync();
            }
        }        
        public static async Task<List<Item>> getAllItems()
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.Set<Item>().ToListAsync();
            }

        }

        public static async Task<List<MbPost>> getMbPostsForItem(int itemId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.MbPosts.Where(r => r.Item.Id == itemId).ToListAsync();
            }

        }

        public static async Task<UniversalisQuery> getLatestUniversalisQueryForItem(int itemId, int worldId)
        {
            using (var xivContext = new XivDbContext())
            {
                return await xivContext.UniversalisQueries
                    .Include(a => a.posts.Take(10)).Include(b => b.postHistory.Take(10))
                    .OrderByDescending(p => p.QueryDate).FirstOrDefaultAsync();
            }
        }

        public static async Task<List<DataCenter>> saveDataCenters(List<DataCenter> dcList)
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
        public static async Task<List<World>> saveWorlds(List<World> worldList)
        {
            using (var xivContext = new XivDbContext())
            {
                foreach (var world in worldList)
                {
                    var worldToSave = await xivContext.Servers.FirstOrDefaultAsync(r => r.Id == world.Id);
                    if (worldToSave == null)
                    {
                        xivContext.Servers.Add(world);
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
        public static async Task<string> SaveRecipiesToDb(List<Recipe> RecipeList)
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
    }
}
