using System;
using System.Collections.Generic;
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

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Announcem__Categ__5070F446");

            entity.HasOne(d => d.SubCategoryNavigation).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.SubCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Announcem__SubCa__5165187F");
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

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.AnnouncementSubCategories)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategories_Category");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
