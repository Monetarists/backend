using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using XIVMarketBoard_Api.Controller;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api;
using XIVMarketBoard_Api.Data;
using XIVMarketBoard_Api.Repositories;
using XIVMarketBoard_Api.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using AutoMapper;
using XIVMarketBoard_Api.Helpers;
using XIVMarketBoard_Api.Authorization;
using XIVMarketBoard_Api.Repositories.Models.Users;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
                      {
                          policy.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { }}
    });
});

services.AddAutoMapper(typeof(MapperProfile));
services.AddTransient<IUniversalisApiController, UniversalisApiController>();
services.AddTransient<IXivApiController, XivApiController>();
services.AddTransient<IUniversalisApiRepository, UniversalisApiRepository>();
services.AddTransient<IXivApiRepository, XivApiRepository>();
services.AddTransient<IMarketBoardApiController, MarketBoardApiController>();
services.AddTransient<IJwtUtils, JwtUtils>();
services.AddScoped<IUserController, UserController>();
services.AddTransient<IMapper, Mapper>();

services.AddDbContext<XivDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("XivDbConnectionString"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("XivDbConnectionString")),
                builder => builder.MigrationsAssembly(typeof(XivDbContext).Assembly.FullName)));
services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Key"]))
    };
});

var app = builder.Build();
app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
app.UseCors();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}

app.MapGet("/Authenticate", (IUserController userController, string username, string password) => {
    AuthenticateRequest req = new AuthenticateRequest() { UserName = username, Password = password};
    var response = userController.Authenticate(req);
    if(response.Token != null)
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


app.MapGet("/items", async (IMarketBoardApiController marketboardApiController) =>
{ 
    var result = await marketboardApiController.GetAllItems().ToListAsync();

    ResponseDto apiResponse = new ResponseDto();
    

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



//get an item based on item id
app.MapGet("/item", async (IMarketBoardApiController marketboardApiController, int itemId) =>
{
    var result = await marketboardApiController.GetItemFromId(itemId);

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

app.MapGet("/recipes", async (IMarketBoardApiController marketboardApiController) =>
{
    var result = await marketboardApiController.GetAllRecipes().ToListAsync();

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

//gets a singular recipe by recipe id
app.MapGet("/recipe", async (IMarketBoardApiController marketboardApiController, int? recipeId, int? itemId, string? recipeName) =>
{
    var result = new List<Recipe>();
    if(recipeId == null && itemId == null && recipeName == null)
    {
        return Results.BadRequest("Endpoint requires one of the following parameters: int? recipeId, int? itemId, string? recipeName");
    }
    if (recipeId != null)
    {
        result = await marketboardApiController.GetRecipesByIds(new List<int> { recipeId.Value }).ToListAsync();
    } else if (itemId != null)
    {
        result = await marketboardApiController.GetRecipesByItemIds(new List<int> { itemId.Value }).ToListAsync();
    } else if (recipeName != null)
    {
        result = await marketboardApiController.GetRecipesFromNameCollIncludeIngredients(new List<string> { recipeName }).ToListAsync();
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

/*
//gets all recipes that produce an item. Different jobs have different recipes for the same item
app.MapGet("/recipe", async (IMarketBoardApiController marketboardApiController, int itemId) =>
{
    var result = await marketboardApiController.GetRecipesByItemIds(new List<int> { itemId }).ToListAsync();

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
.WithName("get info for a specific recipe based on item id"); 

app.MapGet("/recipe", async (IMarketBoardApiController marketboardApiController, string recipeName) =>
{

    var result = await marketboardApiController.GetRecipeFromNameIncludeIngredients(recipeName);
    if (result != null)
    {
        var nameList = result.Ingredients.Select(i => i.Item.Name).ToList();
        var subRecipeList = marketboardApiController.GetRecipesFromNameCollIncludeIngredients(nameList);
    }
    
    //fixa reply som har recipe/item -> ingredients -> recipes/items
    //troligen är best practise att skapa ett gäng models och jsonkoda dom
    return "";
})
.WithName("get recipe by name");*/

app.MapGet("/marketboard-entries", [Authorize] async (IMarketBoardApiController marketboardApiController, IEnumerable<string> itemNames, string worldName) =>
{
    return await marketboardApiController.GetLatestUniversalisQueryForItems(itemNames, worldName).ToListAsync(); ;
})
.WithName("get marketboard entries for item");





app.MapPut("/import-all-recipes", [Authorize] async  (IXivApiController xivApiController, IUniversalisApiController universalisApiController) =>
{

    var Recipes = await xivApiController.ImportRecipiesAndItems();
    var result2 = await universalisApiController.ImportMarketableItems();

    return "";
})
.WithName("import all recipes");

app.MapPut("/reImport-all-recipes", [Authorize] async (IXivApiController xivApiController) =>
{

    var worlds = await xivApiController.ImportRecipiesAndItems();

    return "";
})
.WithName("reset and import all recipes");



app.MapPut("/importWorlds", [Authorize] async (IXivApiController xivApiController) =>
{

    var result = await xivApiController.ImportAllWorldsAndDataCenters();

    return result;
})
.WithName("import worlds");

app.MapPut("/importItemEntry", [Authorize] async (int Itemid, string WorldName, int entries, int listings, IUniversalisApiController universalisApiController, IMarketBoardApiController marketboardApiController) =>
{

    var world = await marketboardApiController.GetWorldFromName(WorldName);
    var item = await marketboardApiController.GetItemFromId(Itemid);

    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world, entries, listings);
        return JsonConvert.SerializeObject(result);
    }
    

    return "";
})
.WithName("import item entry");

app.MapPut("/import-items-for-world", [Authorize] async (IUniversalisApiController universalisApiController, IMarketBoardApiController marketboardApiController, string worldName) =>
{

    var world = await marketboardApiController.GetWorldFromName(worldName);
    if (world != null)
    {
        return await universalisApiController.ImportUniversalisDataForAllItemsOnWorld(world);
    }


    return "error world not found";
})
.WithName("import items for world");


app.MapPut("/set-crafted-on-items", [Authorize] async  (IXivApiController xivApiController, IMarketBoardApiController marketboardApiController) =>
{

    var result = await marketboardApiController.SetCraftableItemsFromRecipes();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("update items crafted");

app.MapPut("/import-marketstatus-items", [Authorize] async (IUniversalisApiController universalisApiController) =>
{

    var result = await universalisApiController.ImportMarketableItems();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
}).WithName("import marketable status for items");

//app.Urls.Add("https://localhost:1923");
app.Run();
