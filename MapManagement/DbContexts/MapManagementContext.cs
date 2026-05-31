using MapManagement.Entity;
using MapManagement.Enums;
using Microsoft.EntityFrameworkCore;


namespace MapManagement.DbContexts

{
    public class MapManagementContext : DbContext
    {
        public MapManagementContext(DbContextOptions<MapManagementContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // önemli
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MapManagementContext>();

            optionsBuilder.UseNpgsql(
                connectionString,
                x => x.UseNetTopologySuite()
            );
        }

        public DbSet<LayerGroup> LayerGroups { get; set; }

        public DbSet<Layer> Layers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LayerGroup>().HasData(
               new LayerGroup { Id = 1, IsDeleted = false, Name = "Altlık Haritalar", OrderNo = 1 }
            );

            modelBuilder.Entity<Layer>().HasData(
               new Layer { Id = 1, IsDeleted = false, Name = "OSM Standart", Url = "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", LayerGroupId = 1, OrderNo = 1, Type = LayerType.BaseMap }
            );
        }
    }
}
