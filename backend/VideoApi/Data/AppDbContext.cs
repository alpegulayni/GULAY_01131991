using Microsoft.EntityFrameworkCore;
using VideoApi.Models;

namespace VideoApi.Data;

/// <summary>
/// Entity Framework Core database context for the video application.  This context
/// defines the tables and relationships used by the API.  The database is
/// configured in Program.cs to use SQLite by default, but may be swapped for
/// any provider supported by EF Core.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Video> Videos => Set<Video>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<VideoCategory> VideoCategories => Set<VideoCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite primary key for the join table
        modelBuilder.Entity<VideoCategory>()
            .HasKey(vc => new { vc.VideoId, vc.CategoryId });

        modelBuilder.Entity<VideoCategory>()
            .HasOne(vc => vc.Video)
            .WithMany(v => v.VideoCategories)
            .HasForeignKey(vc => vc.VideoId);

        modelBuilder.Entity<VideoCategory>()
            .HasOne(vc => vc.Category)
            .WithMany(c => c.VideoCategories)
            .HasForeignKey(vc => vc.CategoryId);

        // Ensure category names are unique
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}