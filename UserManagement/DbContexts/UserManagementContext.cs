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
                new Permission { Id = 1, Name = "Yetki Ekranı Listeleme Yetkisi", Code = "PermissionScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 2, Name = "Yetki Ekranı Kayıt Yetkisi", Code = "PermissionScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 3, Name = "Yetki Ekranı Güncelleme Yetkisi", Code = "PermissionScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 4, Name = "Yetki Ekranı Silme Yetkisi", Code = "PermissionScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 5, Name = "Rol Ekranı Listeleme Yetkisi", Code = "RoleScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 6, Name = "Rol Ekranı Kayıt Yetkisi", Code = "RoleScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 7, Name = "Rol Ekranı Güncelleme Yetkisi", Code = "RoleScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 8, Name = "Rol Ekranı Silme Yetkisi", Code = "RoleScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 9, Name = "Organizasyon Ekranı Listeleme Yetkisi", Code = "OrganizationScene.Paging.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 10, Name = "Organizasyon Ekranı Kayıt Yetkisi", Code = "OrganizationScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 11, Name = "Organizasyon Ekranı Güncelleme Yetkisi", Code = "OrganizationScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 12, Name = "Organizasyon Ekranı Silme Yetkisi", Code = "OrganizationScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 13, Name = "Kullanıcı Ekranı Listeleme Yetkisi", Code = "UserScene.Paging.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 14, Name = "Kullanıcı Ekranı Kayıt Yetkisi", Code = "UserScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 15, Name = "Kullanıcı Ekranı Güncelleme Yetkisi", Code = "UserScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 16, Name = "Kullanıcı Ekranı Silme Yetkisi", Code = "UserScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 17, Name = "Dashboard Görüntüleme Yetkisi", Code = "DashboardScene.View.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 18, Name = "Dosya Ekranı Silme Yetkisi", Code = "FileScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 19, Name = "Dosya Ekranı Kayıt Yetkisi", Code = "FileScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 20, Name = "Sipariş Ekranı Listeleme Yetkisi", Code = "OrderScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 21, Name = "Harita Ekranı Katman Listeleme Yetkisi", Code = "MapScene.LayerGroupList.Permission", IsDeleted = false, IsSystemData = true }

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
                new RolePermission { Id = 25, RoleId = 1, PermissionId = 21, IsDeleted = false },
                //Customer Role Perms
                new RolePermission { Id = 21, RoleId = 2, PermissionId = 17, IsDeleted = false },
                new RolePermission { Id = 22, RoleId = 2, PermissionId = 18, IsDeleted = false },
                new RolePermission { Id = 23, RoleId = 2, PermissionId = 19, IsDeleted = false },
                new RolePermission { Id = 24, RoleId = 2, PermissionId = 20, IsDeleted = false },
                new RolePermission { Id = 26, RoleId = 2, PermissionId = 21, IsDeleted = false }
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
                new UserPermission { Id = 25, UserId = 1, PermissionId = 21, IsDeleted = false },
                //Customer User Permissions
                new UserPermission { Id = 21, UserId = 2, PermissionId = 17, IsDeleted = false },
                new UserPermission { Id = 22, UserId = 2, PermissionId = 18, IsDeleted = false },
                new UserPermission { Id = 23, UserId = 2, PermissionId = 19, IsDeleted = false },
                new UserPermission { Id = 24, UserId = 2, PermissionId = 20, IsDeleted = false },
                new UserPermission { Id = 26, UserId = 2, PermissionId = 21, IsDeleted = false }
            );
        }

    }
}
