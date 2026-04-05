using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class LayerEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Layers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Layers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Layers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LayerName",
                table: "Layers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Opacity",
                table: "Layers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "OrderNo",
                table: "Layers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Layers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Layers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "Format", "IsVisible", "LayerName", "Opacity", "OrderNo", "Type", "Version" },
                values: new object[] { new DateTime(2026, 4, 1, 19, 22, 33, 570, DateTimeKind.Utc).AddTicks(4632), null, true, null, 1.0, 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "Format", "IsVisible", "LayerName", "Opacity", "OrderNo", "Type", "Version" },
                values: new object[] { new DateTime(2026, 4, 1, 19, 22, 33, 570, DateTimeKind.Utc).AddTicks(4639), null, true, null, 1.0, 0, 0, null });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "Format", "IsVisible", "LayerName", "Opacity", "OrderNo", "Type", "Version" },
                values: new object[] { new DateTime(2026, 4, 1, 19, 22, 33, 570, DateTimeKind.Utc).AddTicks(4640), null, true, null, 1.0, 0, 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "LayerName",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "Opacity",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Layers");
        }
    }
}
