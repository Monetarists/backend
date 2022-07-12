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

app.MapGet("/item", async (IRecipeController recipeController, int itemId) =>
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
    var result = await recipeController.GetAllRecipes().ToListAsync();
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

app.MapGet("/marketboard/{itemName}", [Authorize] async (IMarketBoardController marketboardApiController, string itemName, string worldName) =>
{
    try
    {
        var result = await marketboardApiController.GetLatestUniversalisQueryForItems(new List<string>() { itemName }, worldName).ToListAsync();
        if (result.Count > 0) { return Results.Ok(result); }
        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.StatusCode(500);
    }
})
.WithName("get marketboard entries for item");

app.MapGet("/marketboard", [Authorize] async (IMarketBoardController marketboardApiController, IEnumerable<string> itemNames, string worldName) =>
{
    try
    {
        var result = await marketboardApiController.GetLatestUniversalisQueryForItems(itemNames, worldName).ToListAsync();

        if (result.Count > 0)
        {
            return Results.Ok(new ResponseDto() { message = "ok", UniversalisEntry = result });
        }
        return Results.NotFound("No marketboard entries found");
    }
    catch (Exception e)
    {
        return Results.Problem(e.Message, null, 500);
    }
})
.WithName("get marketboard entries for list of items item");

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

app.MapPut("/importWorlds", [Authorize] async (IXivApiController xivApiController) =>
{
    try
    {
        await xivApiController.ImportAllWorldsAndDataCenters();
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
