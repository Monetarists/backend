using System.Data.Entity;
namespace XIVMarketBoard_Backend.Data
{
    public class XivDbContext:DbContext
    {
        public DbSet<Models.Item> Items { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Recipe> Recipes { get; set; }
        public DbSet<Models.World> Worlds { get; set; }
        public DbSet<Models.Job> Jobs { get; set; }
    }
}
