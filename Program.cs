using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository;
using Inventory.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using InventoryManagement.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override(
        "Microsoft.Hosting.Lifetime",
        Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information(
        "Starting Laboratory Inventory Management System");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog();

    // MVC
    builder.Services.AddControllersWithViews();

    // Database
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"),
            sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                )
        ));

    // Razor Pages
    builder.Services.AddRazorPages();

    // Identity
    builder.Services
        .AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(5);

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyz" +
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "0123456789-._@+";

            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    // Application cookie
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath =
            "/Identity/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

    // Unit of Work
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Email sender for local development
    builder.Services.AddTransient<IEmailSender, EmailSender>();

    var app = builder.Build();

    // HTTP request pipeline
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

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    // Razor Pages
    app.MapRazorPages();

    // MVC route
    app.MapControllerRoute(
        name: "default",
        pattern:
            "{area=User}/{controller=Home}/{action=Index}/{id?}");

    Log.Information("Application started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(
        ex,
        "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}