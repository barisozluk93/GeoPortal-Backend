using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class YeniIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts");

            migrationBuilder.AddColumn<string>(
                name: "RequestHash",
                table: "BasketProducts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId_ProductId_AoiId_RequestHash",
                table: "BasketProducts",
                columns: new[] { "BasketId", "ProductId", "AoiId", "RequestHash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_BasketId_ProductId_AoiId_RequestHash",
                table: "BasketProducts");

            migrationBuilder.DropColumn(
                name: "RequestHash",
                table: "BasketProducts");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts",
                column: "BasketId");
        }
    }
}
