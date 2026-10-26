
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Percy.Website.Data;
using Percy.Website.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services.
builder.Services.AddControllersWithViews();

// Register the PercyWorkspace SQL Server database.
builder.Services.AddDbContext<PercyWorkspaceContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PercyWorkspace")
    )
);

// Register the authentication database context.
builder.Services.AddDbContext<PercyIdentityContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PercyWorkspace")
    )
);

// Configure ASP.NET Core Identity.
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Accounts must be confirmed before login.
        options.SignIn.RequireConfirmedAccount = true;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<PercyIdentityContext>()
    .AddDefaultTokenProviders();

// Configure authentication cookies.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// Explicit, development-only administrator provisioning.
// The database must be local, and provisioning must be
// enabled separately through configuration.
if (app.Environment.IsDevelopment() &&
    app.Configuration.GetValue<bool>(
        "InitialAdmin:EnableProvisioning"))
{
    var connectionString = app.Configuration
        .GetConnectionString("PercyWorkspace");

    var connectionBuilder =
        new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(
            connectionString);

    var server = connectionBuilder.DataSource;

    // Prevent provisioning against remote SQL Server instances.
    var isLocalServer =
        server.Equals("(local)", StringComparison.OrdinalIgnoreCase) ||
        server.Equals(".", StringComparison.OrdinalIgnoreCase) ||
        server.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
        server.StartsWith("(local)\\", StringComparison.OrdinalIgnoreCase) ||
        server.StartsWith(".\\", StringComparison.OrdinalIgnoreCase) ||
        server.StartsWith("localhost\\", StringComparison.OrdinalIgnoreCase);

    if (!isLocalServer)
    {
        throw new InvalidOperationException(
            "Initial administrator provisioning is restricted " +
            "to a local SQL Server instance.");
    }

    var adminEmail = app.Configuration["InitialAdmin:Email"];
    var adminPassword = app.Configuration["InitialAdmin:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new InvalidOperationException(
            "Initial administrator credentials are missing.");
    }

    await IdentityProvisioningService
        .ProvisionAdministratorAsync(
            app.Services,
            adminEmail,
            adminPassword);

    app.Logger.LogInformation(
        "Initial administrator provisioning completed. " +
        "Disable provisioning and remove the setup credentials.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();

    // Redirect WWW to the canonical non-WWW domain.
    app.Use(async (context, next) =>
    {
        if (context.Request.Host.Host.Equals(
            "www.nicholascrandell.com",
            StringComparison.OrdinalIgnoreCase))
        {
            var destination = "https://nicholascrandell.com"
                + context.Request.PathBase
                + context.Request.Path
                + context.Request.QueryString;

            context.Response.Redirect(destination, permanent: true);
            return;
        }

        await next(context);
    });
}

app.UseHttpsRedirection();

app.UseRouting();

// Authenticate users before checking authorization.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
