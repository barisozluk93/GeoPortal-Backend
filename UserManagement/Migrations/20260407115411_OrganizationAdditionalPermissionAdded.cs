using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationAdditionalPermissionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "IsDeleted", "IsSystemData", "Name" },
                values: new object[] { 66L, "OrganizationScene.All.Permission", false, true, "Yetki Ekranı Listeleme Yetkisi" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[] { 88L, false, 66L, 1L });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "UserId" },
                values: new object[] { 88L, false, 66L, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 88L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 66L);
        }
    }
}
