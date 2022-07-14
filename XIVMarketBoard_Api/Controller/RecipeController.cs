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
        Task<string> SetCraftableItemsFromRecipes();
        Task<string> UpdateItems(List<Item> iList);
        Task<ICollection<Recipe>> GetAllRecipesList();
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
        public async Task<ICollection<Recipe>> GetAllRecipesList() => await _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.job).Where(r => r.Id != null).ToListAsync();
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

        //TODO rename and use only for large datasets. Smaller datasets should probobly not load the entire database into memory 
        //and can just doublecheck with _xivContext.firstordefault per item
        public async Task<string> GetOrCreateRecipes(IEnumerable<Recipe> RecipeList)
        {
            //TODO call for a list of marketable items and insert everything correctly from the start
            //TODO Clean up the code and make it more readable look at universalis controller // bryt ut till createrecipe och kör firstordefault() ?? createRecipe()
            var currentItems = await _xivContext.Items.ToListAsync();
            var currentRecipes = await _xivContext.Recipes.ToListAsync();
            var currentJobs = await _xivContext.Jobs.ToListAsync();
            var recipeList = new List<Recipe>();

            foreach (var recipe in RecipeList)
            {
                var currentRecipe = currentRecipes.FirstOrDefault(r => r.Id == recipe.Id);
                if (currentRecipe == null)
                {
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        ingredient.Item = currentItems.FirstOrDefault(i => i.Id == ingredient.Item.Id) ?? ingredient.Item;
                        ingredient.Item.IsMarketable = ingredient.Item.IsMarketable ?? false;
                        if (!currentItems.Contains(ingredient.Item)) { currentItems.Add(ingredient.Item); }
                        if (ingredient.Item.CanBeCrafted == null)
                        {
                            ingredient.Item.CanBeCrafted = false;
                        }
                    }

                    recipe.Item = currentItems.FirstOrDefault(i => i.Id == recipe.Item.Id) ?? recipe.Item;
                    recipe.Item.IsMarketable = recipe.Item.IsMarketable ?? false;

                    if (recipe.Item.CanBeCrafted != true)
                    {
                        recipe.Item.CanBeCrafted = true;
                    }
                    recipe.job = currentJobs.FirstOrDefault(j => j.Id == recipe.job.Id) ?? recipe.job;
                    if (!currentItems.Contains(recipe.Item)) { currentItems.Add(recipe.Item); }
                    if (!currentJobs.Contains(recipe.job)) { currentJobs.Add(recipe.job); }

                    recipeList.Add(recipe);
                }
            }

            //Planetscale db errors when too many recipes are saved at once
            for (var i = 0; recipeList.Count > i; i += 100)
            {
                var tempList = recipeList.Skip(i).Take(100).ToList();
                _xivContext.AddRange(tempList);
                await _xivContext.SaveChangesAsync();
            }
            return "successfully saved " + RecipeList.Count() + "recipes";

        }
    }
}
