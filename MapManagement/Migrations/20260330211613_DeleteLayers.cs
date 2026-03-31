using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class DeleteLayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "LayerGroups",
                keyColumn: "Id",
                keyValue: 2L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LayerGroups",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[] { 2L, false, "Uydu Görüntüleri" });

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "IsBaseMap", "IsDeleted", "LayerGroupId", "Name", "Price", "Url" },
                values: new object[,]
                {
                    { 3L, false, false, 2L, "Uydu Görüntüsü 1", 1000.0, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" },
                    { 4L, false, false, 2L, "Uydu Görüntüsü 2", 1500.0, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" }
                });
        }
    }
}
