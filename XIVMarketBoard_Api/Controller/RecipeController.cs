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
        Task<IEnumerable<Item>> GetAllItems();
        Task<Dictionary<int, string>> GetAllRecipeNames();
        IAsyncEnumerable<Recipe> GetAllRecipes();
        IAsyncEnumerable<Recipe> GetAllRecipesIncludeItem();
        Task<Item?> GetItemFromId(int itemId);
        Task<Item?> GetItemFromNameAsync(string itemName);
        Task<List<Item>> GetItemFromNameList(IEnumerable<string> itemNames);
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
        Task<IEnumerable<Recipe>> GetAllRecipesList();
        IAsyncEnumerable<Recipe> GetMarketableRecipesByJob(string jobAbrv);
    }
    public class RecipeController : IRecipeController
    {
        private List<Item> currentItems = new List<Item>();
        private List<Job> currentJobs = new List<Job>();
        private List<ItemSearchCategory> currentItemScs = new List<ItemSearchCategory>();
        private List<ItemUICategory> currentItemUcs = new List<ItemUICategory>();
        private readonly XivDbContext _xivContext;
        public RecipeController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }

        public async Task<Recipe?> GetRecipeFromName(string recipeName) => await _xivContext.Recipes.FirstOrDefaultAsync(r => r.Name_en == recipeName);
        public async Task<Recipe?> GetRecipeFromNameIncludeIngredients(string recipeName) => await _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).FirstOrDefaultAsync(r => r.Name_en == recipeName);
        public IAsyncEnumerable<Recipe> GetRecipesByIds(List<int> recipeId) => _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item.ItemSearchCategory).Include(r => r.Item.ItemUICategory).Include(r => r.Job).Where(r => recipeId.Contains(r.Id)).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetMarketableRecipesByJob(string jobAbrv) => _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.Item.ItemSearchCategory).Include(r => r.Item.ItemUICategory).Where(r => r.Job.Abbreviation == jobAbrv && r.Item.IsMarketable == true).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetRecipesByItemIds(List<int> itemIds) => _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.Job).Where(r => itemIds.Contains(r.Item.Id)).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetAllRecipes() => _xivContext.Set<Recipe>().Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.Job).AsAsyncEnumerable();
        public async Task<IEnumerable<Recipe>> GetAllRecipesList() => await _xivContext.Recipes.Include(r => r.Ingredients).ThenInclude(p => p.Item).Include(r => r.Item).Include(r => r.Job).ToListAsync();
        public async Task<Dictionary<int, string>> GetAllRecipeNames() => await _xivContext.Recipes.Select(recipe => new KeyValuePair<int, string>(recipe.Id, recipe.Name_en)).ToDictionaryAsync(r => r.Key, r => r.Value);
        public IAsyncEnumerable<Recipe> GetAllRecipesIncludeItem() => _xivContext.Set<Recipe>().Include(i => i.Item).AsAsyncEnumerable();
        public IAsyncEnumerable<Recipe> GetRecipesFromNameCollIncludeIngredients(IEnumerable<string> recipeNames) => _xivContext.Recipes.Include(i => i.Ingredients).ThenInclude(p => p.Item).Where(r => recipeNames.Contains(r.Name_en)).ToAsyncEnumerable();
        public async Task<Recipe?> GetRecipeFromNameIncludeItem(string recipeName) => await _xivContext.Recipes.Include(i => i.Item).FirstOrDefaultAsync(r => recipeName.Contains(r.Name_en));
        public IAsyncEnumerable<Item> GetItemsByIds(List<int> itemIds) => _xivContext.Items.Where(r => itemIds.Contains(r.Id)).AsAsyncEnumerable();
        public async Task<Item?> GetItemFromId(int itemId) => await _xivContext.Items.FirstOrDefaultAsync(p => p.Id == itemId);
        public async Task<IEnumerable<Item>> GetAllItems() => await _xivContext.Set<Item>().ToListAsync();
        public async Task<Dictionary<int, string>> GetAllItemNames() => await _xivContext.Items.Select(item => new KeyValuePair<int, string>(item.Id, item.Name_en)).ToDictionaryAsync(r => r.Key, r => r.Value);

        public async Task<Item?> GetItemFromNameAsync(string itemName) => await _xivContext.Items.FirstOrDefaultAsync(r => r.Name_en == itemName);
        public async Task<List<Item>> GetItemFromNameList(IEnumerable<string> itemNames) => await _xivContext.Items.Where(r => itemNames.Contains(r.Name_en)).ToListAsync();

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
            currentItems = await _xivContext.Items.ToListAsync();
            currentJobs = await _xivContext.Jobs.ToListAsync();
            currentItemScs = await _xivContext.ItemSearchCategory.ToListAsync();
            currentItemUcs = await _xivContext.ItemUICategory.ToListAsync();
            var currentRecipes = await _xivContext.Recipes.Include(x => x.Ingredients).ThenInclude(i => i.Item).ToListAsync();

            var recipesToUpsert = new List<Recipe>();

            foreach (var recipe in RecipeList)
            {
                var currentRecipe = currentRecipes.FirstOrDefault(r => r.Id == recipe.Id);
                var tempRecipe = currentRecipe ?? setRecipeVariables(recipe);

                tempRecipe.Ingredients = setIngredientList(recipe.Ingredients, currentRecipe);
                tempRecipe.Item = setItemVariables(recipe.Item);
                if (tempRecipe.Item.CanBeCrafted != true) { tempRecipe.Item.CanBeCrafted = true; }
                recipesToUpsert.Add(tempRecipe);

            }

            //batch saving of recipes to 1000 to not bog down the database
            for (var i = 0; recipesToUpsert.Count > i; i += 1000)
            {
                var tempList = recipesToUpsert.Skip(i).Take(1000).ToList();
                _xivContext.AddRange(tempList.Where(x => _xivContext.Entry(x).State == EntityState.Detached).ToList());
                await _xivContext.SaveChangesAsync();
            }
            return "successfully saved " + RecipeList.Count() + "recipes";

        }
        private ICollection<Ingredient> setIngredientList(ICollection<Ingredient> inputList, Recipe? currentRecipe)
        {

            if (currentRecipe != null && currentRecipe.Ingredients.Count != 0)
            {
                return currentRecipe.Ingredients;
            }
            var returnList = new List<Ingredient>();
            foreach (var ingredient in inputList)
            {
                ingredient.Item = setItemVariables(ingredient.Item);
                returnList.Add(ingredient);

            }
            return returnList;
        }
        private Item setItemVariables(Item item)
        {
            var tempItem = currentItems.FirstOrDefault(i => i.Id == item.Id) ?? item;

            if (!tempItem.CanBeCrafted.HasValue) { tempItem.CanBeCrafted = false; }
            tempItem.IsMarketable = item.IsMarketable ?? false;
            tempItem.ItemUICategory = currentItemUcs.FirstOrDefault(i => i.Id == item.ItemUICategory.Id) ?? item.ItemUICategory;
            if (item.ItemSearchCategory != null && item.ItemSearchCategory.Id != 0)
            {
                tempItem.ItemSearchCategory = currentItemScs.FirstOrDefault(i => i.Id == item.ItemSearchCategory.Id) ?? item.ItemSearchCategory;
            }
            else
            {
                item.ItemSearchCategory = null;
            }

            if (tempItem.ItemSearchCategory != null && tempItem.ItemSearchCategory.Id != 0 && !currentItemScs.Select(x => x.Id).ToList().Contains(tempItem.ItemSearchCategory.Id)) { currentItemScs.Add(tempItem.ItemSearchCategory); }
            if (!currentItemUcs.Select(x => x.Id).ToList().Contains(tempItem.ItemUICategory.Id) && tempItem.ItemUICategory.Id != 0) { currentItemUcs.Add(tempItem.ItemUICategory); }
            if (!currentItems.Contains(tempItem)) { currentItems.Add(tempItem); }
            return tempItem;
        }

        private Recipe setRecipeVariables(Recipe recipe)
        {
            var tempRecipe = recipe;
            tempRecipe.Job = currentJobs.FirstOrDefault(j => j.Id == recipe.Job.Id) ?? recipe.Job;

            if (tempRecipe.Item.ItemSearchCategory != null && tempRecipe.Item.ItemSearchCategory.Id != 0 && !currentItemScs.Select(x => x.Id).ToList().Contains(tempRecipe.Item.ItemSearchCategory.Id)) { currentItemScs.Add(tempRecipe.Item.ItemSearchCategory); }
            if (!currentItemUcs.Select(x => x.Id).ToList().Contains(tempRecipe.Item.ItemUICategory.Id) && tempRecipe.Item.ItemUICategory.Id != 0) { currentItemUcs.Add(tempRecipe.Item.ItemUICategory); }

            if (!currentJobs.Contains(tempRecipe.Job)) { currentJobs.Add(tempRecipe.Job); }
            return tempRecipe;
        }


    }
}
