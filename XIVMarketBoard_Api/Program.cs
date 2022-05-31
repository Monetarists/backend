using Microsoft.OpenApi.Models;
using XIVMarketBoard_Api;
using Newtonsoft.Json;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}

app.UseHttpsRedirection();


app.MapGet("/importWorlds", () =>
{
    var worlds = XIVApiController.ImportAllWorldsAndDataCenters();

    return JsonConvert.SerializeObject(worlds.Result);
})
.WithName("getWorlds");


app.MapGet("/getAllRecipiesAsync", () =>
{

    var recipies = XIVApiController.resetAndImportRecipiesAndItems();

    return JsonConvert.SerializeObject(recipies.Result);
})
.WithName("Import All Recipies From XIV Api Async");
//app.Urls.Add("https://localhost:1923");
app.Run();


/*
 
 
  {
    "indexes": "recipe",
    "page": "1",
    "columns": "ID,Name,UrlType",
    "body": {
        "query": {
            "bool": {
                "must": [
                    {
                        "wildcard": {
                            "Name_en": "*"
                        }
                    }
                ]
            }
        },
        "from": 2,
        "size": 1
    }
}
 */