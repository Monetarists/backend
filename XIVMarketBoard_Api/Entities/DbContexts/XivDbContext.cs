using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System.Diagnostics;
using System.Linq;

namespace XIVMarketBoard_Backend.Data
{
    
    public class XivDbContext:DbContext
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
        public XivDbContext()
        {
        }

        public XivDbContext(DbContextOptions<XivDbContext> options)
            : base(options)
        {
        }
        public XivDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Models.Item> Items { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Recipe> Recipes { get; set; }
        public DbSet<Models.World> Servers { get; set; }
        public DbSet<Models.Job> Jobs { get; set; }
        public DbSet<Models.Retainer> Retainers { get; set; }
        public DbSet<Models.MbPost> MbPosts { get; set; }
        public DbSet<Models.SaleHistory> SaleHistory { get; set; }
    }
}
