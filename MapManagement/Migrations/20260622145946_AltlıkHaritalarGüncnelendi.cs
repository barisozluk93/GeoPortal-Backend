using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class AltlıkHaritalarGüncnelendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2718));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2724), "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2725), "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2727), "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8676));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8682), "https://{a-c}.tile.opentopomap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8683), "https://tile.opentopomap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 37, 47, 767, DateTimeKind.Utc).AddTicks(8684), "https://basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" });
        }
    }
}
