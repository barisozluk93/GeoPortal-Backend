using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class AltlıkGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 11, 18, 11, 29, 711, DateTimeKind.Utc).AddTicks(6645));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Name", "OrderNo", "Url" },
                values: new object[] { new DateTime(2026, 7, 11, 18, 11, 29, 711, DateTimeKind.Utc).AddTicks(6653), "Topografik Harita", 3, "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "CreatedAt", "Name", "OrderNo", "Url" },
                values: new object[] { new DateTime(2026, 7, 6, 13, 55, 35, 627, DateTimeKind.Utc).AddTicks(1702), "Yükseklik Haritası", 2, "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png" });
        }
    }
}
