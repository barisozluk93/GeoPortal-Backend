using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class NewProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DownloadLink",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcquisitionDate",
                table: "Products",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AreaKm2",
                table: "Products",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Products",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CloudRate",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Products",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Products",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClassified",
                table: "Products",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrthorectified",
                table: "Products",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPansharpened",
                table: "Products",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewUrl",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceStr",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "Products",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Resolution",
                table: "Products",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

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

            migrationBuilder.InsertData(
                table: "ProductClasses",
                columns: new[] { "Id", "ClassName", "ColorHex", "PixelCount", "ProductId" },
                values: new object[,]
                {
                    { 5L, "Tarım Alanı", "#8ac926", 211430, 3L },
                    { 6L, "Boş Arazi", "#c2b280", 73450, 3L },
                    { 7L, "Yol", "#adb5bd", 19410, 3L },
                    { 8L, "Su", "#1982c4", 16420, 3L },
                    { 9L, "Yerleşim", "#ef476f", 98400, 4L },
                    { 10L, "Su", "#3a86ff", 62450, 4L },
                    { 11L, "Liman", "#8338ec", 14200, 4L }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "AcquisitionDate", "AreaKm2", "CategoryId", "City", "CloudRate", "Currency", "Description", "District", "IsClassified", "IsOrthorectified", "IsPansharpened", "PreviewUrl", "Price", "PriceStr", "Provider", "Resolution", "ThumbnailUrl" },
                values: new object[] { null, null, 2, null, null, "TRY", null, null, null, null, null, null, 1000m, "₺1000", null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "AcquisitionDate", "AreaKm2", "CategoryId", "City", "CloudRate", "Currency", "Description", "District", "DownloadLink", "IsClassified", "IsOrthorectified", "IsPansharpened", "Name", "PreviewUrl", "Price", "PriceStr", "Provider", "Resolution", "ThumbnailUrl" },
                values: new object[] { new DateTime(2026, 2, 28, 9, 40, 0, 0, DateTimeKind.Unspecified), 87.4m, 2, "Ankara", 9, "TRY", "Tarım alanları için analiz verisi.", "Gölbaşı", "https://example.com/downloads/product-2.zip", true, true, false, "Ankara Tarım Analizi - Gölbaşı", "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=1200", 12400m, "₺12.400", "Planet", 0.5m, "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=600" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "AcquisitionDate", "AreaKm2", "CategoryId", "City", "CloudRate", "Currency", "Description", "District", "DownloadLink", "IsClassified", "IsOrthorectified", "IsPansharpened", "Name", "PreviewUrl", "Price", "PriceStr", "Provider", "Resolution", "ThumbnailUrl" },
                values: new object[] { new DateTime(2026, 3, 5, 14, 20, 0, 0, DateTimeKind.Unspecified), 58.9m, 3, "İzmir", 6, "TRY", "Kıyı ve şehir birleşik uydu görüntüsü.", "Karşıyaka", "https://example.com/downloads/product-3.zip", false, true, true, "İzmir Kıyı Uydu Görüntüsü", "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=1200", 21800m, "₺21.800", "Airbus", 0.4m, "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=600" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AcquisitionDate", "AreaKm2", "CategoryId", "City", "CloudRate", "Currency", "Description", "District", "DownloadLink", "IsClassified", "IsDeleted", "IsOrthorectified", "IsPansharpened", "Name", "PreviewUrl", "Price", "PriceStr", "Provider", "Resolution", "ThumbnailUrl" },
                values: new object[,]
                {
                    { 2L, new DateTime(2026, 3, 12, 10, 15, 0, 0, DateTimeKind.Unspecified), 42.8m, 1, "İstanbul", 4, "TRY", "Yüksek çözünürlüklü şehir uydu görüntüsü.", "Beşiktaş", "https://example.com/downloads/product-1.zip", true, false, true, true, "İstanbul Avrupa Yakası Uydu Görüntüsü - Beşiktaş", "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=1200", 18500m, "₺18.500", "Maxar", 0.3m, "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=600" },
                    { 5L, new DateTime(2026, 1, 19, 11, 10, 0, 0, DateTimeKind.Unspecified), 65.1m, 4, "Bursa", 3, "TRY", "Sanayi alanı analiz görüntüsü.", "Nilüfer", "https://example.com/downloads/product-4.zip", true, false, true, true, "Bursa Sanayi Bölgesi Uydu Verisi", "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=1200", 16750m, "₺16.750", "Maxar", 0.3m, "https://images.unsplash.com/photo-1470115636492-6d2b56f9146d?w=600" },
                    { 6L, new DateTime(2026, 3, 18, 8, 30, 0, 0, DateTimeKind.Unspecified), 73.6m, 5, "Antalya", 7, "TRY", "Turizm ve kıyı analiz verisi.", "Alanya", "https://example.com/downloads/product-5.zip", true, false, true, false, "Antalya Kıyı Uydu Paketi", "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=1200", 14300m, "₺14.300", "Planet", 0.5m, "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=600" },
                    { 7L, new DateTime(2026, 2, 11, 16, 45, 0, 0, DateTimeKind.Unspecified), 120.3m, 2, "Konya", 12, "TRY", "Geniş tarım alanı gözlemi.", "Selçuklu", "https://example.com/downloads/product-6.zip", true, false, true, false, "Konya Tarım Uydu Görüntüsü", "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=1200", 9800m, "₺9.800", "Sentinel", 1.0m, "https://images.unsplash.com/photo-1469474968028-56623f02e42e?w=600" }
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
                name: "IX_ProductClasses_ProductId",
                table: "ProductClasses",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductClasses");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DropColumn(
                name: "AcquisitionDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AreaKm2",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CloudRate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsClassified",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsOrthorectified",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPansharpened",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PreviewUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceStr",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Products");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Products",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "DownloadLink",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "Products",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CategoryId", "Price" },
                values: new object[] { 1L, 1000.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CategoryId", "DownloadLink", "Name", "Price" },
                values: new object[] { 2L, null, "Uydu Görüntüsü 1", 1000.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CategoryId", "DownloadLink", "Name", "Price" },
                values: new object[] { 2L, null, "Uydu Görüntüsü 2", 1000.0 });
        }
    }
}
