using Microsoft.EntityFrameworkCore;
using OrderManagement.Entity;


namespace OrderManagement.DbContexts

{
    public class OrderManagementContext : DbContext
    {
        public OrderManagementContext(DbContextOptions<OrderManagementContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
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
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(500);
                entity.Property(x => x.DownloadLink).HasMaxLength(1000);
                entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
                entity.Property(x => x.PriceStr).HasMaxLength(50);
                entity.Property(x => x.City).HasMaxLength(150);
                entity.Property(x => x.District).HasMaxLength(150);
                entity.Property(x => x.Provider).HasMaxLength(150);
                entity.Property(x => x.Resolution).HasColumnType("decimal(18,2)");
                entity.Property(x => x.AreaKm2).HasColumnType("decimal(18,2)");
                entity.Property(x => x.Currency).HasMaxLength(10);
                entity.Property(x => x.ThumbnailUrl).HasMaxLength(1000);
                entity.Property(x => x.PreviewUrl).HasMaxLength(1000);
                entity.Property(x => x.Description).HasMaxLength(4000);

                entity.HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "API Key",
                        Price = 1000,
                        PriceStr = "₺1000",
                        IsDeleted = false,
                        CategoryId = 2,
                        Currency = "TRY"
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "İstanbul Avrupa Yakası Uydu Görüntüsü - Beşiktaş",
                        DownloadLink = "https://example.com/downloads/product-1.zip",
                        Price = 18500,
                        PriceStr = "₺18.500",
                        IsDeleted = false,
                        CategoryId = 1,
                        City = "İstanbul",
                        District = "Beşiktaş",
                        AcquisitionDate = new DateTime(2026, 3, 12, 10, 15, 0),
                        Provider = "Maxar",
                        Resolution = 0.3m,
                        CloudRate = 4,
                        AreaKm2 = 42.8m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=1200",
                        Description = "Yüksek çözünürlüklü şehir uydu görüntüsü.",
                        IsOrthorectified = true,
                        IsPansharpened = true,
                        IsClassified = true
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Ankara Tarım Analizi - Gölbaşı",
                        DownloadLink = "https://example.com/downloads/product-2.zip",
                        Price = 12400,
                        PriceStr = "₺12.400",
                        IsDeleted = false,
                        CategoryId = 2,
                        City = "Ankara",
                        District = "Gölbaşı",
                        AcquisitionDate = new DateTime(2026, 2, 28, 9, 40, 0),
                        Provider = "Planet",
                        Resolution = 0.5m,
                        CloudRate = 9,
                        AreaKm2 = 87.4m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=1200",
                        Description = "Tarım alanları için analiz verisi.",
                        IsOrthorectified = true,
                        IsPansharpened = false,
                        IsClassified = true
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "İzmir Kıyı Uydu Görüntüsü",
                        DownloadLink = "https://example.com/downloads/product-3.zip",
                        Price = 21800,
                        PriceStr = "₺21.800",
                        IsDeleted = false,
                        CategoryId = 3,
                        City = "İzmir",
                        District = "Karşıyaka",
                        AcquisitionDate = new DateTime(2026, 3, 5, 14, 20, 0),
                        Provider = "Airbus",
                        Resolution = 0.4m,
                        CloudRate = 6,
                        AreaKm2 = 58.9m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=1200",
                        Description = "Kıyı ve şehir birleşik uydu görüntüsü.",
                        IsOrthorectified = true,
                        IsPansharpened = true,
                        IsClassified = false
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Bursa Sanayi Bölgesi Uydu Verisi",
                        DownloadLink = "https://example.com/downloads/product-4.zip",
                        Price = 16750,
                        PriceStr = "₺16.750",
                        IsDeleted = false,
                        CategoryId = 4,
                        City = "Bursa",
                        District = "Nilüfer",
                        AcquisitionDate = new DateTime(2026, 1, 19, 11, 10, 0),
                        Provider = "Maxar",
                        Resolution = 0.3m,
                        CloudRate = 3,
                        AreaKm2 = 65.1m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=1200",
                        Description = "Sanayi alanı analiz görüntüsü.",
                        IsOrthorectified = true,
                        IsPansharpened = true,
                        IsClassified = true
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Antalya Kıyı Uydu Paketi",
                        DownloadLink = "https://example.com/downloads/product-5.zip",
                        Price = 14300,
                        PriceStr = "₺14.300",
                        IsDeleted = false,
                        CategoryId = 5,
                        City = "Antalya",
                        District = "Alanya",
                        AcquisitionDate = new DateTime(2026, 3, 18, 8, 30, 0),
                        Provider = "Planet",
                        Resolution = 0.5m,
                        CloudRate = 7,
                        AreaKm2 = 73.6m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=1200",
                        Description = "Turizm ve kıyı analiz verisi.",
                        IsOrthorectified = true,
                        IsPansharpened = false,
                        IsClassified = true
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Konya Tarım Uydu Görüntüsü",
                        DownloadLink = "https://example.com/downloads/product-6.zip",
                        Price = 9800,
                        PriceStr = "₺9.800",
                        IsDeleted = false,
                        CategoryId = 2,
                        City = "Konya",
                        District = "Selçuklu",
                        AcquisitionDate = new DateTime(2026, 2, 11, 16, 45, 0),
                        Provider = "Sentinel",
                        Resolution = 1.0m,
                        CloudRate = 12,
                        AreaKm2 = 120.3m,
                        Currency = "TRY",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=600",
                        PreviewUrl = "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=1200",
                        Description = "Geniş tarım alanı gözlemi.",
                        IsOrthorectified = true,
                        IsPansharpened = false,
                        IsClassified = true
                    }
                );
            });

            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.ToTable("ApiKeys");

                entity.HasKey(x => x.Id);
                entity.Property(x => x.Key).HasMaxLength(200).IsRequired();
                entity.HasIndex(x => x.Key).IsUnique();

                entity.HasMany(x => x.Permissions)
                    .WithOne(x => x.ApiKey)
                    .HasForeignKey(x => x.ApiKeyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiKeyPermission>(entity =>
            {
                entity.ToTable("ApiKeyPermissions");

                entity.HasKey(x => x.Id);
                entity.Property(x => x.Endpoint).HasMaxLength(500).IsRequired();
            });

            modelBuilder.Entity<ProductClass>(entity =>
            {
                entity.ToTable("ProductClasses");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.ClassName).HasMaxLength(200);
                entity.Property(x => x.ColorHex).HasMaxLength(20);

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.Classes)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasData(
                    new ProductClass { Id = 1, ProductId = 2, ClassName = "Yerleşim", PixelCount = 124500, ColorHex = "#ff6b6b" },
                    new ProductClass { Id = 2, ProductId = 2, ClassName = "Yol", PixelCount = 48210, ColorHex = "#ffd166" },
                    new ProductClass { Id = 3, ProductId = 2, ClassName = "Yeşil Alan", PixelCount = 38120, ColorHex = "#06d6a0" },
                    new ProductClass { Id = 4, ProductId = 2, ClassName = "Su", PixelCount = 19340, ColorHex = "#118ab2" },

                    new ProductClass { Id = 5, ProductId = 3, ClassName = "Tarım Alanı", PixelCount = 211430, ColorHex = "#8ac926" },
                    new ProductClass { Id = 6, ProductId = 3, ClassName = "Boş Arazi", PixelCount = 73450, ColorHex = "#c2b280" },
                    new ProductClass { Id = 7, ProductId = 3, ClassName = "Yol", PixelCount = 19410, ColorHex = "#adb5bd" },
                    new ProductClass { Id = 8, ProductId = 3, ClassName = "Su", PixelCount = 16420, ColorHex = "#1982c4" },

                    new ProductClass { Id = 9, ProductId = 4, ClassName = "Yerleşim", PixelCount = 98400, ColorHex = "#ef476f" },
                    new ProductClass { Id = 10, ProductId = 4, ClassName = "Su", PixelCount = 62450, ColorHex = "#3a86ff" },
                    new ProductClass { Id = 11, ProductId = 4, ClassName = "Liman", PixelCount = 14200, ColorHex = "#8338ec" },

                    new ProductClass { Id = 12, ProductId = 5, ClassName = "Sanayi", PixelCount = 110230, ColorHex = "#ff9f1c" },
                    new ProductClass { Id = 13, ProductId = 5, ClassName = "Yol", PixelCount = 36610, ColorHex = "#adb5bd" },
                    new ProductClass { Id = 14, ProductId = 5, ClassName = "Yeşil Alan", PixelCount = 22780, ColorHex = "#2ec4b6" },

                    new ProductClass { Id = 15, ProductId = 6, ClassName = "Otel Bölgesi", PixelCount = 45220, ColorHex = "#ff595e" },
                    new ProductClass { Id = 16, ProductId = 6, ClassName = "Kıyı", PixelCount = 33910, ColorHex = "#1982c4" },
                    new ProductClass { Id = 17, ProductId = 6, ClassName = "Yol", PixelCount = 20110, ColorHex = "#6c757d" },
                    new ProductClass { Id = 18, ProductId = 6, ClassName = "Yeşil Alan", PixelCount = 28440, ColorHex = "#8ac926" },

                    new ProductClass { Id = 19, ProductId = 7, ClassName = "Tarım Alanı", PixelCount = 402800, ColorHex = "#8ac926" },
                    new ProductClass { Id = 20, ProductId = 7, ClassName = "Boş Arazi", PixelCount = 95420, ColorHex = "#c2b280" },
                    new ProductClass { Id = 21, ProductId = 7, ClassName = "Sulama Kanalı", PixelCount = 11840, ColorHex = "#3a86ff" }
                );
            });
        }
    }
}
