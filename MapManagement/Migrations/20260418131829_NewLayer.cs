using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class NewLayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LayerGroups",
                keyColumn: "Id",
                keyValue: 1L,
                column: "OrderNo",
                value: 1);

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "CreatedAt", "Format", "IsDeleted", "IsVisible", "LayerGroupId", "LayerName", "Name", "Opacity", "OrderNo", "Type", "Url", "Version" },
                values: new object[] { 1L, new DateTime(2026, 4, 18, 13, 18, 28, 51, DateTimeKind.Utc).AddTicks(9650), null, false, true, 1L, null, "OSM Standart", 1.0, 1, 0, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.UpdateData(
                table: "LayerGroups",
                keyColumn: "Id",
                keyValue: 1L,
                column: "OrderNo",
                value: 0);
        }
    }
}
