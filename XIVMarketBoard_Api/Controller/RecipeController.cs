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

    public interface IRecipeController
    {
        Task<Dictionary<int, string>> GetAllItemNames();
        IAsyncEnumerable<Item> GetAllItems();
        Task<Dictionary<int, string>> GetAllRecipeNames();
        IAsyncEnumerable<Recipe> GetAllRecipes();
        IAsyncEnumerable<Recipe> GetAllRecipesIncludeItem();
        Task<Item?> GetItemFromId(int itemId);
        Task<Item?> GetItemFromNameAsync(string itemName);
        IAsyncEnumerable<Item> GetItemsByIds(List<int> itemIds);
        Task<string> GetOrCreateRecipes(IEnumerable<Recipe> RecipeList);
        Task<Recipe?> GetRecipeFromName(string recipeName);
        Task<Recipe?> GetRecipeFromNameIncludeIngredients(string recipeName);
        Task<Recipe?> GetRecipeFromNameIncludeItem(string recipeName);
        IAsyncEnumerable<Recipe> GetRecipesByIds(List<int> recipeId);
        IAsyncEnumerable<Recipe> GetRecipesByItemIds(List<int> itemIds);
        IAsyncEnumerable<Recipe> GetRecipesFromNameCollIncludeIngredients(IEnumerable<string> recipeNames);
        Task<string> ResetAndSaveRecipesToDb(IEnumerable<Recipe> RecipeList);
        Task<string> SetCraftableItemsFromRecipes();
        Task<string> UpdateItems(List<Item> iList);
    }
    public class RecipeController : IRecipeController
    {
        private readonly XivDbContext _xivContext;
        public RecipeController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }

        public async Task<Recipe?> GetRecipeFromName(string recipeName) => await _xivContext.Recipes.FirstOrDefaultAsync(r => r.Name == recipeName);
        public async Task<Recipe?> GetRecipeFromNameIncludeIngredients(string recipeName) => await _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).FirstOrDefaultAsync(r => r.Name == recipeName);
        public IAsyncEnumerable<Recipe> GetRecipesByIds(List<int> recipeId) => _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.job).Where(r => recipeId.Contains(r.Id)).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetRecipesByItemIds(List<int> itemIds) => _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.job).Where(r => itemIds.Contains(r.Item.Id)).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetAllRecipes() => _xivContext.Set<Recipe>().Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.job).AsAsyncEnumerable();
        public async Task<Dictionary<int, string>> GetAllRecipeNames() => await _xivContext.Recipes.Select(recipe => new KeyValuePair<int, string>(recipe.Id, recipe.Name)).ToDictionaryAsync(r => r.Key, r => r.Value);
        public IAsyncEnumerable<Recipe> GetAllRecipesIncludeItem() => _xivContext.Set<Recipe>().Include(i => i.Item).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetRecipesFromNameCollIncludeIngredients(IEnumerable<string> recipeNames) => _xivContext.Recipes.Include(i => i.Ingredients).ThenInclude(p => p.Item).Where(r => recipeNames.Contains(r.Name)).ToAsyncEnumerable();
        public async Task<Recipe?> GetRecipeFromNameIncludeItem(string recipeName) => await _xivContext.Recipes.Include(i => i.Item).FirstOrDefaultAsync(r => recipeName.Contains(r.Name));
        public IAsyncEnumerable<Item> GetItemsByIds(List<int> itemIds) => _xivContext.Items.Where(r => itemIds.Contains(r.Id)).AsAsyncEnumerable();
        public async Task<Item?> GetItemFromId(int itemId) => await _xivContext.Items.FirstOrDefaultAsync(p => p.Id == itemId);
        public IAsyncEnumerable<Item> GetAllItems() => _xivContext.Set<Item>().AsAsyncEnumerable();
        public async Task<Dictionary<int, string>> GetAllItemNames() => await _xivContext.Items.Select(item => new KeyValuePair<int, string>(item.Id, item.Name)).ToDictionaryAsync(r => r.Key, r => r.Value);

        public async Task<Item?> GetItemFromNameAsync(string itemName) => await _xivContext.Items.FirstOrDefaultAsync(r => r.Name == itemName);
        public async Task<string> UpdateItems(List<Item> iList)
        {
            try
            {
                //_xivContext.Update(iList);
                await _xivContext.SaveChangesAsync();
                return "Success";
            }
            catch (Exception e) { return e.Message; }


        }
        public async Task<string> SetCraftableItemsFromRecipes()
        {

            try
            {
                var recipeColl = GetAllRecipesIncludeItem();
                //_xivContext.UpdateRange(recipeColl);
                await foreach (var r in recipeColl)
                {
                    r.Item.CanBeCrafted = true;
                    //todo crashes Cannot access a disposed object.
                }
                await _xivContext.SaveChangesAsync();
                return "updated recipes";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        public async Task<string> ResetAndSaveRecipesToDb(IEnumerable<Recipe> RecipeList)
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

            return "successfully saved " + RecipeList.Count() + "recipes";

        }

        public async Task<string> GetOrCreateRecipes(IEnumerable<Recipe> RecipeList)
        {

            foreach (var recipe in RecipeList)
            {
                var currentRecipe = await _xivContext.Recipes.FirstOrDefaultAsync(r => r.Id == recipe.Id);
                if (currentRecipe == null)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Item = await GetOrCreateItemFromContext(ingredient.Item);
                        if (ingredient.Item.CanBeCrafted == null)
                        {
                            ingredient.Item.CanBeCrafted = false;
                        }
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

            return "successfully saved " + RecipeList.Count() + "recipes";

        }
        private async Task<Item> GetOrCreateItemFromContext(Item item)
        {

            var tempItem = await _xivContext.Items.FindAsync(item.Id);
            if (tempItem == null)
            {
                item.IsMarketable = false;
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
    }
}
