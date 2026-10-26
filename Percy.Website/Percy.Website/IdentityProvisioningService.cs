
using Microsoft.AspNetCore.Identity;

namespace Percy.Website.Services
{
    public static class IdentityProvisioningService
    {
        public const string SystemAdministratorRole =
            "SystemAdministrator";

        public static async Task ProvisionAdministratorAsync(
            IServiceProvider serviceProvider,
            string email,
            string password)
        {
            using var scope = serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(
                SystemAdministratorRole))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(SystemAdministratorRole));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Unable to create the administrator role.");
                }
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                var userResult = await userManager.CreateAsync(
                    user,
                    password);

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Unable to create the administrator account: "
                        + string.Join(
                            "; ",
                            userResult.Errors.Select(
                                error => error.Description)));
                }
            }
            else if (!user.EmailConfirmed)
            {
                throw new InvalidOperationException(
                    "Existing account must be verified before provisioning.");
            }

            if (!await userManager.IsInRoleAsync(
                user,
                SystemAdministratorRole))
            {
                var assignmentResult =
                    await userManager.AddToRoleAsync(
                        user,
                        SystemAdministratorRole);

                if (!assignmentResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Unable to assign the administrator role.");
                }
            }
        }
    }
}
