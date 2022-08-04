using XIVMarketBoard_Api.Controller;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api;
using XIVMarketBoard_Api.Tools;
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models.Users;

using Microsoft.AspNetCore.Authorization;
using XIVMarketBoard_Api.Repositories.Models.ResponseDto;
using System.Linq;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
var services = Builder.ConfigureServices(builder);

var app = Builder.ConfigureWebApp(builder);

app.MapGet("/Authenticate", (IUserController userController, string username, string password) =>
{
    AuthenticateRequest req = new AuthenticateRequest() { UserName = username, Password = password };
    var response = userController.Authenticate(req);

    if (response.Token != null)
    {
        return Results.Ok(response);
    }
    return Results.NotFound(response);
}).WithName("Authenticate user");

app.MapGet("/Register", [Authorize] (IUserController userController, string username, string password) =>
{
    string error;
    RegisterRequest req = new RegisterRequest() { UserName = username, Password = password };
    try
    {
        userController.Register(req);
        return Results.Ok("Registration successful");
    }
    catch (Exception e)
    {
        error = e.Message;
    }
    return Results.BadRequest(error);

}).WithName("Register user");


app.MapGet("/items", async (IRecipeController recipeController, IMapper mapper) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var result = await recipeController.GetAllItems().ToListAsync();

    if (result.Count > 0)
    {
        apiResponse.Items = mapper.Map(result, new List<ResponseItem>());
        //apiResponse.Items = result;
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }

    apiResponse.message = ResponseResult.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all items from db");

app.MapGet("/item/{itemId}", async (IRecipeController recipeController, IMapper mapper, int itemId) =>
{
    var result = await recipeController.GetItemFromId(itemId);
    ResponseResult apiResponse = new ResponseResult();

    if (result != null)
    {

        apiResponse.Items = new List<ResponseItem> { mapper.Map(result, new ResponseItem()) };
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = ResponseResult.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get info for a specific item");

app.MapGet("/recipes", async (IRecipeController recipeController, IMapper mapper) =>
{
    var result = await recipeController.GetAllRecipesList();
    ResponseResult apiResponse = new ResponseResult();

    if (result.Count() > 0)
    {
        apiResponse.Recipes = mapper.Map(result, new List<ResponseRecipe>());

        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = ResponseResult.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all recipes from db");

app.MapGet("/craftingcost/recipe/{worldName}/{recipeId}", async (IDataCentreController dataCentreController, IRecipeController recipeController, ICalculateCraftingCost calculateCraftingCost, IMapper mapper, string worldName, int recipeId) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var world = await dataCentreController.GetWorldFromName(worldName);
    var recipe = await recipeController.GetRecipesByIds(new List<int> { recipeId }).FirstOrDefaultAsync();
    var itemList = recipe.Ingredients.Select(x => x.Item).ToList();
    itemList.Add(recipe.Item);
    if (world == null || itemList.Count() == 0)
    {
        apiResponse.message = "Not Found";
        return Results.NotFound(apiResponse);
    }

    var result = new ResponseResult();
    result.CraftingCosts = new List<ResponseCraftingCostDto>();
    result.CraftingCosts = await calculateCraftingCost.GetCraftingCostRecipes(world, new List<Recipe> { recipe }, itemList);
    apiResponse.message = "ok";

    return Results.Ok(result);

})
.WithName("get crafting cost for recipe");
app.MapGet("/craftingcost/job/{worldName}/{jobAbr}", async (IDataCentreController dataCentreController, IRecipeController recipeController, ICalculateCraftingCost calculateCraftingCost, string worldName, string jobAbr) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var world = await dataCentreController.GetWorldFromName(worldName);
    var recipeList = await recipeController.GetMarketableRecipesByJob(jobAbr).ToListAsync();
    var itemList = recipeList.SelectMany(x => x.Ingredients.Select(x => x.Item)).ToList();
    itemList.AddRange(recipeList.Select(x => x.Item).ToList());
    itemList = itemList.DistinctBy(x => x.Id).ToList();
    if (world == null || itemList.Count() == 0)
    {
        apiResponse.message = "Not Found";
        return Results.NotFound(apiResponse);
    }

    var result = new ResponseResult();
    result.CraftingCosts = await calculateCraftingCost.GetCraftingCostRecipes(world, recipeList, itemList);
    apiResponse.message = "ok";

    return Results.Ok(result);

})
.WithName("get crafting cost for job");

app.MapGet("/recipe", async (IRecipeController recipeController, IMapper mapper, int? recipeId, int? itemId, string? recipeName) =>
{
    var result = new List<Recipe>();
    if (recipeId == null && itemId == null && recipeName == null)
    {
        return Results.BadRequest("Endpoint requires one of the following parameters: int recipeId, int itemId, string recipeName");
    }
    if (recipeId != null)
    {
        result = await recipeController.GetRecipesByIds(new List<int> { recipeId.Value }).ToListAsync();
    }
    else if (itemId != null)
    {
        result = await recipeController.GetRecipesByItemIds(new List<int> { itemId.Value }).ToListAsync();
    }
    else if (recipeName != null)
    {
        result = await recipeController.GetRecipesFromNameCollIncludeIngredients(new List<string> { recipeName }).ToListAsync();
    }
    ResponseResult apiResponse = new ResponseResult();

    if (result.Count > 0)
    {

        apiResponse.Recipes = mapper.Map(result, new List<ResponseRecipe>());
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = "Not Found";
    return Results.NotFound(apiResponse);
})
.WithName("get info for a specific recipe based on recipe/item-id or recipe name");

app.MapGet("/marketboard/{worldName}/{itemName}", async (
    IDataCentreController dataCentreController, IRecipeController recipeController,
    IUniversalisApiController universalisController, IMarketBoardController marketboardController, IMapper mapper,
    string worldName, string itemName) =>
{
    try
    {
        var item = await recipeController.GetItemFromNameAsync(itemName);
        var world = await dataCentreController.GetWorldFromName(worldName);
        if (item is null) return Results.NotFound("Item name gave no result");
        if (world is null) return Results.NotFound("World name gave no result");
        var result = await marketboardController.GetLatestUniversalisQueryForItem(item.Name_en, world.Name);
        if (result != null &&
        DateTime.Now.AddHours(-6) > result.LastUploadDate &&
        DateTime.Now.AddHours(-1) > result.QueryDate)
        {
            var updatedEntry = await universalisController.ImportUniversalisDataForItemAndWorld(item, world, 5, 5);
            if (updatedEntry is null) return Results.NotFound("Universalis returned no entries for item");

            var responseResult = new ResponseResult();
            responseResult.UniversalisEntry = new List<ResponseUniversalisEntry> { mapper.Map(updatedEntry, new ResponseUniversalisEntry()) };
            responseResult.message = "ok";
            return Results.Ok(responseResult);
        }

        if (result != null)
        {
            var responseResult = new ResponseResult();
            responseResult.UniversalisEntry = new List<ResponseUniversalisEntry> { mapper.Map(result, new ResponseUniversalisEntry()) };
            responseResult.message = "ok";
            return Results.Ok(responseResult);
        }

        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.StatusCode(500);
    }
})
.WithName("get marketboard entries for item");

app.MapGet("/marketboard/{worldName}", async (IDataCentreController dataCentreController, IRecipeController recipeController,
    IUniversalisApiController universalisController, IMarketBoardController marketboardController, IMapper mapper, string itemString, string worldName) =>
{
    try
    {
        var outdatedList = new List<Item>();
        //var resultList = new List<UniversalisEntry>();
        //var responseList = new List<UniversalisEntry>();

        var itemNames = itemString.Split(",").ToList();
        var items = await recipeController.GetItemFromNameList(itemNames);
        var world = await dataCentreController.GetWorldFromName(worldName);

        if (items.Count == 0 || world is null) return Results.NotFound("items or world not found");
        var resultList = await marketboardController.GetLatestUniversalisQueryForItems(itemNames.Take(10), worldName);
        foreach (var entry in resultList)
        {
            if (DateTime.Now.AddHours(-6) > entry.LastUploadDate && DateTime.Now.AddMinutes(-30) > entry.QueryDate)
            {
                outdatedList.Add(entry.Item);
            }
        }

        var responseList = resultList.Where(a => !outdatedList.Contains(a.Item)).ToList();
        if (outdatedList.Count > 0) { responseList.AddRange(await universalisController.ImportUniversalisDataForItemListAndWorld(outdatedList, world, 5, 5)); }
        if (responseList.Count() > 0)
        {
            var responseResult = new ResponseResult();
            responseResult.UniversalisEntry = mapper.Map(responseList, new List<ResponseUniversalisEntry>());
            responseResult.message = "ok";
            return Results.Ok(responseResult);
        }
        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message, null, 500);
    }
})
.WithName("get marketboard entries for list of items names");

app.MapPut("/import/marketboard", async (IUniversalisApiController universalisApiController, IDataCentreController dataCentreController, IRecipeController recipeController, IMapper mapper,
    int itemId, string worldName, int nrOfEntries, int nrOfListings) =>
{
    var world = await dataCentreController.GetWorldFromName(worldName);
    var item = await recipeController.GetItemFromId(itemId);

    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world, nrOfEntries, nrOfListings);

        if (result != null)
        {
            var responseResult = new ResponseResult();
            responseResult.UniversalisEntry = new List<ResponseUniversalisEntry> { mapper.Map(result, new ResponseUniversalisEntry()) };
            responseResult.message = "ok";
            return Results.Ok(responseResult);
        }
    }
    return Results.NotFound("World or Item not found");
})
.WithName("import item entry");

app.MapPut("/import/marketboard/{worldName}", async (IUniversalisApiController universalisApiController, IDataCentreController dataCentreController, string worldName) =>
{
    var world = await dataCentreController.GetWorldFromName(worldName);

    if (world != null)
    {
        try
        {
            await universalisApiController.ImportUniversalisDataForAllItemsOnWorld(world);
            return Results.Ok("Import successful");
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message, null, 500);
        }
    }
    return Results.NotFound("World not found");
})
.WithName("import items for world");

app.MapPut("/import/worlds", async (IXivApiController xivApiController) =>
{
    try
    {
        await xivApiController.ImportWorldsDataCentres();
        return Results.Ok("Import successful");
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message, null, 500);

    }
})
.WithName("import worlds");

app.MapPut("/import/recipes", async (IXivApiController xivApiController, IUniversalisApiController universalisApiController) =>
{
    try
    {
        await xivApiController.ImportRecipiesAndItems();
        await universalisApiController.ImportMarketableItems();
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message, null, 500);
    }

})
.WithName("import all recipes");

app.Run();
