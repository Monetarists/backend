using Microsoft.OpenApi.Models;
using XIVMarketBoard_Api.Data;
using Newtonsoft.Json;
using XIVMarketBoard_Api.Controller;
using XIVMarketBoard_Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header,

      },
      new List<string>()
    }});
});
builder.Services.AddTransient<IUniversalisApiController, UniversalisApiController>();
builder.Services.AddTransient<IXivApiController, XivApiController>();
builder.Services.AddTransient<IUniversalisApiRepository, UniversalisApiRepository>();
builder.Services.AddTransient<IXivApiRepository, XivApiRepository>();
builder.Services.AddTransient<IDbController, DbController>();
builder.Services.AddDbContext<XivDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}

//app.UseHttpsRedirection();

app.MapGet("/getAllItemNames", async (IDbController dbController) =>
{ 
    return JsonConvert.SerializeObject(await dbController.GetAllItems().ToListAsync());
})
.WithName("getAllItemNames");

app.MapGet("/all-recipies", () =>
{

    //var recipies = XivApiController.resetAndImportRecipiesAndItems();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("get all recipies from db");

app.MapGet("/recipe-by-name", async (IDbController dbController, string recipeName) =>
{

    var result = await dbController.GetRecipeFromNameIncludeIngredients(recipeName);
    if (result != null)
    {
        var nameList = result.Ingredients.Select(i => i.Item.Name).ToList();
        var subRecipeList = dbController.GetRecipesFromNameCollIncludeIngredients(nameList);
    }
    
    //fixa reply som har recipe/item -> ingredients -> recipes/items
    //troligen är best practise att skapa ett gäng models och jsonkoda dom
    return "";
})
.WithName("get recipe and items");

app.MapGet("/marketboard-entries", async (IDbController dbController, IEnumerable<string> itemNames, string worldName) =>
{
    return await dbController.GetLatestUniversalisQueryForItems(itemNames, worldName).ToListAsync(); ;
})
.WithName("get marketboard entries for item");



app.MapPut("/import-all-recipes", async  (IXivApiController xivApiController) =>
{

    var worlds = await xivApiController.ImportRecipiesAndItems();

    return "";
})
.WithName("import all recipes");


app.MapPut("/importWorlds", async (IXivApiController xivApiController) =>
{

    var result = await xivApiController.ImportAllWorldsAndDataCenters();

    return result;
})
.WithName("import worlds");

app.MapPut("/importItemEntry", async (int Itemid, string WorldName, int entries, int listings, IUniversalisApiController universalisApiController, IDbController dbController) =>
{

    var world = await dbController.GetWorldFromName(WorldName);
    var item = await dbController.GetItemFromId(Itemid);

    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world, entries, listings);
        return JsonConvert.SerializeObject(result);
    }
    

    return "";
})
.WithName("import item entry");

app.MapPut("/import-items-for-world", async (IUniversalisApiController universalisApiController, IDbController dbController, string worldName) =>
{

    var world = await dbController.GetWorldFromName(worldName);
    if (world != null)
    {
        return await universalisApiController.ImportUniversalisDataForAllItemsOnWorld(world);
    }


    return "error world not found";
})
.WithName("import items for world");


app.MapPut("/set-crafted-on-items", async  (IXivApiController xivApiController, IDbController dbController) =>
{

    var result = await dbController.SetCraftableItemsFromRecipes();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("update items crafted");

app.MapPut("/import-marketstatus-items", async (IUniversalisApiController universalisApiController) =>
{

    var result = await universalisApiController.ImportMarketableItems();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
}).WithName("import marketable status for items");

//app.Urls.Add("https://localhost:1923");
app.Run();
