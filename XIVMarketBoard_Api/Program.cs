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

app.UseHttpsRedirection();


app.MapGet("/importWorlds", (IXivApiController xivApiController) =>
{
    
    var worlds = xivApiController.ImportAllWorldsAndDataCenters();

    return JsonConvert.SerializeObject(worlds.Result);
})
.WithName("getWorlds");

app.MapGet("/importRecipies", async  (IXivApiController xivApiController) =>
{

    var worlds = await xivApiController.ImportRecipiesAndItems();

    return "";
})
.WithName("importRecipies");

app.MapGet("/importItemEntry", async (int Itemid, string WorldName, int entries, int listings, IUniversalisApiController universalisApiController, IDbController dbController) =>
{

    var world = dbController.GetWorldFromName(WorldName);
    var item = dbController.GetItemFromId(Itemid);
    if (world != null && item != null)
    {
        var result = await universalisApiController.ImportUniversalisDataForItemAndWorld(item, world, entries, listings);
        return JsonConvert.SerializeObject(result);
    }
    

    return "";
})
.WithName("importItemEntry");

app.MapGet("/importAllItemForWorld", async (IUniversalisApiController universalisApiController, IDbController dbController) =>
{

    var world = dbController.GetWorldFromName("Twintania");
    if (world != null)
    {
        return await universalisApiController.ImportUniversalisDataForAllItemsOnWorld(world);
    }


    return "";
})
.WithName("importAllItemForWorld");
app.MapGet("/getAllRecipiesAsync", () =>
{

    //var recipies = XivApiController.resetAndImportRecipiesAndItems();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("Import All Recipies From XIV Api Async");

app.MapGet("/updateItemsCrafted", async  (IXivApiController xivApiController, IDbController dbController) =>
{

    var result = await dbController.SetCraftableItemsFromRecipes();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("update items crafted");

app.MapGet("/GetMarketableItems", async (IUniversalisApiController universalisApiController) =>
{

    var result = await universalisApiController.ImportMarketableItems();
    return "";
    //return JsonConvert.SerializeObject(recipies.Result);
}).WithName("GetMarketableItems");

//app.Urls.Add("https://localhost:1923");
app.Run();
