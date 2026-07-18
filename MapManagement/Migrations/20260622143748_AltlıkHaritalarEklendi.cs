using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class AltlıkHaritalarEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8676));

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsDeleted", "IsVisible", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[,]
                {
                    { 2L, new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8682), null, false, false, null, "Yükseklik Haritası", 1.0, 2, 1, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png", null },
                    { 3L, new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8683), null, false, false, null, "Topografik Harita", 1.0, 3, 1, "https://tile.opentopomap.org/{z}/{x}/{y}.png", null },
                    { 4L, new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8684), null, false, false, null, "Gece Haritası", 1.0, 4, 1, "https://basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 20, 12, 4, 37, 982, DateTimeKind.Utc).AddTicks(2816));
        }
    }
}
