using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoApi.Data;
using VideoApi.Models;

namespace VideoApi.Migrations
{
    /// <summary>
    /// Snapshot of the application's model used by EF Core to determine schema differences when creating subsequent migrations.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
                entity.ToTable("Categories");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Description);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.ThumbnailPath).IsRequired();
                entity.ToTable("Videos");
            });

            modelBuilder.Entity<VideoCategory>(entity =>
            {
                entity.HasKey(e => new { e.VideoId, e.CategoryId });
                entity.HasIndex(e => e.CategoryId);
                entity.ToTable("VideoCategory");
                entity.HasOne<Category>().WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne<Video>().WithMany().HasForeignKey(e => e.VideoId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}