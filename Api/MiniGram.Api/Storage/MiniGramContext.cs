using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Storage.Entities;

namespace MiniGram.Api.Storage;

public class MiniGramContext(DbContextOptions<MiniGramContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Credentials> Credentials { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MiniGramContextModelBuilder.Build(modelBuilder);
    }
}
