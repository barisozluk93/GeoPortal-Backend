using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class NewProductPropertyPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Products");
        }
    }
}
