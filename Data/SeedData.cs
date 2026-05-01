using Microsoft.AspNetCore.Identity;
using OilShopManagement.Models;

namespace OilShopManagement.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = ["Admin", "Employee"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync("admin@oilshop.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@oilshop.com",
                Email = "admin@oilshop.com",
                FullName = "ظ…ط¯ظٹط± ط§ظ„ظ†ط¸ط§ظ…",
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}


