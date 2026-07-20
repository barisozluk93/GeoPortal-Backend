using MapManagement.Entity;
using MapManagement.Enums;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;


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

        public DbSet<Layer> Layers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Layer>().HasData(
               new Layer { Id = 1, IsDeleted = false, Name = "OSM Standart", Url = "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", OrderNo = 1, Type = LayerType.BaseMap, IsVisible = true, Opacity = 1 },
               new Layer { Id = 2, IsDeleted = false, Name = "Topografik Harita", Url = "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png", OrderNo = 2, Type = LayerType.BaseMap, IsVisible = false, Opacity = 1 },
               new Layer { Id = 3, IsDeleted = false, Name = "Uydu Görüntüsü", Url = "https://taiearth.com/geoserver/Maps/wms", OrderNo = 3, Type = LayerType.Wms, LayerName = "Maps:blue_marble", Format = "image/png", IsVisible = false, Opacity = 1 }
            );
        }
    }
}
