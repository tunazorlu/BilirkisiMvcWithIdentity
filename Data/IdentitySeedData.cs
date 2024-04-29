using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BilirkisiMvc.Data
{
    public class IdentitySeedData
    {
        private const string adminUser = "admin";
        private const string adminPassword = "Admin_123";

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<VtBaglaci>();

            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(adminUser);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminUser,
                    Email = "admin@tunazorlu.com.tr",
                    PhoneNumber = "5393595969"
                };

                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}