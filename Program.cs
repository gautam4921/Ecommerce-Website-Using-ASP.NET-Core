using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShopApp.Data;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Database
// ------------------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ------------------------------------------------------------
// Identity
// ------------------------------------------------------------

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ------------------------------------------------------------
// MVC + Razor Pages
// ------------------------------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Database developer exception filter
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

var app = builder.Build();

// ------------------------------------------------------------
// Database migrations
// ------------------------------------------------------------
//
// Set:
// ApplyDatabaseMigrations=true
//
// in Azure App Service Development environment.
//
// This allows the Azure SQL database to be updated automatically
// when the application starts.
//
// ------------------------------------------------------------

var applyDatabaseMigrations =
    builder.Configuration.GetValue<bool>("ApplyDatabaseMigrations");

if (applyDatabaseMigrations)
{
    using var scope = app.Services.CreateScope();

    var dbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

// ------------------------------------------------------------
// HTTP pipeline
// ------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// ------------------------------------------------------------
// Routes
// ------------------------------------------------------------

app.MapControllerRoute(
    name: "areas",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
