using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class KatmanSiralamasiDegistirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 20, 11, 56, 52, 592, DateTimeKind.Utc).AddTicks(3266), null, null, "OSM Standart", 1, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Name", "Url" },
                values: new object[] { new DateTime(2026, 7, 20, 11, 56, 52, 592, DateTimeKind.Utc).AddTicks(3275), "Topografik Harita", "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 20, 11, 56, 52, 592, DateTimeKind.Utc).AddTicks(3276), "image/png", "Maps:blue_marble", "Uydu Görüntüsü", 2, "https://taiearth.com/geoserver/Maps/wms" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5031), "image/png", "Maps:blue_marble", "Blue Marbel", 2, "https://taiearth.com/geoserver/Maps/wms" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Name", "Url" },
                values: new object[] { new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5037), "OSM Standart", "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 19, 11, 19, 58, 171, DateTimeKind.Utc).AddTicks(5039), null, null, "Topografik Harita", 1, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });
        }
    }
}
