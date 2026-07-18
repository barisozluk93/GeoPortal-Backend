using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class AltlıkHaritalarGüncnelendi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6190));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 15, 1, 26, 930, DateTimeKind.Utc).AddTicks(6191), "https://basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2724));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2725));

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "Url" },
                values: new object[] { new DateTime(2026, 6, 22, 14, 59, 46, 234, DateTimeKind.Utc).AddTicks(2727), "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png" });
        }
    }
}
