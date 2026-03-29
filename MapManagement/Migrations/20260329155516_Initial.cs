using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LayerGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayerGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LayerGroupId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IsBaseMap = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Layers_LayerGroups_LayerGroupId",
                        column: x => x.LayerGroupId,
                        principalTable: "LayerGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LayerGroups",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1L, false, "Altlık Haritalar" },
                    { 2L, false, "Uydu Görüntüleri" }
                });

            migrationBuilder.InsertData(
                table: "Layers",
                columns: new[] { "Id", "IsBaseMap", "IsDeleted", "LayerGroupId", "Name", "Price", "Url" },
                values: new object[,]
                {
                    { 1L, true, false, 1L, "OSM Standart", null, "https://{a-c}.tile.openstreetmap.org/{z}/{x}/{y}.png" },
                    { 2L, true, false, 1L, "OSM Hot", null, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" },
                    { 3L, false, false, 2L, "Uydu Görüntüsü 1", 1000.0, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" },
                    { 4L, false, false, 2L, "Uydu Görüntüsü 2", 1500.0, "https://{a-c}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Layers_LayerGroupId",
                table: "Layers",
                column: "LayerGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropTable(
                name: "LayerGroups");
        }
    }
}
