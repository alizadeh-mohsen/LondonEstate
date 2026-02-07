using LondonEstate.Data;
using LondonEstate.Services;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
    cfg.AddMaps(typeof(ApplicationDbContext).Assembly);
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAxOTU4NDAwIiwiaWF0IjoiMTc3MDQ2NTk0NSIsImFjY291bnRfaWQiOiIwMTljMzdmZGUwNjc3NTQ4YTMxNTdkNjE4ODI2ZTdmZiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2d2end0MzJjOGZudDNqNzVmcjg5ZDVzIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.q4HdD44qh911D4LMxArtwGIZiPfRbi_eMuMFrrzA7egheI2uZDUo3WPJlkKMGrpVaoZqREHyOY3j0sCq3wq50E7SKD9FA7F33eIUaD5AhKBvoB4yOu75hPDHrfceRpes8luDlTqYjrIZy91A2Gyjou8IkJrzPsrH6NCrv1vgtklRnkWA2qaE5hUkx6ML7uFpe2l4swCikBG66BIe5xuwvOc5fU6HekJNkJw3er_mi4ZdjWP7ey42q7Sc9o531wWZBs6B-bnXIsf_FxCd3v3UBClNVHDYv9F8HApF-OurUe7RAnChq1Tv3U3tsIMQHWHZo3AsMcIW5A61HVZiVTnZnQ";
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<IEstimateRequestService, EstimateRequestService>();
builder.Services.AddScoped<ILogError, LogError>();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/LEGroup.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

var app = builder.Build();

await ApplyMigrationsAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    try
    {
        //await DatabaseSeeder.SeedData(userManager, roleManager, configuration);

        var context = services.GetRequiredService<ApplicationDbContext>();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            Log.Warning("Applying {Count} pending migrations...", pendingMigrations.Count());
            await context.Database.MigrateAsync();
            Log.Information("Migrations applied successfully.");
        }
        else
        {
            Log.Information("Database is up to date.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating the database.");

        //if (!app.Environment.IsDevelopment())
        //{
        //    throw;
        //}
    }
}