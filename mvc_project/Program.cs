using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using mvc_project.Models;


var builder = WebApplication.CreateBuilder(args);

// 1. Servisler
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ⚠️ Cookie ayarları her zaman Build()'den önce yapılmalı
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// 2. Seed işlemi (await ile!)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);  // ✅ await eklendi
}

// 3. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 4. Route ayarları
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "Category/{action=Index}/{id?}",
    defaults: new { controller = "Category" });

app.MapControllerRoute(
    name: "product",
    pattern: "Product/{action=Index}/{id?}",
    defaults: new { controller = "Product" });

app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}/{id?}",
    defaults: new { controller = "Account" });

app.Run();
