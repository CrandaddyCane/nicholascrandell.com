
var builder = WebApplication.CreateBuilder(args);

// Register MVC services.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security in production.
    app.UseHsts();

    // Permanently redirect WWW to the canonical non-WWW domain.
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

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
