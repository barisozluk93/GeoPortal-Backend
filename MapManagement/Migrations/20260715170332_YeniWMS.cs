using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class YeniWMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 15, 17, 3, 31, 987, DateTimeKind.Utc).AddTicks(7667), "image/png", "Maps:blue_marble", "Blue Marbel", 2, "https://taiearth.com/geoserver/Maps/wms" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Name", "OrderNo", "Url" },
                values: new object[] { new DateTime(2026, 7, 15, 17, 3, 31, 987, DateTimeKind.Utc).AddTicks(7676), "OSM Standart", 2, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsDeleted", "IsVisible", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[] { 3L, new DateTime(2026, 7, 15, 17, 3, 31, 987, DateTimeKind.Utc).AddTicks(7677), null, false, false, null, "Topografik Harita", 1.0, 3, 1, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Format", "LayerName", "Name", "Type", "Url" },
                values: new object[] { new DateTime(2026, 7, 11, 18, 11, 29, 711, DateTimeKind.Utc).AddTicks(6645), null, null, "OSM Standart", 1, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Name", "OrderNo", "Url" },
                values: new object[] { new DateTime(2026, 7, 11, 18, 11, 29, 711, DateTimeKind.Utc).AddTicks(6653), "Topografik Harita", 3, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });
        }
    }
}
