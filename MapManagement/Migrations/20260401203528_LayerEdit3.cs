using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class LayerEdit3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DropColumn(
                name: "IsBaseMap",
                table: "Layers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBaseMap",
                table: "Layers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsBaseMap", "IsDeleted", "IsVisible", "LayerGroupId", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 4, 1, 19, 58, 28, 973, DateTimeKind.Utc).AddTicks(5249), null, true, false, true, 1L, null, "Carto Light", 1.0, 0, 0, "https://{a-d}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}.png", null },
                    { 2L, new DateTime(2026, 4, 1, 19, 58, 28, 973, DateTimeKind.Utc).AddTicks(5255), null, true, false, true, 1L, null, "OSM Standart", 1.0, 0, 0, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", null },
                    { 3L, new DateTime(2026, 4, 1, 19, 58, 28, 973, DateTimeKind.Utc).AddTicks(5256), null, true, false, true, 1L, null, "OSM Hot", 1.0, 0, 0, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png", null }
                });
        }
    }
}
