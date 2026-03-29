using Microsoft.EntityFrameworkCore;
using NotificationManagement.Entity;

namespace NotificationManagement.DbContexts
{
    public class NotificationManagementContext : DbContext
    {
        public NotificationManagementContext(DbContextOptions<NotificationManagementContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Type).IsRequired();
                entity.Property(x => x.Title).IsRequired();
                entity.Property(x => x.Body).IsRequired();

                entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
            });

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Platform).IsRequired();
                entity.Property(x => x.PushToken).IsRequired();

                entity.HasIndex(x => new { x.UserId, x.PushToken }).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}