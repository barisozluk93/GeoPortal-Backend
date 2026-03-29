using FileManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace FileManagement.DbContexts

{
    public class FileManagementContext : DbContext
    {
        public FileManagementContext(DbContextOptions<FileManagementContext> options) : base(options)
        {
        }

        public DbSet<Entity.File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
