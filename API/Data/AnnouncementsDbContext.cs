using AnnouncementBoard.Models;
using Microsoft.EntityFrameworkCore;

namespace AnnouncementBoard.Data;

public partial class AnnouncementsDbContext : DbContext
{
    public AnnouncementsDbContext()
    {
    }

    public AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AnnouncementCategory> AnnouncementCategories { get; set; }

    public virtual DbSet<AnnouncementSubCategory> AnnouncementSubCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Announce__3214EC072C2738FD");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.SubCategory).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<AnnouncementCategory>(entity =>
        {
            entity.HasKey(e => e.Category).HasName("PK__Announce__4BB73C33D335C4FA");

            entity.Property(e => e.Category).HasMaxLength(50);
        });

        modelBuilder.Entity<AnnouncementSubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategory).HasName("PK__Announce__527FD52008F1A0C2");

            entity.Property(e => e.SubCategory).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
