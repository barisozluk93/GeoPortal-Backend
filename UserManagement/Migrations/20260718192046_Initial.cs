using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TaxNo = table.Column<string>(type: "text", nullable: false),
                    TaxOffice = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: true),
                    Sector = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    AddressHeader = table.Column<string>(type: "text", nullable: false),
                    InvoiceType = table.Column<long>(type: "bigint", nullable: false),
                    VKN = table.Column<string>(type: "text", nullable: true),
                    VergiDairesi = table.Column<string>(type: "text", nullable: true),
                    FirmaAdi = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "IsDeleted", "IsSystemData", "Name" },
                values: new object[,]
                {
                    { 1L, "PermissionScene.Paging.Permission", false, true, "Yetki Ekranı Sayfalama Yetkisi" },
                    { 2L, "PermissionScene.Save.Permission", false, true, "Yetki Ekranı Kayıt Yetkisi" },
                    { 3L, "PermissionScene.Edit.Permission", false, true, "Yetki Ekranı Güncelleme Yetkisi" },
                    { 4L, "PermissionScene.Delete.Permission", false, true, "Yetki Ekranı Silme Yetkisi" },
                    { 5L, "PermissionScene.Get.Permission", false, true, "Yetki Ekranı Görüntüleme Yetkisi" },
                    { 6L, "PermissionScene.All.Permission", false, true, "Yetki Ekranı Listeleme Yetkisi" },
                    { 7L, "RoleScene.Paging.Permission", false, true, "Rol Ekranı Sayfalama Yetkisi" },
                    { 8L, "RoleScene.Save.Permission", false, true, "Rol Ekranı Kayıt Yetkisi" },
                    { 9L, "RoleScene.Edit.Permission", false, true, "Rol Ekranı Güncelleme Yetkisi" },
                    { 10L, "RoleScene.Delete.Permission", false, true, "Rol Ekranı Silme Yetkisi" },
                    { 11L, "RoleScene.Get.Permission", false, true, "Rol Ekranı Görüntüleme Yetkisi" },
                    { 12L, "RoleScene.All.Permission", false, true, "Rol Ekranı Listeleme Yetkisi" },
                    { 13L, "UserScene.Paging.Permission", false, true, "Kullanıcı Ekranı Sayfalama Yetkisi" },
                    { 14L, "UserScene.Save.Permission", false, true, "Kullanıcı Ekranı Kayıt Yetkisi" },
                    { 15L, "UserScene.Edit.Permission", false, true, "Kullanıcı Ekranı Güncelleme Yetkisi" },
                    { 16L, "UserScene.Delete.Permission", false, true, "Kullanıcı Ekranı Silme Yetkisi" },
                    { 17L, "UserScene.Get.Permission", false, true, "Kullanıcı Ekranı Görüntüleme Yetkisi" },
                    { 18L, "UserScene.SuperAll.Permission", false, true, "Kullanıcı Ekranı Super Kullanıcı Listeleme Yetkisi" },
                    { 19L, "UserScene.GetUserPermissionList.Permission", false, true, "Kullanıcı Ekranı Kullanıcı Yetki Listeleme Yetkisi" },
                    { 20L, "ProfileScene.ChangePassword.Permission", false, true, "Profil Ekranı Şifre Değiştirme Yetkisi" },
                    { 21L, "ProfileScene.Edit.Permission", false, true, "Profil Ekranı Güncelleme Yetkisi" },
                    { 22L, "ProfileScene.AddressList.Permission", false, true, "Profil Ekranı Adres Listeleme Yetkisi" },
                    { 23L, "ProfileScene.AddressSave.Permission", false, true, "Profil Ekranı Adres Kayıt Yetkisi" },
                    { 24L, "ProfileScene.AddressEdit.Permission", false, true, "Profil Ekranı Adres Güncelleme Yetkisi" },
                    { 25L, "ProfileScene.AddressDelete.Permission", false, true, "Profil Ekranı Adres Silme Yetkisi" },
                    { 26L, "ProfileScene.AddressGet.Permission", false, true, "Profil Ekranı Adres Görüntüleme Yetkisi" },
                    { 27L, "NotificationScene.All.Permission", false, true, "Bildirim Ekranı Listeleme Yetkisi" },
                    { 28L, "NotificationScene.Save.Permission", false, true, "Bildirim Ekranı Kayıt Yetkisi" },
                    { 29L, "NotificationScene.Read.Permission", false, true, "Bildirim Ekranı Okuma Yetkisi" },
                    { 30L, "NotificationScene.Unread.Permission", false, true, "Bildirim Ekranı Okunmamış Görüntüleme Yetkisi" },
                    { 31L, "NotificationScene.Delete.Permission", false, true, "Bildirim Ekranı Silme Yetkisi" },
                    { 32L, "OrderScene.Paging.Permission", false, true, "Sipariş Ekranı Sayfalama Yetkisi" },
                    { 33L, "OrderScene.Edit.Permission", false, true, "Sipariş Ekranı Güncelleme Yetkisi" },
                    { 34L, "OrderScene.Save.Permission", false, true, "Sipariş Ekranı Kayıt Yetkisi" },
                    { 35L, "OrderScene.Get.Permission", false, true, "Sipariş Ekranı Görüntüleme Yetkisi" },
                    { 36L, "OrderScene.InvoiceSave.Permission", false, true, "Sipariş Ekranı Fatura Yükleme Yetkisi" },
                    { 37L, "OrderScene.InvoiceDelete.Permission", false, true, "Sipariş Ekranı Fatura Silme Yetkisi" },
                    { 38L, "BasketScene.List.Permission", false, true, "Sepet Ekranı Listeleme Yetkisi" },
                    { 39L, "BasketScene.Save.Permission", false, true, "Sepet Ekranı Kayıt Yetkisi" },
                    { 40L, "BasketScene.Delete.Permission", false, true, "Sepet Ekranı Silme Yetkisi" },
                    { 41L, "DashboardScene.View.Permission", false, true, "Dashboard Görüntüleme Yetkisi" },
                    { 42L, "FileScene.Delete.Permission", false, true, "Dosya Ekranı Silme Yetkisi" },
                    { 43L, "FileScene.Save.Permission", false, true, "Dosya Ekranı Kayıt Yetkisi" },
                    { 44L, "LayerGroupScene.Paging.Permission", false, true, "Harita Ekranı Katman Grubu Sayfalama Yetkisi" },
                    { 45L, "LayerGroupScene.Save.Permission", false, true, "Harita Ekranı Katman Grubu Kayıt Yetkisi" },
                    { 46L, "LayerGroupScene.Edit.Permission", false, true, "Harita Ekranı Katman Grubu Güncelleme Yetkisi" },
                    { 47L, "LayerGroupScene.Delete.Permission", false, true, "Harita Ekranı Katman Grubu Silme Yetkisi" },
                    { 48L, "LayerGroupScene.Get.Permission", false, true, "Harita Ekranı Katman Grubu Görüntüleme Yetkisi" },
                    { 49L, "LayerScene.Paging.Permission", false, true, "Harita Ekranı Katman Sayfalama Yetkisi" },
                    { 50L, "LayerScene.Save.Permission", false, true, "Harita Ekranı Katman Kayıt Yetkisi" },
                    { 51L, "LayerScene.Edit.Permission", false, true, "Harita Ekranı Katman Güncelleme Yetkisi" },
                    { 52L, "LayerScene.Delete.Permission", false, true, "Harita Ekranı Katman Silme Yetkisi" },
                    { 53L, "LayerScene.Get.Permission", false, true, "Harita Ekranı Katman Görüntüleme Yetkisi" },
                    { 54L, "LayerGroupScene.All.Permission", false, true, "Harita Ekranı Katman Grubu Listeleme Yetkisi" },
                    { 55L, "OrderScene.CustomerPaging.Permission", false, true, "Sipariş Ekranı Müşteri Sayfalama Yetkisi" },
                    { 56L, "SupportScene.Paging.Permission", false, true, "Destek Ekranı Sayfalama Yetkisi" },
                    { 57L, "SupportScene.Get.Permission", false, true, "Destek Ekranı Görüntüleme Yetkisi" },
                    { 58L, "SupportScene.Edit.Permission", false, true, "Destek Ekranı Durum Güncelleme Yetkisi" },
                    { 59L, "SupportScene.Reply.Permission", false, true, "Destek Ekranı Cevaplama Yetkisi" },
                    { 60L, "Table.Export.Permission", false, true, "Tablo Dışa Aktarma Yetkisi" },
                    { 61L, "OrganizationScene.Paging.Permission", false, true, "Organizasyon Ekranı Listeleme Yetkisi" },
                    { 62L, "OrganizationScene.Save.Permission", false, true, "Organizasyon Ekranı Kayıt Yetkisi" },
                    { 63L, "OrganizationScene.Edit.Permission", false, true, "Organizasyon Ekranı Güncelleme Yetkisi" },
                    { 64L, "OrganizationScene.Delete.Permission", false, true, "Organizasyon Ekranı Silme Yetkisi" },
                    { 65L, "OrganizationScene.Get.Permission", false, true, "Organizasyon Ekranı Görüntüleme Yetkisi" },
                    { 66L, "OrganizationScene.All.Permission", false, true, "Yetki Ekranı Listeleme Yetkisi" },
                    { 67L, "LogScene.Paging.Permission", false, true, "Log Ekranı Listeleme Yetkisi" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "IsSystemData", "Name" },
                values: new object[,]
                {
                    { 1L, false, true, "SuperAdmin" },
                    { 2L, false, true, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FileId", "IsDeleted", "IsSystemData", "Name", "Password", "Phone", "Salt", "Sector", "Surname", "Username" },
                values: new object[,]
                {
                    { 1L, "super@test.com", null, false, true, "SuperAdmin", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, null, "SuperAdmin", "superadmin" },
                    { 2L, "customer@test.com", null, false, true, "Customer", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, null, "Customer", "customer" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 1L },
                    { 3L, false, 3L, 1L },
                    { 4L, false, 4L, 1L },
                    { 5L, false, 5L, 1L },
                    { 6L, false, 6L, 1L },
                    { 7L, false, 7L, 1L },
                    { 8L, false, 8L, 1L },
                    { 9L, false, 9L, 1L },
                    { 10L, false, 10L, 1L },
                    { 11L, false, 11L, 1L },
                    { 12L, false, 12L, 1L },
                    { 13L, false, 13L, 1L },
                    { 14L, false, 14L, 1L },
                    { 15L, false, 15L, 1L },
                    { 16L, false, 16L, 1L },
                    { 17L, false, 17L, 1L },
                    { 18L, false, 18L, 1L },
                    { 19L, false, 19L, 1L },
                    { 20L, false, 20L, 1L },
                    { 21L, false, 21L, 1L },
                    { 22L, false, 22L, 1L },
                    { 23L, false, 23L, 1L },
                    { 24L, false, 24L, 1L },
                    { 25L, false, 25L, 1L },
                    { 26L, false, 26L, 1L },
                    { 27L, false, 27L, 1L },
                    { 28L, false, 28L, 1L },
                    { 29L, false, 29L, 1L },
                    { 30L, false, 30L, 1L },
                    { 31L, false, 31L, 1L },
                    { 32L, false, 32L, 1L },
                    { 33L, false, 33L, 1L },
                    { 34L, false, 34L, 1L },
                    { 35L, false, 35L, 1L },
                    { 36L, false, 36L, 1L },
                    { 37L, false, 37L, 1L },
                    { 38L, false, 38L, 1L },
                    { 39L, false, 39L, 1L },
                    { 40L, false, 40L, 1L },
                    { 41L, false, 41L, 1L },
                    { 42L, false, 42L, 1L },
                    { 43L, false, 43L, 1L },
                    { 44L, false, 44L, 1L },
                    { 45L, false, 45L, 1L },
                    { 46L, false, 46L, 1L },
                    { 47L, false, 47L, 1L },
                    { 48L, false, 48L, 1L },
                    { 49L, false, 49L, 1L },
                    { 50L, false, 50L, 1L },
                    { 51L, false, 51L, 1L },
                    { 52L, false, 52L, 1L },
                    { 53L, false, 53L, 1L },
                    { 54L, false, 54L, 1L },
                    { 55L, false, 56L, 1L },
                    { 56L, false, 57L, 1L },
                    { 57L, false, 58L, 1L },
                    { 58L, false, 59L, 1L },
                    { 59L, false, 60L, 1L },
                    { 60L, false, 19L, 2L },
                    { 61L, false, 20L, 2L },
                    { 62L, false, 21L, 2L },
                    { 63L, false, 22L, 2L },
                    { 64L, false, 23L, 2L },
                    { 65L, false, 24L, 2L },
                    { 66L, false, 25L, 2L },
                    { 67L, false, 26L, 2L },
                    { 68L, false, 27L, 2L },
                    { 69L, false, 28L, 2L },
                    { 70L, false, 29L, 2L },
                    { 71L, false, 30L, 2L },
                    { 72L, false, 31L, 2L },
                    { 73L, false, 34L, 2L },
                    { 74L, false, 35L, 2L },
                    { 75L, false, 38L, 2L },
                    { 76L, false, 39L, 2L },
                    { 77L, false, 40L, 2L },
                    { 78L, false, 42L, 2L },
                    { 79L, false, 43L, 2L },
                    { 80L, false, 17L, 2L },
                    { 81L, false, 55L, 2L },
                    { 82L, false, 18L, 2L },
                    { 83L, false, 61L, 1L },
                    { 84L, false, 62L, 1L },
                    { 85L, false, 63L, 1L },
                    { 86L, false, 64L, 1L },
                    { 87L, false, 65L, 1L },
                    { 88L, false, 66L, 1L },
                    { 89L, false, 67L, 1L }
                });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "UserId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 1L },
                    { 3L, false, 3L, 1L },
                    { 4L, false, 4L, 1L },
                    { 5L, false, 5L, 1L },
                    { 6L, false, 6L, 1L },
                    { 7L, false, 7L, 1L },
                    { 8L, false, 8L, 1L },
                    { 9L, false, 9L, 1L },
                    { 10L, false, 10L, 1L },
                    { 11L, false, 11L, 1L },
                    { 12L, false, 12L, 1L },
                    { 13L, false, 13L, 1L },
                    { 14L, false, 14L, 1L },
                    { 15L, false, 15L, 1L },
                    { 16L, false, 16L, 1L },
                    { 17L, false, 17L, 1L },
                    { 18L, false, 18L, 1L },
                    { 19L, false, 19L, 1L },
                    { 20L, false, 20L, 1L },
                    { 21L, false, 21L, 1L },
                    { 22L, false, 22L, 1L },
                    { 23L, false, 23L, 1L },
                    { 24L, false, 24L, 1L },
                    { 25L, false, 25L, 1L },
                    { 26L, false, 26L, 1L },
                    { 27L, false, 27L, 1L },
                    { 28L, false, 28L, 1L },
                    { 29L, false, 29L, 1L },
                    { 30L, false, 30L, 1L },
                    { 31L, false, 31L, 1L },
                    { 32L, false, 32L, 1L },
                    { 33L, false, 33L, 1L },
                    { 34L, false, 34L, 1L },
                    { 35L, false, 35L, 1L },
                    { 36L, false, 36L, 1L },
                    { 37L, false, 37L, 1L },
                    { 38L, false, 38L, 1L },
                    { 39L, false, 39L, 1L },
                    { 40L, false, 40L, 1L },
                    { 41L, false, 41L, 1L },
                    { 42L, false, 42L, 1L },
                    { 43L, false, 43L, 1L },
                    { 44L, false, 44L, 1L },
                    { 45L, false, 45L, 1L },
                    { 46L, false, 46L, 1L },
                    { 47L, false, 47L, 1L },
                    { 48L, false, 48L, 1L },
                    { 49L, false, 49L, 1L },
                    { 50L, false, 50L, 1L },
                    { 51L, false, 51L, 1L },
                    { 52L, false, 52L, 1L },
                    { 53L, false, 53L, 1L },
                    { 54L, false, 54L, 1L },
                    { 55L, false, 56L, 1L },
                    { 56L, false, 57L, 1L },
                    { 57L, false, 58L, 1L },
                    { 58L, false, 59L, 1L },
                    { 59L, false, 60L, 1L },
                    { 60L, false, 19L, 2L },
                    { 61L, false, 20L, 2L },
                    { 62L, false, 21L, 2L },
                    { 63L, false, 22L, 2L },
                    { 64L, false, 23L, 2L },
                    { 65L, false, 24L, 2L },
                    { 66L, false, 25L, 2L },
                    { 67L, false, 26L, 2L },
                    { 68L, false, 27L, 2L },
                    { 69L, false, 28L, 2L },
                    { 70L, false, 29L, 2L },
                    { 71L, false, 30L, 2L },
                    { 72L, false, 31L, 2L },
                    { 73L, false, 34L, 2L },
                    { 74L, false, 35L, 2L },
                    { 75L, false, 38L, 2L },
                    { 76L, false, 39L, 2L },
                    { 77L, false, 40L, 2L },
                    { 78L, false, 42L, 2L },
                    { 79L, false, 43L, 2L },
                    { 80L, false, 17L, 2L },
                    { 81L, false, 55L, 2L },
                    { 82L, false, 18L, 2L },
                    { 83L, false, 61L, 1L },
                    { 84L, false, 62L, 1L },
                    { 85L, false, 63L, 1L },
                    { 86L, false, 64L, 1L },
                    { 87L, false, 65L, 1L },
                    { 88L, false, 66L, 1L },
                    { 89L, false, 67L, 1L }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "IsDeleted", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 2L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_UserId",
                table: "ApplicationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_OrganizationId",
                table: "OrganizationUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "OrganizationUsers");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
