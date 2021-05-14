using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Data
{
    public static class SeedData
    {
        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var user = new IdentityUser()
                {
                    UserName = "admin",
                    Email = "admin@bookstoreapi.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }    
            }
            if (await userManager.FindByEmailAsync("customer@gmail.com") == null)
            {
                var user = new IdentityUser()
                {
                    UserName = "customer",
                    Email = "customer@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "Customer1*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }
        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole()
                { Name = "Administrator" };
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole()
                { Name = "Customer" };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
