using kartverket2025.Data;
using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using kartverket2025.Repositories;
using kartverket2025.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(60) // Set timeout to 60 seconds
    )
);

builder.Services.AddScoped<IMapReportRepository, MapReportRepository>();
builder.Services.AddScoped<IUsersListRepository, UsersListRepository>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
// In Program.cs or Startup.cs
builder.Services.AddSession();
// Configure identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(25);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication and authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login/";
    options.AccessDeniedPath = "/Account/AccessDenied/";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seeding or migration fail");
        Environment.Exit(1); 
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// enable CSP security 
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' https://cdnjs.cloudflare.com/ https://unpkg.com/ https://cdn.jsdelivr.net/ 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' https://cdnjs.cloudflare.com/ https://unpkg.com/ https://fonts.googleapis.com/ 'unsafe-inline'; " +
        "font-src 'self' https://fonts.gstatic.com/ https://ka-f.fontawesome.com/ https://kit.fontawesome.com/ data:; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' wss://localhost:* https://api.kartverket.no/ https://%2A.kartverket.no/ https://ka-f.fontawesome.com/ https://nominatim.openstreetmap.org/; " +
        "object-src 'none';");

    await next();
});

;
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
