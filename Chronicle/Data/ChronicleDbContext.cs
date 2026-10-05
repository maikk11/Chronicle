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
    public DbSet<Image> Images {get;set;} = null!;
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
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "a71bdca4-500b-4bd4-9a40-4ce084883d6a",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1f8c2d90-0001-4a1a-9c3e-aaaa00000001"
            },
            new IdentityRole
            {
                Id = "c577002b-a010-449e-990c-99c0d10c1d1a",
                Name = "Revisor",
                NormalizedName = "REVISOR",
                ConcurrencyStamp = "1f8c2d90-0002-4a1a-9c3e-aaaa00000002"
            },
            new IdentityRole
            {
                Id = "b845423f-422d-4235-9f6b-76f2d22d2f2d",
                Name = "Writer",
                NormalizedName = "WRITER",
                ConcurrencyStamp = "1f8c2d90-0003-4a1a-9c3e-aaaa00000003"
            }
        );
        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = "e3b0c442-98fc-4c14-9afb-f4c8996fb924",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEDp/kcQLbkjSpcGPMCe53mSXVhjieDabahZbX0fvZYwxdV7HUUmQVQ8hjedPpGvaJw==",
                SecurityStamp = "1f8c2d90-0004-4a1a-9c3e-aaaa00000004",
                ConcurrencyStamp = "1f8c2d90-0005-4a1a-9c3e-aaaa00000005"
            }
        );
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = "e3b0c442-98fc-4c14-9afb-f4c8996fb924",
                RoleId = "a71bdca4-500b-4bd4-9a40-4ce084883d6a"
            }
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
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Image)
            .WithOne(i => i.Article)
            .HasForeignKey<Image>(i => i.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}