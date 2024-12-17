using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserLike>()
            .HasKey(x=> new {x.SourceUserId, x.TargetUserId});

        builder.Entity<UserLike>()
            .HasOne(x => x.SourceUser)
            .WithMany(l =>l.LikedUsers)
            .HasForeignKey(s =>s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(x => x.TargetUser)
            .WithMany(l =>l.LikedByUsers)
            .HasForeignKey(s =>s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
