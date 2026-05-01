using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Authorization;
using OilShopManagement.Data;
using OilShopManagement.Models;
using OilShopManagement.Repositories;
using OilShopManagement.Services;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/oilshop-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

    // Database — support Railway's DATABASE_URL or local appsettings connection string
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string connStr;
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var ui  = uri.UserInfo.Split(':', 2);
        connStr = $"Host={uri.Host};Port={uri.Port};" +
                  $"Database={uri.AbsolutePath.TrimStart('/')};" +
                  $"Username={ui[0]};Password={Uri.UnescapeDataString(ui[1])};" +
                  $"SSL Mode=Require;Trust Server Certificate=true";
    }
    else
    {
        connStr = builder.Configuration.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("No connection string found.");
    }
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connStr));

    // Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

    // Repositories (DI)
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    builder.Services.AddScoped<IStockRepository, StockRepository>();

    // Permission-based Authorization
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
    builder.Services.AddAuthorization(options =>
    {
        foreach (var perm in Permissions.All)
        {
            options.AddPolicy(PolicyNames.For(perm),
                policy => policy.Requirements.Add(new PermissionRequirement(perm)));
        }
    });

    builder.Services.AddScoped<LangHelper>();
    builder.Services.AddControllersWithViews().AddViewLocalization();
    builder.Services.AddSession();

    // Localization
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "ar-SA", "en-US" };
        options.DefaultRequestCulture = new RequestCulture("ar-SA");
        options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
        options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
        options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new CookieRequestCultureProvider { CookieName = "language" },
            new AcceptLanguageHeaderRequestCultureProvider()
        };
    });

    var app = builder.Build();

    // Seed database
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        await SeedData.InitializeAsync(scope.ServiceProvider);
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    app.UseStaticFiles();
    app.UseRouting();
    app.UseRequestLocalization();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();
    app.UseSerilogRequestLogging();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Run($"http://+:{port}");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
