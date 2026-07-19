using System;
using HistoryManagement.DbContexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable
namespace HistoryManagement.Migrations;
[DbContext(typeof(HistoryManagementContext))]
[Migration("20260719130000_Initial")]
public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Histories",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RecordId = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                EntityType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                OperationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                ServiceName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                UserId = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                UserName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ChangesJson = table.Column<string>(type: "jsonb", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Histories", x => x.Id));
        migrationBuilder.CreateIndex(name: "IX_Histories_EntityType_RecordId_CreatedDate", table: "Histories", columns: new[] { "EntityType", "RecordId", "CreatedDate" });
        migrationBuilder.CreateIndex(name: "IX_Histories_OperationType", table: "Histories", column: "OperationType");
        migrationBuilder.CreateIndex(name: "IX_Histories_ServiceName", table: "Histories", column: "ServiceName");
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Histories");
}
