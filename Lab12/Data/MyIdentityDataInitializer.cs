﻿using Microsoft.AspNetCore.Identity;

namespace Lab12.Data
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<IdentityUser> userManager,
             RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)  
            {
                var role = new IdentityRole("Admin");
                var result = roleManager.CreateAsync(role).Result; 
            }
        }

        public static void SeedOneUser(UserManager<IdentityUser> userManager,
             string name, string password, string role = null)
        {
            if (userManager.FindByNameAsync(name).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = name,
                    Email = name
                };
                IdentityResult result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded && role != null)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                }
            }
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            SeedOneUser(userManager, "user@localhost", "Password1!");
            SeedOneUser(userManager, "admin@localhost", "Password1!", "Admin");
        }
    }

}
