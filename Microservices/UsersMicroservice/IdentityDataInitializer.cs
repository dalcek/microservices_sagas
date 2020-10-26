using Microsoft.AspNetCore.Identity;
using UsersMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("ivana").Result == null)
            {
                User user = new User();
                user.UserName = "ivana";
                user.Email = "user1@localhost";
                user.FullName = "Ivana Kosutic";
                user.Birthday = new DateTime(1990, 1, 1);
                user.Address = "Gacko";
                user.FriendRequests = new List<FriendRequestReceived>();
                user.Friends = new List<User>();
                user.Passport = "11111111";
                user.PhoneNumber = "065111222";
                user.Points = 0;
                user.SentRequests = new List<FriendRequestSent>();
                user.ChangedPassword = true;
                user.MainWebsiteAdministrator = false;
                user.SocialUser = false;
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "ivana").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "RegisteredUser").Wait();
                }
            }
            if (userManager.FindByNameAsync("zeljka").Result == null)
            {
                User user = new User();
                user.UserName = "zeljka";
                user.Email = "user1@localhost";
                user.FullName = "Zeljka Matovic";
                user.Birthday = new DateTime(1990, 1, 1);
                user.Address = "Gacko";
                user.FriendRequests = new List<FriendRequestReceived>();
                user.Friends = new List<User>();
                user.Passport = "123456789";
                user.PhoneNumber = "2222222";
                user.Points = 0;

                user.SentRequests = new List<FriendRequestSent>();
                user.SocialUser = false;
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "zeljka").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "RegisteredUser").Wait();
                }
            }
            if (userManager.FindByNameAsync("milica").Result == null)
            {
                User user = new User();
                user.UserName = "milica";
                user.Email = "user2@localhost";
                user.FullName = "Milica Sarenac";
                user.Birthday = new DateTime(1965, 1, 1);
                user.Address = "Zvornik";
                user.PhoneNumber = "065111222";
                user.ChangedPassword = true;
                user.MainWebsiteAdministrator = true;
                Discount disc = new Discount();
                disc.BronzeTier = 50;
                disc.SilverTier = 100;
                disc.GoldTier = 200;
                disc.DiscountPercent = 10;
                user.Discount = disc;
                user.SocialUser = false;
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "milica").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "WebsiteAdministrator").Wait();
                }
            }
            if (userManager.FindByNameAsync("nina").Result == null)
            {
                User user = new User();
                user.UserName = "nina";
                user.Email = "user3@localhost";
                user.FullName = "Nikolina Sarenac";
                user.Birthday = new DateTime(1965, 1, 1);
                user.Address = "Zvornik";
                user.PhoneNumber = "4444444";
                user.CompanyId = 1;
                user.ChangedPassword = true;
                user.MainWebsiteAdministrator = false;
                user.SocialUser = false;
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "nina").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "RentACarAdministrator").Wait();
                }
            }
            if (userManager.FindByNameAsync("stanka").Result == null)
            {
                User user = new User();
                user.UserName = "stanka";
                user.Email = "user2@localhost";
                user.FullName = "Stanka Kosutic";
                user.Birthday = new DateTime(1965, 1, 1);
                user.Address = "Gacko";
                user.PhoneNumber = "065111222";
                user.ChangedPassword = true;
                user.MainWebsiteAdministrator = false;
                user.CompanyId = 1;
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "stanka").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "AirlineAdministrator").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<Role> roleManager)
        {
            if (!roleManager.RoleExistsAsync("RegisteredUser").Result)
            {
                Role role = new Role();
                role.Name = "RegisteredUser";
                role.Description = "Perform normal operations.";
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("WebsiteAdministrator").Result)
            {
                Role role = new Role();
                role.Name = "WebsiteAdministrator";
                role.Description = "Perform all the operations.";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("AirlineAdministrator").Result)
            {
                Role role = new Role();
                role.Name = "AirlineAdministrator";
                role.Description = "Manage airline.";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("RentACarAdministrator").Result)
            {
                Role role = new Role();
                role.Name = "RentACarAdministrator";
                role.Description = "Manage rent a car.";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
