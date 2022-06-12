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
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("XivDbConnectionString");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }*/
        private const string connectionString = "server=localhost;port=3306;database=EFCoreMySQL;user=root;password=root";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Data Source=192.168.10.38,3306;initial catalog=XivMarketBoard;User ID=XivUser;Password=pot01atis;Trusted_Connection=false;");
                optionsBuilder.UseMySql("Data Source=192.168.10.38,3306;initial catalog=XivMarketBoard;User ID=XivUser;Password=pot01atis;",
                new MySqlServerVersion(new Version(8, 0, 11)));
            }

        }

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
    }
}
