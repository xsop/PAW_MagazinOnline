using Microsoft.AspNetCore.Identity;
using Shop.Consts;

namespace Shop.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefault(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles to DB

            await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // Create Moderator

            var moderator = new IdentityUser
            {
                UserName = "moderator@test.test",
                Email = "moderator@test.test",
                EmailConfirmed = true
            };

            var isModeratorDB = await userManager.FindByEmailAsync(moderator.Email);
            if (isModeratorDB == null)
            {
                await userManager.CreateAsync(moderator, "Moderator1!");
                await userManager.AddToRoleAsync(moderator, Roles.Moderator.ToString());
            }
        }
    }
}
