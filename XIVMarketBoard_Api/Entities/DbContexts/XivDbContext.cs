using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System.Diagnostics;
using System.Linq;
using XIVMarketBoard_Api.Entities;

namespace XIVMarketBoard_Api.Data
{

    public class XivDbContext : DbContext
    {

        public XivDbContext(DbContextOptions<XivDbContext> options)
            : base(options)
        {
        }
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<World> Worlds => Set<World>();
        public DbSet<DataCenter> DataCenters => Set<DataCenter>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<Retainer> Retainers => Set<Retainer>();
        public DbSet<MbPost> Posts => Set<MbPost>();
        public DbSet<SaleHistory> SaleHistory => Set<SaleHistory>();
        public DbSet<UniversalisEntry> UniversalisEntries => Set<UniversalisEntry>();
        public DbSet<ItemSearchCategory> ItemSearchCategory => Set<ItemSearchCategory>();
        public DbSet<ItemUICategory> ItemUICategory => Set<ItemUICategory>();
    }
}
