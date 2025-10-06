using MediLabo.Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace MediLabo.Identity.API.Data
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByEmailAsync("admin@medilabo.com");

            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = "admin@medilabo.com",
                    Email = "admin@medilabo.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "MediLabo",
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(newAdminUser, "Admin123!");

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }
        }
    }
}
