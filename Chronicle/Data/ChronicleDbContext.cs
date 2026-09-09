using Microsoft.EntityFrameworkCore;
using Chronicle.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Chronicle.Data;
public class ChronicleDbContext : IdentityDbContext<IdentityUser>
{
    public ChronicleDbContext(DbContextOptions<ChronicleDbContext> options) : base(options){}
    public DbSet<Article> Articles {get;set;} = null!;
    public DbSet<Category> Categories {get;set;} = null!;
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "politics" },
            new Category { Id = 2, Name = "economy" },
            new Category { Id = 3, Name = "food&drink" },
            new Category { Id = 4, Name = "sport" },
            new Category { Id = 5, Name = "entertainment" },
            new Category { Id = 6, Name = "tech" }
        );

        modelBuilder.Entity<Article>()
            .Property(a => a.CategoryId)
            .IsRequired(false);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}