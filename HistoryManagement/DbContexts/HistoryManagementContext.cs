using HistoryManagement.Entities;
using Microsoft.EntityFrameworkCore;
namespace HistoryManagement.DbContexts;
public sealed class HistoryManagementContext(DbContextOptions<HistoryManagementContext> options) : DbContext(options)
{
    public DbSet<HistoryRecord> Histories => Set<HistoryRecord>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<HistoryRecord>();
        e.HasKey(x => x.Id);
        e.Property(x => x.RecordId).HasMaxLength(250).IsRequired();
        e.Property(x => x.EntityType).HasMaxLength(250).IsRequired();
        e.Property(x => x.OperationType).HasMaxLength(50).IsRequired();
        e.Property(x => x.ServiceName).HasMaxLength(150).IsRequired();
        e.Property(x => x.UserId).HasMaxLength(250);
        e.Property(x => x.UserName).HasMaxLength(250);
        e.Property(x => x.ChangesJson).HasColumnType("jsonb");
        e.HasIndex(x => new { x.EntityType, x.RecordId, x.CreatedDate });
        e.HasIndex(x => x.ServiceName);
        e.HasIndex(x => x.OperationType);
        base.OnModelCreating(modelBuilder);
    }
}
