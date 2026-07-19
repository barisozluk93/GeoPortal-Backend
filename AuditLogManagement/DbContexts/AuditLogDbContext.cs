using AuditLogManagement.Entities;
using Microsoft.EntityFrameworkCore;
namespace AuditLogManagement.DbContexts;
public sealed class AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : DbContext(options)
{
    public DbSet<AuditLogRecord> AuditLogs => Set<AuditLogRecord>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<AuditLogRecord>();
        e.HasKey(x => x.Id);
        e.Property(x => x.ServiceName).HasMaxLength(100).IsRequired();
        e.Property(x => x.ActionType).HasMaxLength(100).IsRequired();
        e.Property(x => x.HttpMethod).HasMaxLength(10).IsRequired();
        e.Property(x => x.Path).HasMaxLength(2000).IsRequired();
        e.Property(x => x.CorrelationId).HasMaxLength(200).IsRequired();
        e.HasIndex(x => x.TimestampUtc);
        e.HasIndex(x => x.UserId);
        e.HasIndex(x => x.ServiceName);
        e.HasIndex(x => x.ActionType);
        e.HasIndex(x => x.CorrelationId);
    }
}
