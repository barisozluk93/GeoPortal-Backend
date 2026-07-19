using Microsoft.EntityFrameworkCore;
using UserManagement.Entity;

namespace UserManagement.DbContexts
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options)
        {
        }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(
                //Permission
                new Permission { Id = 1, Name = "Yetki Ekranı Sayfalama Yetkisi", Code = "PermissionScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 2, Name = "Yetki Ekranı Kayıt Yetkisi", Code = "PermissionScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 3, Name = "Yetki Ekranı Güncelleme Yetkisi", Code = "PermissionScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 4, Name = "Yetki Ekranı Silme Yetkisi", Code = "PermissionScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 5, Name = "Yetki Ekranı Görüntüleme Yetkisi", Code = "PermissionScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 6, Name = "Yetki Ekranı Listeleme Yetkisi", Code = "PermissionScene.All.Permission", IsDeleted = false, IsSystemData = true },
                //Role
                new Permission { Id = 7, Name = "Rol Ekranı Sayfalama Yetkisi", Code = "RoleScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 8, Name = "Rol Ekranı Kayıt Yetkisi", Code = "RoleScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 9, Name = "Rol Ekranı Güncelleme Yetkisi", Code = "RoleScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 10, Name = "Rol Ekranı Silme Yetkisi", Code = "RoleScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 11, Name = "Rol Ekranı Görüntüleme Yetkisi", Code = "RoleScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 12, Name = "Rol Ekranı Listeleme Yetkisi", Code = "RoleScene.All.Permission", IsDeleted = false, IsSystemData = true },
                //User
                new Permission { Id = 13, Name = "Kullanıcı Ekranı Sayfalama Yetkisi", Code = "UserScene.Paging.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 14, Name = "Kullanıcı Ekranı Kayıt Yetkisi", Code = "UserScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 15, Name = "Kullanıcı Ekranı Güncelleme Yetkisi", Code = "UserScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 16, Name = "Kullanıcı Ekranı Silme Yetkisi", Code = "UserScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 17, Name = "Kullanıcı Ekranı Görüntüleme Yetkisi", Code = "UserScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 18, Name = "Kullanıcı Ekranı Super Kullanıcı Listeleme Yetkisi", Code = "UserScene.SuperAll.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 19, Name = "Kullanıcı Ekranı Kullanıcı Yetki Listeleme Yetkisi", Code = "UserScene.GetUserPermissionList.Permission", IsDeleted = false, IsSystemData = true },
                //Profile
                new Permission { Id = 20, Name = "Profil Ekranı Şifre Değiştirme Yetkisi", Code = "ProfileScene.ChangePassword.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 21, Name = "Profil Ekranı Güncelleme Yetkisi", Code = "ProfileScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 22, Name = "Profil Ekranı Adres Listeleme Yetkisi", Code = "ProfileScene.AddressList.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 23, Name = "Profil Ekranı Adres Kayıt Yetkisi", Code = "ProfileScene.AddressSave.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 24, Name = "Profil Ekranı Adres Güncelleme Yetkisi", Code = "ProfileScene.AddressEdit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 25, Name = "Profil Ekranı Adres Silme Yetkisi", Code = "ProfileScene.AddressDelete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 26, Name = "Profil Ekranı Adres Görüntüleme Yetkisi", Code = "ProfileScene.AddressGet.Permission", IsDeleted = false, IsSystemData = true },
                //Notification
                new Permission { Id = 27, Name = "Bildirim Ekranı Listeleme Yetkisi", Code = "NotificationScene.All.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 28, Name = "Bildirim Ekranı Kayıt Yetkisi", Code = "NotificationScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 29, Name = "Bildirim Ekranı Okuma Yetkisi", Code = "NotificationScene.Read.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 30, Name = "Bildirim Ekranı Okunmamış Görüntüleme Yetkisi", Code = "NotificationScene.Unread.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 31, Name = "Bildirim Ekranı Silme Yetkisi", Code = "NotificationScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                //Order
                new Permission { Id = 32, Name = "Sipariş Ekranı Sayfalama Yetkisi", Code = "OrderScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 33, Name = "Sipariş Ekranı Güncelleme Yetkisi", Code = "OrderScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 34, Name = "Sipariş Ekranı Kayıt Yetkisi", Code = "OrderScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 35, Name = "Sipariş Ekranı Görüntüleme Yetkisi", Code = "OrderScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 36, Name = "Sipariş Ekranı Fatura Yükleme Yetkisi", Code = "OrderScene.InvoiceSave.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 37, Name = "Sipariş Ekranı Fatura Silme Yetkisi", Code = "OrderScene.InvoiceDelete.Permission", IsDeleted = false, IsSystemData = true },
                //Basket
                new Permission { Id = 38, Name = "Sepet Ekranı Listeleme Yetkisi", Code = "BasketScene.List.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 39, Name = "Sepet Ekranı Kayıt Yetkisi", Code = "BasketScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 40, Name = "Sepet Ekranı Silme Yetkisi", Code = "BasketScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                //Dashboard
                new Permission { Id = 41, Name = "Dashboard Görüntüleme Yetkisi", Code = "DashboardScene.View.Permission", IsDeleted = false , IsSystemData = true },
                //File
                new Permission { Id = 42, Name = "Dosya Ekranı Silme Yetkisi", Code = "FileScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 43, Name = "Dosya Ekranı Kayıt Yetkisi", Code = "FileScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                //Map
                new Permission { Id = 44, Name = "Harita Ekranı Katman Grubu Sayfalama Yetkisi", Code = "LayerGroupScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 45, Name = "Harita Ekranı Katman Grubu Kayıt Yetkisi", Code = "LayerGroupScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 46, Name = "Harita Ekranı Katman Grubu Güncelleme Yetkisi", Code = "LayerGroupScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 47, Name = "Harita Ekranı Katman Grubu Silme Yetkisi", Code = "LayerGroupScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 48, Name = "Harita Ekranı Katman Grubu Görüntüleme Yetkisi", Code = "LayerGroupScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 49, Name = "Harita Ekranı Katman Sayfalama Yetkisi", Code = "LayerScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 50, Name = "Harita Ekranı Katman Kayıt Yetkisi", Code = "LayerScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 51, Name = "Harita Ekranı Katman Güncelleme Yetkisi", Code = "LayerScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 52, Name = "Harita Ekranı Katman Silme Yetkisi", Code = "LayerScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 53, Name = "Harita Ekranı Katman Görüntüleme Yetkisi", Code = "LayerScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 54, Name = "Harita Ekranı Katman Grubu Listeleme Yetkisi", Code = "LayerGroupScene.All.Permission", IsDeleted = false, IsSystemData = true },
                //Order Customer
                new Permission { Id = 55, Name = "Sipariş Ekranı Müşteri Sayfalama Yetkisi", Code = "OrderScene.CustomerPaging.Permission", IsDeleted = false, IsSystemData = true },
                //Support
                new Permission { Id = 56, Name = "Destek Ekranı Sayfalama Yetkisi", Code = "SupportScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 57, Name = "Destek Ekranı Görüntüleme Yetkisi", Code = "SupportScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 58, Name = "Destek Ekranı Durum Güncelleme Yetkisi", Code = "SupportScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 59, Name = "Destek Ekranı Cevaplama Yetkisi", Code = "SupportScene.Reply.Permission", IsDeleted = false, IsSystemData = true },
                //Export
                new Permission { Id = 60, Name = "Tablo Dışa Aktarma Yetkisi", Code = "Table.Export.Permission", IsDeleted = false, IsSystemData = true },
                //Organization
                new Permission { Id = 61, Name = "Organizasyon Ekranı Listeleme Yetkisi", Code = "OrganizationScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 62, Name = "Organizasyon Ekranı Kayıt Yetkisi", Code = "OrganizationScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 63, Name = "Organizasyon Ekranı Güncelleme Yetkisi", Code = "OrganizationScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 64, Name = "Organizasyon Ekranı Silme Yetkisi", Code = "OrganizationScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 65, Name = "Organizasyon Ekranı Görüntüleme Yetkisi", Code = "OrganizationScene.Get.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 66, Name = "Yetki Ekranı Listeleme Yetkisi", Code = "OrganizationScene.All.Permission", IsDeleted = false, IsSystemData = true },
                 //Log
                new Permission { Id = 67, Name = "Log Ekranı Listeleme Yetkisi", Code = "LogScene.Paging.Permission", IsDeleted = false, IsSystemData = true }
            );


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", IsDeleted = false, IsSystemData = true },
                new Role { Id = 2, Name = "Customer", IsDeleted = false, IsSystemData = true }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                //SuperAdmin Role Perms
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, IsDeleted = false },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, IsDeleted = false },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, IsDeleted = false },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, IsDeleted = false },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, IsDeleted = false },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, IsDeleted = false },
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7, IsDeleted = false },
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8, IsDeleted = false },
                new RolePermission { Id = 9, RoleId = 1, PermissionId = 9, IsDeleted = false },
                new RolePermission { Id = 10, RoleId = 1, PermissionId = 10, IsDeleted = false },
                new RolePermission { Id = 11, RoleId = 1, PermissionId = 11, IsDeleted = false },
                new RolePermission { Id = 12, RoleId = 1, PermissionId = 12, IsDeleted = false },
                new RolePermission { Id = 13, RoleId = 1, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 14, RoleId = 1, PermissionId = 14, IsDeleted = false },
                new RolePermission { Id = 15, RoleId = 1, PermissionId = 15, IsDeleted = false },
                new RolePermission { Id = 16, RoleId = 1, PermissionId = 16, IsDeleted = false },
                new RolePermission { Id = 17, RoleId = 1, PermissionId = 17, IsDeleted = false },
                new RolePermission { Id = 18, RoleId = 1, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 19, RoleId = 1, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 20, RoleId = 1, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 21, RoleId = 1, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 22, RoleId = 1, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 23, RoleId = 1, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 24, RoleId = 1, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 25, RoleId = 1, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 26, RoleId = 1, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 27, RoleId = 1, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 28, RoleId = 1, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 29, RoleId = 1, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 30, RoleId = 1, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 31, RoleId = 1, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 32, RoleId = 1, PermissionId = 32, IsDeleted = false },
                new RolePermission { Id = 33, RoleId = 1, PermissionId = 33, IsDeleted = false },
                new RolePermission { Id = 34, RoleId = 1, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 35, RoleId = 1, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 36, RoleId = 1, PermissionId = 36, IsDeleted = false },
                new RolePermission { Id = 37, RoleId = 1, PermissionId = 37, IsDeleted = false },
                new RolePermission { Id = 38, RoleId = 1, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 39, RoleId = 1, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 40, RoleId = 1, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 41, RoleId = 1, PermissionId = 41, IsDeleted = false },
                new RolePermission { Id = 42, RoleId = 1, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 43, RoleId = 1, PermissionId = 43, IsDeleted = false },
                new RolePermission { Id = 44, RoleId = 1, PermissionId = 44, IsDeleted = false },
                new RolePermission { Id = 45, RoleId = 1, PermissionId = 45, IsDeleted = false },
                new RolePermission { Id = 46, RoleId = 1, PermissionId = 46, IsDeleted = false },
                new RolePermission { Id = 47, RoleId = 1, PermissionId = 47, IsDeleted = false },
                new RolePermission { Id = 48, RoleId = 1, PermissionId = 48, IsDeleted = false },
                new RolePermission { Id = 49, RoleId = 1, PermissionId = 49, IsDeleted = false },
                new RolePermission { Id = 50, RoleId = 1, PermissionId = 50, IsDeleted = false },
                new RolePermission { Id = 51, RoleId = 1, PermissionId = 51, IsDeleted = false },
                new RolePermission { Id = 52, RoleId = 1, PermissionId = 52, IsDeleted = false },
                new RolePermission { Id = 53, RoleId = 1, PermissionId = 53, IsDeleted = false },
                new RolePermission { Id = 54, RoleId = 1, PermissionId = 54, IsDeleted = false },
                new RolePermission { Id = 55, RoleId = 1, PermissionId = 56, IsDeleted = false },
                new RolePermission { Id = 56, RoleId = 1, PermissionId = 57, IsDeleted = false },
                new RolePermission { Id = 57, RoleId = 1, PermissionId = 58, IsDeleted = false },
                new RolePermission { Id = 58, RoleId = 1, PermissionId = 59, IsDeleted = false },
                new RolePermission { Id = 59, RoleId = 1, PermissionId = 60, IsDeleted = false },
                new RolePermission { Id = 83, RoleId = 1, PermissionId = 61, IsDeleted = false },
                new RolePermission { Id = 84, RoleId = 1, PermissionId = 62, IsDeleted = false },
                new RolePermission { Id = 85, RoleId = 1, PermissionId = 63, IsDeleted = false },
                new RolePermission { Id = 86, RoleId = 1, PermissionId = 64, IsDeleted = false },
                new RolePermission { Id = 87, RoleId = 1, PermissionId = 65, IsDeleted = false },
                new RolePermission { Id = 88, RoleId = 1, PermissionId = 66, IsDeleted = false },
                new RolePermission { Id = 89, RoleId = 1, PermissionId = 67, IsDeleted = false },
                //Customer Role Perms
                new RolePermission { Id = 60, RoleId = 2, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 61, RoleId = 2, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 62, RoleId = 2, PermissionId = 21, IsDeleted = false },
                new RolePermission { Id = 63, RoleId = 2, PermissionId = 22, IsDeleted = false },
                new RolePermission { Id = 64, RoleId = 2, PermissionId = 23, IsDeleted = false },
                new RolePermission { Id = 65, RoleId = 2, PermissionId = 24, IsDeleted = false },
                new RolePermission { Id = 66, RoleId = 2, PermissionId = 25, IsDeleted = false },
                new RolePermission { Id = 67, RoleId = 2, PermissionId = 26, IsDeleted = false },
                new RolePermission { Id = 68, RoleId = 2, PermissionId = 27, IsDeleted = false },
                new RolePermission { Id = 69, RoleId = 2, PermissionId = 28, IsDeleted = false },
                new RolePermission { Id = 70, RoleId = 2, PermissionId = 29, IsDeleted = false },
                new RolePermission { Id = 71, RoleId = 2, PermissionId = 30, IsDeleted = false },
                new RolePermission { Id = 72, RoleId = 2, PermissionId = 31, IsDeleted = false },
                new RolePermission { Id = 73, RoleId = 2, PermissionId = 34, IsDeleted = false },
                new RolePermission { Id = 74, RoleId = 2, PermissionId = 35, IsDeleted = false },
                new RolePermission { Id = 75, RoleId = 2, PermissionId = 38, IsDeleted = false },
                new RolePermission { Id = 76, RoleId = 2, PermissionId = 39, IsDeleted = false },
                new RolePermission { Id = 77, RoleId = 2, PermissionId = 40, IsDeleted = false },
                new RolePermission { Id = 78, RoleId = 2, PermissionId = 42, IsDeleted = false },
                new RolePermission { Id = 79, RoleId = 2, PermissionId = 43, IsDeleted = false },
                new RolePermission { Id = 80, RoleId = 2, PermissionId = 17, IsDeleted = false },
                new RolePermission { Id = 81, RoleId = 2, PermissionId = 55, IsDeleted = false },
                new RolePermission { Id = 82, RoleId = 2, PermissionId = 18, IsDeleted = false }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "SuperAdmin", Surname = "SuperAdmin", Email = "super@test.com", 
                        Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", 
                        Phone = "+905077352772", Username = "superadmin", Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="), IsDeleted = false, IsSystemData = true },
                new User
                {
                    Id = 2,
                    Name = "Customer",
                    Surname = "Customer",
                    Email = "customer@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "customer",
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                //SuperAdmin User Role
                new UserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 1,
                    IsDeleted = false
                },
                //Customer User Role
                new UserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = 2,
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<UserPermission>().HasData(
                //SuperAdmin User Permissions
                new UserPermission { Id = 1, UserId = 1, PermissionId = 1, IsDeleted = false },
                new UserPermission { Id = 2, UserId = 1, PermissionId = 2, IsDeleted = false },
                new UserPermission { Id = 3, UserId = 1, PermissionId = 3, IsDeleted = false },
                new UserPermission { Id = 4, UserId = 1, PermissionId = 4, IsDeleted = false },
                new UserPermission { Id = 5, UserId = 1, PermissionId = 5, IsDeleted = false },
                new UserPermission { Id = 6, UserId = 1, PermissionId = 6, IsDeleted = false },
                new UserPermission { Id = 7, UserId = 1, PermissionId = 7, IsDeleted = false },
                new UserPermission { Id = 8, UserId = 1, PermissionId = 8, IsDeleted = false },
                new UserPermission { Id = 9, UserId = 1, PermissionId = 9, IsDeleted = false },
                new UserPermission { Id = 10, UserId = 1, PermissionId = 10, IsDeleted = false },
                new UserPermission { Id = 11, UserId = 1, PermissionId = 11, IsDeleted = false },
                new UserPermission { Id = 12, UserId = 1, PermissionId = 12, IsDeleted = false },
                new UserPermission { Id = 13, UserId = 1, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 14, UserId = 1, PermissionId = 14, IsDeleted = false },
                new UserPermission { Id = 15, UserId = 1, PermissionId = 15, IsDeleted = false },
                new UserPermission { Id = 16, UserId = 1, PermissionId = 16, IsDeleted = false },
                new UserPermission { Id = 17, UserId = 1, PermissionId = 17, IsDeleted = false },
                new UserPermission { Id = 18, UserId = 1, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 19, UserId = 1, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 20, UserId = 1, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 21, UserId = 1, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 22, UserId = 1, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 23, UserId = 1, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 24, UserId = 1, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 25, UserId = 1, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 26, UserId = 1, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 27, UserId = 1, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 28, UserId = 1, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 29, UserId = 1, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 30, UserId = 1, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 31, UserId = 1, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 32, UserId = 1, PermissionId = 32, IsDeleted = false },
                new UserPermission { Id = 33, UserId = 1, PermissionId = 33, IsDeleted = false },
                new UserPermission { Id = 34, UserId = 1, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 35, UserId = 1, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 36, UserId = 1, PermissionId = 36, IsDeleted = false },
                new UserPermission { Id = 37, UserId = 1, PermissionId = 37, IsDeleted = false },
                new UserPermission { Id = 38, UserId = 1, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 39, UserId = 1, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 40, UserId = 1, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 41, UserId = 1, PermissionId = 41, IsDeleted = false },
                new UserPermission { Id = 42, UserId = 1, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 43, UserId = 1, PermissionId = 43, IsDeleted = false },
                new UserPermission { Id = 44, UserId = 1, PermissionId = 44, IsDeleted = false },
                new UserPermission { Id = 45, UserId = 1, PermissionId = 45, IsDeleted = false },
                new UserPermission { Id = 46, UserId = 1, PermissionId = 46, IsDeleted = false },
                new UserPermission { Id = 47, UserId = 1, PermissionId = 47, IsDeleted = false },
                new UserPermission { Id = 48, UserId = 1, PermissionId = 48, IsDeleted = false },
                new UserPermission { Id = 49, UserId = 1, PermissionId = 49, IsDeleted = false },
                new UserPermission { Id = 50, UserId = 1, PermissionId = 50, IsDeleted = false },
                new UserPermission { Id = 51, UserId = 1, PermissionId = 51, IsDeleted = false },
                new UserPermission { Id = 52, UserId = 1, PermissionId = 52, IsDeleted = false },
                new UserPermission { Id = 53, UserId = 1, PermissionId = 53, IsDeleted = false },
                new UserPermission { Id = 54, UserId = 1, PermissionId = 54, IsDeleted = false },
                new UserPermission { Id = 55, UserId = 1, PermissionId = 56, IsDeleted = false },
                new UserPermission { Id = 56, UserId = 1, PermissionId = 57, IsDeleted = false },
                new UserPermission { Id = 57, UserId = 1, PermissionId = 58, IsDeleted = false },
                new UserPermission { Id = 58, UserId = 1, PermissionId = 59, IsDeleted = false },
                new UserPermission { Id = 59, UserId = 1, PermissionId = 60, IsDeleted = false },
                new UserPermission { Id = 83, UserId = 1, PermissionId = 61, IsDeleted = false },
                new UserPermission { Id = 84, UserId = 1, PermissionId = 62, IsDeleted = false },
                new UserPermission { Id = 85, UserId = 1, PermissionId = 63, IsDeleted = false },
                new UserPermission { Id = 86, UserId = 1, PermissionId = 64, IsDeleted = false },
                new UserPermission { Id = 87, UserId = 1, PermissionId = 65, IsDeleted = false },
                new UserPermission { Id = 88, UserId = 1, PermissionId = 66, IsDeleted = false },
                new UserPermission { Id = 89, UserId = 1, PermissionId = 67, IsDeleted = false },
                //Customer User Permissions
                new UserPermission { Id = 60, UserId = 2, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 61, UserId = 2, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 62, UserId = 2, PermissionId = 21, IsDeleted = false },
                new UserPermission { Id = 63, UserId = 2, PermissionId = 22, IsDeleted = false },
                new UserPermission { Id = 64, UserId = 2, PermissionId = 23, IsDeleted = false },
                new UserPermission { Id = 65, UserId = 2, PermissionId = 24, IsDeleted = false },
                new UserPermission { Id = 66, UserId = 2, PermissionId = 25, IsDeleted = false },
                new UserPermission { Id = 67, UserId = 2, PermissionId = 26, IsDeleted = false },
                new UserPermission { Id = 68, UserId = 2, PermissionId = 27, IsDeleted = false },
                new UserPermission { Id = 69, UserId = 2, PermissionId = 28, IsDeleted = false },
                new UserPermission { Id = 70, UserId = 2, PermissionId = 29, IsDeleted = false },
                new UserPermission { Id = 71, UserId = 2, PermissionId = 30, IsDeleted = false },
                new UserPermission { Id = 72, UserId = 2, PermissionId = 31, IsDeleted = false },
                new UserPermission { Id = 73, UserId = 2, PermissionId = 34, IsDeleted = false },
                new UserPermission { Id = 74, UserId = 2, PermissionId = 35, IsDeleted = false },
                new UserPermission { Id = 75, UserId = 2, PermissionId = 38, IsDeleted = false },
                new UserPermission { Id = 76, UserId = 2, PermissionId = 39, IsDeleted = false },
                new UserPermission { Id = 77, UserId = 2, PermissionId = 40, IsDeleted = false },
                new UserPermission { Id = 78, UserId = 2, PermissionId = 42, IsDeleted = false },
                new UserPermission { Id = 79, UserId = 2, PermissionId = 43, IsDeleted = false },
                new UserPermission { Id = 80, UserId = 2, PermissionId = 17, IsDeleted = false },
                new UserPermission { Id = 81, UserId = 2, PermissionId = 55, IsDeleted = false },
                new UserPermission { Id = 82, UserId = 2, PermissionId = 18, IsDeleted = false }

            );
        }

    }
}
