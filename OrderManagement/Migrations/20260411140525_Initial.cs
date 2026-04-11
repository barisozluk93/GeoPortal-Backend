using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DownloadLink = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceStr = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    Resolution = table.Column<decimal>(type: "numeric", nullable: true),
                    CloudRate = table.Column<int>(type: "integer", nullable: true),
                    AreaKm2 = table.Column<decimal>(type: "numeric", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    PreviewUrl = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsOrthorectified = table.Column<bool>(type: "boolean", nullable: true),
                    IsPansharpened = table.Column<bool>(type: "boolean", nullable: true),
                    IsClassified = table.Column<bool>(type: "boolean", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true),
                    BboxMinX = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMinY = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMaxX = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMaxY = table.Column<decimal>(type: "numeric", nullable: true),
                    SourceType = table.Column<int>(type: "integer", nullable: true),
                    SourceLabel = table.Column<string>(type: "text", nullable: true),
                    RequestHash = table.Column<string>(type: "text", nullable: true),
                    IsCustomArea = table.Column<bool>(type: "boolean", nullable: false),
                    IsInMarket = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    OrderNo = table.Column<string>(type: "text", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OrderStatus = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BasketId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceAddressId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasketProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    BasketId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketProducts_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClasses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<long>(type: "bigint", nullable: true),
                    ClassName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PixelCount = table.Column<int>(type: "integer", nullable: true),
                    ColorHex = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClasses_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductValue = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false),
                    ProccessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    OrderProductId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_OrderProducts_OrderProductId",
                        column: x => x.OrderProductId,
                        principalTable: "OrderProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiKeyId = table.Column<long>(type: "bigint", nullable: false),
                    Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeyPermissions_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AcquisitionDate", "AreaKm2", "BboxMaxX", "BboxMaxY", "BboxMinX", "BboxMinY", "CategoryId", "City", "CloudRate", "Currency", "Description", "District", "DownloadLink", "Geometry", "IsClassified", "IsCustomArea", "IsDeleted", "IsInMarket", "IsOrthorectified", "IsPansharpened", "Name", "PreviewUrl", "Price", "PriceStr", "Provider", "RequestHash", "Resolution", "SourceLabel", "SourceType", "ThumbnailUrl" },
                values: new object[,]
                {
                    { 1L, null, null, null, null, null, null, 2, null, null, "TRY", null, null, null, null, null, false, false, true, null, null, "API Key", null, 1000m, "₺1000", null, null, null, null, null, null },
                    { 2L, new DateTime(2026, 3, 12, 10, 15, 0, 0, DateTimeKind.Unspecified), 42.8m, null, null, null, null, 1, "İstanbul", 4, "TRY", "Yüksek çözünürlüklü şehir uydu görüntüsü.", "Beşiktaş", "https://example.com/downloads/product-1.zip", null, true, false, false, true, true, true, "İstanbul Avrupa Yakası Uydu Görüntüsü - Beşiktaş", "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=1200", 18500m, "₺18.500", "Maxar", null, 0.3m, null, null, "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=600" },
                    { 3L, new DateTime(2026, 2, 28, 9, 40, 0, 0, DateTimeKind.Unspecified), 87.4m, null, null, null, null, 2, "Ankara", 9, "TRY", "Tarım alanları için analiz verisi.", "Gölbaşı", "https://example.com/downloads/product-2.zip", null, true, false, false, true, true, false, "Ankara Tarım Analizi - Gölbaşı", "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=1200", 12400m, "₺12.400", "Planet", null, 0.5m, null, null, "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=600" },
                    { 4L, new DateTime(2026, 3, 5, 14, 20, 0, 0, DateTimeKind.Unspecified), 58.9m, null, null, null, null, 3, "İzmir", 6, "TRY", "Kıyı ve şehir birleşik uydu görüntüsü.", "Karşıyaka", "https://example.com/downloads/product-3.zip", null, false, false, false, true, true, true, "İzmir Kıyı Uydu Görüntüsü", "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=1200", 21800m, "₺21.800", "Airbus", null, 0.4m, null, null, "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=600" },
                    { 5L, new DateTime(2026, 1, 19, 11, 10, 0, 0, DateTimeKind.Unspecified), 65.1m, null, null, null, null, 4, "Bursa", 3, "TRY", "Sanayi alanı analiz görüntüsü.", "Nilüfer", "https://example.com/downloads/product-4.zip", null, true, false, false, true, true, true, "Bursa Sanayi Bölgesi Uydu Verisi", "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=1200", 16750m, "₺16.750", "Maxar", null, 0.3m, null, null, "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=600" },
                    { 6L, new DateTime(2026, 3, 18, 8, 30, 0, 0, DateTimeKind.Unspecified), 73.6m, null, null, null, null, 5, "Antalya", 7, "TRY", "Turizm ve kıyı analiz verisi.", "Alanya", "https://example.com/downloads/product-5.zip", null, true, false, false, true, true, false, "Antalya Kıyı Uydu Paketi", "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=1200", 14300m, "₺14.300", "Planet", null, 0.5m, null, null, "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=600" },
                    { 7L, new DateTime(2026, 2, 11, 16, 45, 0, 0, DateTimeKind.Unspecified), 120.3m, null, null, null, null, 2, "Konya", 12, "TRY", "Geniş tarım alanı gözlemi.", "Selçuklu", "https://example.com/downloads/product-6.zip", null, true, false, false, true, true, false, "Konya Tarım Uydu Görüntüsü", "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=1200", 9800m, "₺9.800", "Sentinel", null, 1.0m, null, null, "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=600" }
                });

            migrationBuilder.InsertData(
                table: "ProductClasses",
                columns: new[] { "Id", "ClassName", "ColorHex", "PixelCount", "ProductId" },
                values: new object[,]
                {
                    { 1L, "Yerleşim", "#ff6b6b", 124500, 2L },
                    { 2L, "Yol", "#ffd166", 48210, 2L },
                    { 3L, "Yeşil Alan", "#06d6a0", 38120, 2L },
                    { 4L, "Su", "#118ab2", 19340, 2L },
                    { 5L, "Tarım Alanı", "#8ac926", 211430, 3L },
                    { 6L, "Boş Arazi", "#c2b280", 73450, 3L },
                    { 7L, "Yol", "#adb5bd", 19410, 3L },
                    { 8L, "Su", "#1982c4", 16420, 3L },
                    { 9L, "Yerleşim", "#ef476f", 98400, 4L },
                    { 10L, "Su", "#3a86ff", 62450, 4L },
                    { 11L, "Liman", "#8338ec", 14200, 4L },
                    { 12L, "Sanayi", "#ff9f1c", 110230, 5L },
                    { 13L, "Yol", "#adb5bd", 36610, 5L },
                    { 14L, "Yeşil Alan", "#2ec4b6", 22780, 5L },
                    { 15L, "Otel Bölgesi", "#ff595e", 45220, 6L },
                    { 16L, "Kıyı", "#1982c4", 33910, 6L },
                    { 17L, "Yol", "#6c757d", 20110, 6L },
                    { 18L, "Yeşil Alan", "#8ac926", 28440, 6L },
                    { 19L, "Tarım Alanı", "#8ac926", 402800, 7L },
                    { 20L, "Boş Arazi", "#c2b280", 95420, 7L },
                    { 21L, "Sulama Kanalı", "#3a86ff", 11840, 7L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyPermissions_ApiKeyId",
                table: "ApiKeyPermissions",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OrderId",
                table: "ApiKeys",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OrderProductId",
                table: "ApiKeys",
                column: "OrderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_ProductId",
                table: "BasketProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductId",
                table: "OrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BasketId",
                table: "Orders",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClasses_ProductId",
                table: "ProductClasses",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyPermissions");

            migrationBuilder.DropTable(
                name: "BasketProducts");

            migrationBuilder.DropTable(
                name: "ProductClasses");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Baskets");
        }
    }
}
