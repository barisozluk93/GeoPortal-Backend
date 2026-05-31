using Microsoft.EntityFrameworkCore;
using OrderManagement.Entity;
using System;


namespace OrderManagement.DbContexts

{
    public class OrderManagementContext : DbContext
    {
        public OrderManagementContext(DbContextOptions<OrderManagementContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // önemli
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementContext>();

            optionsBuilder.UseNpgsql(
                connectionString,
                x => x.UseNetTopologySuite()
            );
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductClass> ProductClasses { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketProduct> BasketProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<ApiKeyPermission> ApiKeyPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderManagementContext).Assembly);
        }
    }
}
