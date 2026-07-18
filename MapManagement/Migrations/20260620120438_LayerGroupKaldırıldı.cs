using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MapManagement.Migrations
{
    /// <inheritdoc />
    public partial class LayerGroupKaldırıldı : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layers_LayerGroups_LayerGroupId",
                table: "Layers");

            migrationBuilder.DropTable(
                name: "LayerGroups");

            migrationBuilder.DropIndex(
                name: "IX_Layers_LayerGroupId",
                table: "Layers");

            migrationBuilder.DropColumn(
                name: "LayerGroupId",
                table: "Layers");

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 20, 12, 4, 37, 982, DateTimeKind.Utc).AddTicks(2816));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LayerGroupId",
                table: "Layers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "LayerGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderNo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayerGroups", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LayerGroups",
                columns: new[] { "Id", "IsDeleted", "Name", "OrderNo" },
                values: new object[] { 1L, false, "Altlık Haritalar", 1 });

            migrationBuilder.UpdateData(
                table: "Layers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "LayerGroupId" },
                values: new object[] { new DateTime(2026, 4, 18, 13, 22, 52, 962, DateTimeKind.Utc).AddTicks(6817), 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Layers_LayerGroupId",
                table: "Layers",
                column: "LayerGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Layers_LayerGroups_LayerGroupId",
                table: "Layers",
                column: "LayerGroupId",
                principalTable: "LayerGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
