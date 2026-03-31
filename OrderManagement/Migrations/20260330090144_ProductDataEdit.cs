using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class ProductDataEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: -1L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CategoryId", "Name" },
                values: new object[] { 1L, "API Key" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "DownloadLink", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { 3L, 2L, null, false, "Uydu Görüntüsü 1", 1000.0 },
                    { 4L, 2L, null, false, "Uydu Görüntüsü 2", 1000.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CategoryId", "Name" },
                values: new object[] { 2L, "Uydu Görüntüsü 1" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "DownloadLink", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { -1L, 1L, null, false, "API Key", 1000.0 },
                    { 2L, 2L, null, false, "Uydu Görüntüsü 2", 1000.0 }
                });
        }
    }
}
