using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class OrderProductEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingDate",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "TrackingNo",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "OrderProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TrackingDate",
                table: "OrderProducts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNo",
                table: "OrderProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VendorId",
                table: "OrderProducts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
