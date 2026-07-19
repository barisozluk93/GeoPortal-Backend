using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    LayerName = table.Column<string>(type: "text", nullable: true),
                    Format = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    Opacity = table.Column<double>(type: "double precision", nullable: false),
                    OrderNo = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsDeleted", "IsVisible", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5031), "image/png", false, true, "Maps:blue_marble", "Blue Marbel", 1.0, 1, 2, "https://taiearth.com/geoserver/Maps/wms", null },
                    { 2L, new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5037), null, false, false, null, "OSM Standart", 1.0, 2, 1, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", null },
                    { 3L, new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5039), null, false, false, null, "Topografik Harita", 1.0, 3, 1, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Layers");
        }
    }
}
