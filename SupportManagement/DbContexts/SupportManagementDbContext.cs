using Microsoft.EntityFrameworkCore;
using SupportManagement.Entities;

namespace SupportManagement.DbContexts;

public class SupportManagementDbContext : DbContext
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

    public SupportManagementDbContext(DbContextOptions<SupportManagementDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.TicketNo).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.CustomerEmail).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Subject).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Organization).HasMaxLength(200);
            entity.Property(x => x.AssignedAdminEmail).HasMaxLength(200);

            entity.HasIndex(x => x.TicketNo).IsUnique();
        });

        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.ToTable("TicketMessages");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.SenderType).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            entity.Property(x => x.SenderEmail).HasMaxLength(200);
            entity.Property(x => x.ExternalMessageId).HasMaxLength(500);
            entity.Property(x => x.InReplyToExternalMessageId).HasMaxLength(500);
            entity.Property(x => x.Body).IsRequired();

            entity.HasOne(x => x.Ticket)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ExternalMessageId).IsUnique(false);
        });
    }
}
