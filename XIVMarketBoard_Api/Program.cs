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
Builder.ConfigureServices(builder);

var app = Builder.ConfigureWebApp(builder);

app.MapGet("/Authenticate", (IUserController userController, string username, string password) =>
{
    var req = new AuthenticateRequest(username, password);
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
    var req = new RegisterRequest(username, password);
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

app.MapGet("/items", async (IRecipeController recipeController, IMapper mapper) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var result = await recipeController.GetAllItems();

    if (result.Any())
    {
        apiResponse.Items = mapper.Map(result, new List<ResponseItem>());
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }

    apiResponse.message = ResponseResult.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all items from db");

app.MapGet("/recipe/{recipeId}", async (IRecipeController recipeController, IMapper mapper, int recipeId) =>
{
    var result = await recipeController.GetRecipesByIds(new List<int> { recipeId }).ToListAsync();
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
.WithName("get info for a specific recipe based on recipe id");

app.MapGet("/recipe/{recipeId}/{worldName}", async (IDataCentreController dataCentreController, IRecipeController recipeController, ICalculateCraftingCost calculateCraftingCost, IMapper mapper, int recipeId, string worldName) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var world = await dataCentreController.GetWorldFromName(worldName);
    var recipe = await recipeController.GetRecipesByIds(new List<int> { recipeId }).FirstOrDefaultAsync();
    if (recipe is null)
    {
        return Results.NotFound("recipe not found");
    }
    var itemList = recipe.Ingredients.Select(x => x.Item).ToList();
    itemList.Add(recipe.Item);
    if (world == null || !itemList.Any())
    {
        apiResponse.message = "Not Found";
        return Results.NotFound(apiResponse);
    }

    var result = new ResponseResult();
    result.Recipes = await calculateCraftingCost.GetCraftingCostRecipes(world, new List<Recipe> { recipe }, itemList);
    apiResponse.message = "ok";

    return Results.Ok(result);

})
.WithName("get crafting cost for recipe");

app.MapGet("/recipes", async (IRecipeController recipeController, IMapper mapper) =>
{
    var result = await recipeController.GetAllRecipesList();
    ResponseResult apiResponse = new ResponseResult();

    if (result.Any())
    {
        apiResponse.Recipes = mapper.Map(result, new List<ResponseRecipe>());

        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = ResponseResult.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all recipes from db");


app.MapGet("/recipes/{jobAbr}/{worldName}", async (IDataCentreController dataCentreController, IRecipeController recipeController, ICalculateCraftingCost calculateCraftingCost, string jobAbr, string worldName) =>
{
    ResponseResult apiResponse = new ResponseResult();
    var world = await dataCentreController.GetWorldFromName(worldName);
    var recipeList = await recipeController.GetMarketableRecipesByJob(jobAbr).ToListAsync();
    var itemList = recipeList.SelectMany(x => x.Ingredients.Select(x => x.Item)).ToList();
    itemList.AddRange(recipeList.Select(x => x.Item).ToList());
    itemList = itemList.DistinctBy(x => x.Id).ToList();
    if (world == null || !itemList.Any())
    {
        apiResponse.message = "Not Found";
        return Results.NotFound(apiResponse);
    }

    var result = new ResponseResult();
    result.Recipes = await calculateCraftingCost.GetCraftingCostRecipes(world, recipeList, itemList);
    apiResponse.message = "ok";

    return Results.Ok(result);

})
.WithName("get crafting cost for job");



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
        var responseResult = new ResponseResult();

        if (result == null ||
        (DateTime.UtcNow.AddHours(-6) > result.LastUploadDate && DateTime.UtcNow.AddHours(-1) > result.QueryDate))
        {
            var updatedEntry = await universalisController.ImportUniversalisDataForItemAndWorld(item, world);
            if (updatedEntry is null) return Results.NotFound("Universalis returned no entries for item");

            responseResult.UniversalisEntries = new List<ResponseUniversalisEntry> { mapper.Map(updatedEntry, new ResponseUniversalisEntry()) };
            responseResult.message = "ok";
            return Results.Ok(responseResult);
        }
        responseResult.UniversalisEntries = new List<ResponseUniversalisEntry> { mapper.Map(result, new ResponseUniversalisEntry()) };
        responseResult.message = "ok";
        return Results.Ok(responseResult);
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message);
    }
})
.WithName("get marketboard entries for item");

app.MapGet("/marketboard/{worldName}", async (IDataCentreController dataCentreController, IRecipeController recipeController,
    IUniversalisApiController universalisController, IMarketBoardController marketboardController, IMapper mapper, string itemString, string worldName) =>
{
    try
    {
        var outdatedList = new List<Item>();
        var itemNames = itemString.Split(",").ToList();
        var items = await recipeController.GetItemFromNameList(itemNames);
        var world = await dataCentreController.GetWorldFromName(worldName);

        if (items.Count == 0 || world is null) return Results.NotFound("items or world not found");
        var resultList = await marketboardController.GetLatestUniversalisQueryForItems(itemNames.Take(10), worldName);
        foreach (var entry in resultList)
        {
            if (entry != null && DateTime.UtcNow.AddHours(-6) > entry.LastUploadDate && DateTime.UtcNow.AddMinutes(-30) > entry.QueryDate)
            {
                outdatedList.Add(entry.Item);
            }
        }

        var responseList = resultList.Where(a => a != null && !outdatedList.Contains(a.Item)).ToList();
        if (outdatedList.Any()) { responseList.AddRange(await universalisController.ImportUniversalisDataForItemListAndWorld(outdatedList, world)); }
        if (responseList.Any())
        {
            var responseResult = new ResponseResult();
            responseResult.UniversalisEntries = mapper.Map(responseList, new List<ResponseUniversalisEntry>());
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
    int itemId, string worldName) =>
{
    var world = await dataCentreController.GetWorldFromName(worldName);
    var item = await recipeController.GetItemFromId(itemId);

    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world);

        if (result != null)
        {
            var responseResult = new ResponseResult();
            responseResult.UniversalisEntries = new List<ResponseUniversalisEntry> { mapper.Map(result, new ResponseUniversalisEntry()) };
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
