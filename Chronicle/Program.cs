using Microsoft.EntityFrameworkCore;
using Chronicle.Data;
using Chronicle.Models.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ChronicleDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // Open a connection at startup of application. In production will need to be replaced with explicit version.
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// At the first startup of application insert categories for the articles.
// At the first startup of the application, insert the categories for the articles.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChronicleDbContext>();
    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { Name = "News" },
            new Category { Name = "Politics" },
            new Category { Name = "Economy" },
            new Category { Name = "World" },
            new Category { Name = "Culture" },
            new Category { Name = "Entertainment" },
            new Category { Name = "Sport" },
            new Category { Name = "Technology" },
            new Category { Name = "Health" },
            new Category { Name = "Environment" },
            new Category { Name = "Lifestyle" }
        );

        db.SaveChanges();
    }
}

app.Run();
