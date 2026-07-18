using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class TopografikHaritaVeGeceHaritasıKaldırılı : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                value: new DateTime(2026, 7, 6, 13, 55, 35, 627, DateTimeKind.Utc).AddTicks(1697));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 6, 13, 55, 35, 627, DateTimeKind.Utc).AddTicks(1702));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6182));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6188));

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsDeleted", "IsVisible", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[,]
                {
                    { 3L, new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6190), null, false, false, null, "Topografik Harita", 1.0, 3, 1, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png", null },
                    { 4L, new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6191), null, false, false, null, "Gece Haritası", 1.0, 4, 1, "https://basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png", null }
                });
        }
    }
}
