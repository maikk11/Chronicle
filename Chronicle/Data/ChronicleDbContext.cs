using Microsoft.EntityFrameworkCore;
using Chronicle.Models.Domain;
namespace Chronicle.Data;
public class ChronicleDbContext : DbContext
{
    public ChronicleDbContext(DbContextOptions<ChronicleDbContext> options) : base(options){}
    public DbSet<Article> Articles {get;set;} = null!;
    public DbSet<Category> Categories {get;set;} = null!;
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Deleting a category must not delete its articles: the admin has to
        // reassign or remove them first.
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}