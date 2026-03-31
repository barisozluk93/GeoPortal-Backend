using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class EditLayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Name", "Url" },
                values: new object[] { "Carto Light", "https://{a-d}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Name", "Url" },
                values: new object[] { "OSM Standart", "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "IsBaseMap", "IsDeleted", "LayerGroupId", "Name", "Price", "Url" },
                values: new object[] { 3L, true, false, 1L, "OSM Hot", null, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Name", "Url" },
                values: new object[] { "OSM Standart", "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Name", "Url" },
                values: new object[] { "OSM Hot", "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" });
        }
    }
}
