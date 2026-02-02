using Microsoft.AspNetCore.Identity;
using Serilog;

namespace LondonEstate.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedData(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            // Seed Roles
            string[] roleNames = { "Admin", "Guest" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Get password from configuration (User Secrets in dev, Environment Variables in prod)
            var adminPassword = configuration["EmailSettings:Password"];

            if (string.IsNullOrEmpty(adminPassword))
            {
                Log.Error(
                    "Admin password not configured. Set 'SeedData:AdminPassword' in user secrets or environment variables.");
            }

            // Seed Admin User
            var adminEmail = "q@q.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}

