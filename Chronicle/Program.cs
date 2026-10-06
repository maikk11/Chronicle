using Microsoft.EntityFrameworkCore;
using Chronicle.Data;
using Microsoft.AspNetCore.Identity;
using Chronicle.Repositories;
using Chronicle.Services;
using Chonicle.Repositories;
using Chonicle.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Read once, with an explicit message if it is missing. The value is not in the
// repository: it is supplied through user secrets or environment variables, and
// appsettings.json carries a localhost default so a fresh clone starts.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is not configured. See the README for how to set it.");

// The server version is pinned rather than detected. AutoDetect opens a
// connection at startup, so an unreachable database stopped the application
// from starting at all instead of failing on the pages that need it.
builder.Services.AddDbContext<ChronicleDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ChronicleDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<IArticleImageRepository, ArticleImageRepository>();
builder.Services.AddScoped<IImageService, SupabaseImageService>();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICareerRequestRepository, CareerRequestRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICareerRequestService, CareerRequestService>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();