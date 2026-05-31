using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class LayerDataEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2026, 4, 18, 13, 22, 52, 962, DateTimeKind.Utc).AddTicks(6817), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Type" },
                values: new object[] { new DateTime(2026, 4, 18, 13, 18, 28, 51, DateTimeKind.Utc).AddTicks(9650), 0 });
        }
    }
}
