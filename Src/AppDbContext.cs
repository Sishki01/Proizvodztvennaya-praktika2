using Microsoft.EntityFrameworkCore;
using ImageApi.Models;

namespace ImageApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ImageRecord> Images => Set<ImageRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(255);
            e.Property(x => x.Type).IsRequired().HasMaxLength(10);
            e.Property(x => x.FilePath).IsRequired().HasMaxLength(1024);
        });
    }
}
