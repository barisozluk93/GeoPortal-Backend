using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class IndexKaldırıldı : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_BasketId_ProductId_AoiId_RequestWkt",
                table: "BasketProducts");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts",
                column: "BasketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BasketProducts_BasketId",
                table: "BasketProducts");

            migrationBuilder.CreateIndex(
                name: "IX_BasketProducts_BasketId_ProductId_AoiId_RequestWkt",
                table: "BasketProducts",
                columns: new[] { "BasketId", "ProductId", "AoiId", "RequestWkt" });
        }
    }
}
