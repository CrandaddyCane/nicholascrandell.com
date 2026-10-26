
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Percy.Website.Data;
using Percy.Website.Services;
using System.ComponentModel.DataAnnotations;

namespace Percy.IdentityProvisioner;

internal class ProvisionerProgram
{
    private static async Task Main()
    {
        Console.WriteLine("Percy Hosted Administrator Provisioner");
        Console.WriteLine("=====================================");
        Console.WriteLine();

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ProvisionerProgram>()
            .Build();

        var connectionString = configuration.GetConnectionString(
            "PercyWorkspaceHosted");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("ERROR: Hosted connection string is missing.");
            return;
        }

        var settings = new SqlConnectionStringBuilder(connectionString);

        if (!string.Equals(settings.DataSource.Trim(),
                "sql9001.site4now.net",
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(settings.InitialCatalog.Trim(),
                "db_acfa48_percynw",
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(settings.UserID.Trim(),
                "db_acfa48_percynw_admin",
                StringComparison.OrdinalIgnoreCase) ||
            settings.IntegratedSecurity ||
            settings.Encrypt == SqlConnectionEncryptOption.Optional)
        {
            Console.WriteLine("ERROR: Hosted connection validation failed.");
            return;
        }

        Console.WriteLine($"Server: {settings.DataSource}");
        Console.WriteLine($"Database: {settings.InitialCatalog}");
        Console.WriteLine();

        var services = new ServiceCollection();

        services.AddLogging();

        // Required for ASP.NET Core Identity token providers.
        services.AddDataProtection();

        services.AddDbContext<PercyIdentityContext>(options =>
            options.UseSqlServer(settings.ConnectionString));

        services
            .AddIdentityCore<IdentityUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<PercyIdentityContext>()
            .AddDefaultTokenProviders();

        using var provider = services.BuildServiceProvider();

        try
        {
            using var scope = provider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<PercyIdentityContext>();

            await context.Database.OpenConnectionAsync();

            var userCount = await context.Users.CountAsync();

            await context.Database.CloseConnectionAsync();

            if (userCount != 0)
            {
                Console.WriteLine(
                    "STOP: The hosted database already contains users.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Database check failed: {ex.GetType().Name}");
            Console.WriteLine(
                "No administrator account was provisioned.");
            return;
        }

        Console.WriteLine("Hosted database connection successful.");
        Console.WriteLine("No existing users found.");
        Console.WriteLine();

        Console.Write("Administrator email: ");
        var email = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(email) ||
            !new EmailAddressAttribute().IsValid(email))
        {
            Console.WriteLine("Invalid administrator email.");
            return;
        }

        Console.Write("Administrator password: ");

        var characters = new List<char>();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (characters.Count > 0)
                    characters.RemoveAt(characters.Count - 1);

                continue;
            }

            if (!char.IsControl(key.KeyChar))
                characters.Add(key.KeyChar);
        }

        var password = new string(characters.ToArray());
        characters.Clear();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Administrator password is required.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("You are about to create:");
        Console.WriteLine($"Account: {email}");
        Console.WriteLine("Role: SystemAdministrator");
        Console.WriteLine($"Database: {settings.InitialCatalog}");
        Console.WriteLine();

        Console.Write("Type PROVISION to confirm: ");

        if (Console.ReadLine() != "PROVISION")
        {
            Console.WriteLine("Provisioning cancelled.");
            return;
        }

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<PercyIdentityContext>();

            if (await context.Users.AnyAsync())
            {
                Console.WriteLine(
                    "STOP: User accounts already exist.");
                return;
            }
        }

        try
        {
            await IdentityProvisioningService
                .ProvisionAdministratorAsync(
                    provider,
                    email,
                    password);

            Console.WriteLine();
            Console.WriteLine("ADMINISTRATOR PROVISIONING SUCCESSFUL");
            Console.WriteLine(
                "Your hosted administrator account has been created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("Administrator provisioning failed.");
            Console.WriteLine(ex.Message);
        }
    }
}
