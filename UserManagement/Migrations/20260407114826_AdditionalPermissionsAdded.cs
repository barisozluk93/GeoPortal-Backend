using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalPermissionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "IsDeleted", "IsSystemData", "Name" },
                values: new object[,]
                {
                    { 61L, "OrganizationScene.Paging.Permission", false, true, "Organizasyon Ekranı Listeleme Yetkisi" },
                    { 62L, "OrganizationScene.Save.Permission", false, true, "Organizasyon Ekranı Kayıt Yetkisi" },
                    { 63L, "OrganizationScene.Edit.Permission", false, true, "Organizasyon Ekranı Güncelleme Yetkisi" },
                    { 64L, "OrganizationScene.Delete.Permission", false, true, "Organizasyon Ekranı Silme Yetkisi" },
                    { 65L, "OrganizationScene.Get.Permission", false, true, "Organizasyon Ekranı Görüntüleme Yetkisi" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 83L, false, 61L, 1L },
                    { 84L, false, 62L, 1L },
                    { 85L, false, 63L, 1L },
                    { 86L, false, 64L, 1L },
                    { 87L, false, 65L, 1L }
                });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "UserId" },
                values: new object[,]
                {
                    { 83L, false, 61L, 1L },
                    { 84L, false, 62L, 1L },
                    { 85L, false, 63L, 1L },
                    { 86L, false, 64L, 1L },
                    { 87L, false, 65L, 1L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 83L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 84L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 85L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 86L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 87L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 61L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 62L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 63L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 64L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 65L);
        }
    }
}
