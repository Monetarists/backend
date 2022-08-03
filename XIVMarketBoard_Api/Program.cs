using XIVMarketBoard_Api.Controller;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api;
using XIVMarketBoard_Api.Entities;
using XIVMarketBoard_Api.Repositories.Models.Users;
using Microsoft.AspNetCore.Authorization;

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

/*app.MapGet("/Register", [Authorize] (IUserController userController, string username, string password) =>
{
    string error;
    RegisterRequest req = new RegisterRequest() { UserName = username, Password = password };
    try
    {
        userController.Register(req);
        return Results.Ok("Registration successful");
    }
    catch(Exception e)
    {
        error = e.Message;
    }
    return Results.BadRequest(error);

}).WithName("Register user");*/


app.MapGet("/items", async (IRecipeController recipeController) =>
{
    ResponseDto apiResponse = new ResponseDto();
    var result = await recipeController.GetAllItems().ToListAsync();

    if (result.Count > 0)
    {
        //remove take10
        apiResponse.Items = result.Take(10);
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }

    apiResponse.message = ResponseDto.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all items from db");

app.MapGet("/item/{itemId}", async (IRecipeController recipeController, int itemId) =>
{
    var result = await recipeController.GetItemFromId(itemId);
    ResponseDto apiResponse = new ResponseDto();

    if (result != null)
    {
        //remove take10
        apiResponse.Items = new List<Item> { result };
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = ResponseDto.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get info for a specific item");

app.MapGet("/recipes", async (IRecipeController recipeController) =>
{
    var result = await recipeController.GetAllRecipesList();
    ResponseDto apiResponse = new ResponseDto();

    if (result.Count > 0)
    {
        //remove take10    
        apiResponse.Recipes = result.Take(10);
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = ResponseDto.noItemsMessage;
    return Results.NotFound(apiResponse);
})
.WithName("get all recipes from db");

app.MapGet("/recipe", async (IRecipeController recipeController, int? recipeId, int? itemId, string? recipeName) =>
{
    var result = new List<Recipe>();
    if (recipeId == null && itemId == null && recipeName == null)
    {
        return Results.BadRequest("Endpoint requires one of the following parameters: int? recipeId, int? itemId, string? recipeName");
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
    ResponseDto apiResponse = new ResponseDto();

    if (result.Count > 0)
    {
        //remove take10
        apiResponse.Recipes = result;
        apiResponse.message = "ok";
        return Results.Ok(apiResponse);
    }
    apiResponse.message = "Not Found";
    return Results.NotFound(apiResponse);
})
.WithName("get info for a specific recipe based on recipe/item-id or recipe name");

app.MapGet("/marketboard/{worldName}/{itemName}", async (
    IDataCentreController dataCentreController, IRecipeController recipeController,
    IUniversalisApiController universalisController, IMarketBoardController marketboardController,
    string itemName, string worldName) =>
{
    try
    {
        var item = await recipeController.GetItemFromNameAsync(itemName);
        var world = await dataCentreController.GetWorldFromName(worldName);
        if (item is null) return Results.NotFound("Item name gave no result");
        if (world is null) return Results.NotFound("Item name gave no result");
        var result = await marketboardController.GetLatestUniversalisQueryForItem(item.Name_en, world.Name);
        if (result != null &&
        DateTime.Now.AddHours(-6) > result.LastUploadDate &&
        DateTime.Now.AddMinutes(-30) > result.QueryDate)
        {
            var updatedEntry = await universalisController.ImportUniversalisDataForItemAndWorld(item, world, 5, 5);
            if (updatedEntry is null) return Results.NotFound("Universalis returned no entries for item");

            return Results.Ok(new ResponseDto() { message = "ok", UniversalisEntry = new List<UniversalisEntry> { updatedEntry } });
        }

        if (result != null) { return Results.Ok(new ResponseDto() { message = "ok", UniversalisEntry = new List<UniversalisEntry> { result } }); }

        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.StatusCode(500);
    }
})
.WithName("get marketboard entries for item");

app.MapGet("/marketboard/{worldName}", async (IDataCentreController dataCentreController, IRecipeController recipeController,
    IUniversalisApiController universalisController, IMarketBoardController marketboardController, string itemString, string worldName) =>
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
        //items.ForEach(async i => resultList.Add(
        //   await marketboardController.GetLatestUniversalisQueryForItem(i.Id, world.Id) ??
        //   throw new ArgumentNullException("Null was returned from context")));
        var resultList = await marketboardController.GetLatestUniversalisQueryForItems(itemNames, worldName);
        //var resultList = await marketboardController.GetLatestUniversalisQueryForItems(items.Select(a => a.Name), worldName);
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
            return Results.Ok(new ResponseDto() { message = "ok", UniversalisEntry = responseList });
        }
        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message, null, 500);
    }
})
.WithName("get marketboard entries for list of items names");

app.MapPut("/import/marketboard", [Authorize] async (IUniversalisApiController universalisApiController, IDataCentreController dataCentreController, IRecipeController recipeController,
    int itemId, string worldName, int nrOfEntries, int nrOfListings) =>
{
    var world = await dataCentreController.GetWorldFromName(worldName);
    var item = await recipeController.GetItemFromId(itemId);

    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world, nrOfEntries, nrOfListings);

        if (result != null)
        {

            return Results.Ok(new ResponseDto() { message = "ok", UniversalisEntry = new List<UniversalisEntry>() { result } });
        }
    }
    return Results.NotFound("World or Item not found");
})
.WithName("import item entry");

app.MapPut("/import/marketboard/{worldName}", [Authorize] async (IUniversalisApiController universalisApiController, IDataCentreController dataCentreController, string worldName) =>
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

app.MapPut("/import/worlds", [Authorize] async (IXivApiController xivApiController) =>
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

app.MapPut("/import/recipes", [Authorize] async (IXivApiController xivApiController, IUniversalisApiController universalisApiController) =>
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
