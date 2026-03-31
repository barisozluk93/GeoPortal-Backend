using MapManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace MapManagement.DbContexts

{
    public class MapManagementContext : DbContext
    {
        public MapManagementContext(DbContextOptions<MapManagementContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<LayerGroup> LayerGroups { get; set; }

        public DbSet<Layer> Layers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LayerGroup>().HasData(
               new LayerGroup { Id = 1, IsDeleted = false, Name = "Altlık Haritalar" }
            );

            modelBuilder.Entity<Layer>().HasData(
               new Layer { Id = 1, IsDeleted = false, Name = "Carto Light", Url = "https://{a-d}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}.png", LayerGroupId = 1, IsBaseMap = true },
               new Layer { Id = 2, IsDeleted = false, Name = "OSM Standart", Url = "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", LayerGroupId = 1, IsBaseMap = true },
               new Layer { Id = 3, IsDeleted = false, Name = "OSM Hot", Url = "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png", LayerGroupId = 1, IsBaseMap = true }
            );
        }
    }
}
