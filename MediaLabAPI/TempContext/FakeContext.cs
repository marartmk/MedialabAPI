using System;
using System.Collections.Generic;
using MediaLabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.TempContext;

public partial class FakeContext : DbContext
{
    public FakeContext(DbContextOptions<FakeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SysUser> SysUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SysUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SYS_User__3214EC07369BD957");

            entity.ToTable("SYS_Users");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessLevel).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
