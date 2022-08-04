using Microsoft.OpenApi.Models;
using XIVMarketBoard_Api.Controller;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using XIVMarketBoard_Api.Data;
using XIVMarketBoard_Api.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using AutoMapper;
using XIVMarketBoard_Api.Helpers;
using XIVMarketBoard_Api.Tools;
using XIVMarketBoard_Api.Authorization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace XIVMarketBoard_Api
{
    public static class Builder
    {
        public static IServiceCollection ConfigureServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;
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
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new string[] { } } });
            });
            services.AddControllers().AddNewtonsoftJson(o => { o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddTransient<ICalculateCraftingCost, CalculateCraftingCost>();
            services.AddTransient<IUniversalisApiController, UniversalisApiController>();
            services.AddTransient<IXivApiController, XivApiController>();
            services.AddTransient<IUniversalisApiRepository, UniversalisApiRepository>();
            services.AddTransient<IXivApiRepository, XivApiRepository>();
            services.AddTransient<IMarketBoardController, MarketBoardController>();
            services.AddScoped<IUserController, UserController>();
            services.AddTransient<IRecipeController, RecipeController>();
            services.AddTransient<IDataCentreController, DataCentreController>();
            services.AddTransient<IJwtUtils, JwtUtils>();
            services.AddTransient<IMapper, Mapper>();

            services.AddDbContext<XivDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("XivDbConnectionString"),
                    ServerVersion.AutoDetect(
                        builder.Configuration.GetConnectionString("XivDbConnectionString")
                    ),
                    options => options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)

                )
            );

            services.Configure<AppSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            return services;
        }

        public static WebApplication ConfigureWebApp(WebApplicationBuilder builder)
        {
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
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            return app;
        }

    }
}
