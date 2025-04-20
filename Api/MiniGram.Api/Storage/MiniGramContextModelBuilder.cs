using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Storage.Entities;

namespace MiniGram.Api.Storage;

public class MiniGramContextModelBuilder
{
    public static void Build(ModelBuilder builder)
    {
        // User
        builder.Entity<User>(o =>
        {
            o.HasKey(p => p.UserId);
            o.Property(p => p.UserId);
            o.Property(p => p.Email).HasMaxLength(60);
            o.HasIndex(p => p.Email).IsUnique();
            o.Property(p => p.Username).HasMaxLength(30);
            o.HasIndex(p => p.Username).IsUnique();
        });

        // Credentials
        builder.Entity<Credentials>(o =>
        {
            o.HasKey(p => p.UserId);
            o.Property(p => p.UserId).ValueGeneratedNever();
            o.Property(p => p.Password).HasMaxLength(400);
        });

        // Photo
        builder.Entity<Photo>(o =>
        {
            o.HasKey(p => p.Id);
            o.Property(p => p.Contents).HasMaxLength(1024 * 1024 * 5); // 5 MB
            o.Property(p => p.Description).HasMaxLength(500);
        });

        builder.Entity<Photo>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment
        builder.Entity<Comment>(o =>
        {
            o.HasKey(p => p.Id);
            o.Property(p => p.Id).ValueGeneratedOnAdd();
            o.Property(p => p.Text).HasMaxLength(500);
            o.Property(p => p.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<Comment>()
            .HasOne(p => p.Photo)
            .WithMany(p => p.Comments)
            .HasForeignKey(p => p.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
