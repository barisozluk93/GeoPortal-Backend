using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

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
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    FootprintPath = table.Column<string>(type: "text", nullable: true),
                    GeoTiffPath = table.Column<string>(type: "text", nullable: true),
                    PreviewPath = table.Column<string>(type: "text", nullable: true),
                    QuicklookPath = table.Column<string>(type: "text", nullable: true),
                    MetadataPath = table.Column<string>(type: "text", nullable: true),
                    DownloadLink = table.Column<string>(type: "text", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true),
                    BboxMinX = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMinY = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMaxX = table.Column<decimal>(type: "numeric", nullable: true),
                    BboxMaxY = table.Column<decimal>(type: "numeric", nullable: true),
                    AreaKm2 = table.Column<decimal>(type: "numeric", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AcquisitionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AcquisitionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Resolution = table.Column<decimal>(type: "numeric", nullable: true),
                    CloudRate = table.Column<decimal>(type: "numeric", nullable: true),
                    SunElevation = table.Column<decimal>(type: "numeric", nullable: true),
                    SunAzimuth = table.Column<decimal>(type: "numeric", nullable: true),
                    OffNadirAngle = table.Column<decimal>(type: "numeric", nullable: true),
                    Satellite = table.Column<string>(type: "text", nullable: true),
                    Sensor = table.Column<string>(type: "text", nullable: true),
                    ProcessingLevel = table.Column<string>(type: "text", nullable: true),
                    SourceLabel = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<string>(type: "text", nullable: true),
                    StripId = table.Column<string>(type: "text", nullable: true),
                    CatalogId = table.Column<string>(type: "text", nullable: true),
                    ImageDescriptor = table.Column<string>(type: "text", nullable: true),
                    ImageType = table.Column<string>(type: "text", nullable: true),
                    SensorMode = table.Column<string>(type: "text", nullable: true),
                    BandId = table.Column<string>(type: "text", nullable: true),
                    ProductType = table.Column<string>(type: "text", nullable: true),
                    ProductLevel = table.Column<string>(type: "text", nullable: true),
                    RadiometricLevel = table.Column<string>(type: "text", nullable: true),
                    OutputFormat = table.Column<string>(type: "text", nullable: true),
                    SpatialReference = table.Column<string>(type: "text", nullable: true),
                    ScanDirection = table.Column<string>(type: "text", nullable: true),
                    DataOwner = table.Column<string>(type: "text", nullable: true),
                    IsOrthorectified = table.Column<bool>(type: "boolean", nullable: true),
                    IsNVDIAnalysis = table.Column<bool>(type: "boolean", nullable: true),
                    IsPansharpened = table.Column<bool>(type: "boolean", nullable: true),
                    IsClassified = table.Column<bool>(type: "boolean", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    PreviewUrl = table.Column<string>(type: "text", nullable: true),
                    MetadataUrl = table.Column<string>(type: "text", nullable: true),
                    PropertyUrl = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceStr = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsInMarket = table.Column<bool>(type: "boolean", nullable: false),
                    IsCustomArea = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<string>(type: "text", nullable: true)
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
                    NumberOf = table.Column<int>(type: "integer", nullable: false),
                    AoiId = table.Column<string>(type: "text", nullable: true),
                    AoiName = table.Column<string>(type: "text", nullable: true),
                    AoiWkt = table.Column<string>(type: "text", nullable: true),
                    RequestWkt = table.Column<string>(type: "text", nullable: true),
                    RequestHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IntersectionWkt = table.Column<string>(type: "text", nullable: true),
                    RequestAreaKm2 = table.Column<double>(type: "double precision", nullable: false),
                    UnitPrice = table.Column<double>(type: "double precision", nullable: false),
                    BaseTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    ProcessingOptionsJson = table.Column<string>(type: "jsonb", nullable: false),
                    ProcessingTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    CalculatedTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    ItemType = table.Column<string>(type: "text", nullable: false),
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
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: true),
                    PixelCount = table.Column<int>(type: "integer", nullable: true),
                    ColorHex = table.Column<string>(type: "text", nullable: true)
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
                    CompletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NumberOf = table.Column<int>(type: "integer", nullable: false),
                    AoiId = table.Column<string>(type: "text", nullable: true),
                    AoiName = table.Column<string>(type: "text", nullable: true),
                    AoiWkt = table.Column<string>(type: "text", nullable: true),
                    RequestWkt = table.Column<string>(type: "text", nullable: true),
                    IntersectionWkt = table.Column<string>(type: "text", nullable: true),
                    RequestAreaKm2 = table.Column<double>(type: "double precision", nullable: false),
                    UnitPrice = table.Column<double>(type: "double precision", nullable: false),
                    BaseTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    ProcessingOptionsJson = table.Column<string>(type: "jsonb", nullable: false),
                    ProcessingTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    CalculatedTotalPrice = table.Column<double>(type: "double precision", nullable: false),
                    ItemType = table.Column<string>(type: "text", nullable: false)
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
                    Key = table.Column<string>(type: "text", nullable: false),
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
                    Endpoint = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyPermissions_ApiKeyId",
                table: "ApiKeyPermissions",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OrderId",
                table: "ApiKeys",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OrderProductId",
                table: "ApiKeys",
                column: "OrderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId_ProductId_AoiId_RequestHash",
                table: "BasketProducts",
                columns: new[] { "BasketId", "ProductId", "AoiId", "RequestHash" });

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
