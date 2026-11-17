using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            context.Database.Migrate();

            // roles
            string[] roles = { "Organizer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // users
            await EnsureUser(userManager, "organizer@test.com", "Password123!", "Organizer");
            await EnsureUser(userManager, "user@test.com", "Password123!", null);

            // events (only seeding if no events exist)
            if (!context.Events.Any())
            {
                var events = new Event[]
                {
                    new Event
                    {
                        Title = "Compilers",
                        Description = "Introduction to Compilers in C.",
                        Date = DateTime.Now.AddDays(7),
                        Location = "C123",
                        BannerUrl = "",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "Dale Cooper", Email = "Cooper@test.com" },
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "Laura Palmer", Email = "Laura@test.com" }
                        }
                    },
                    new Event
                    {
                        Title = "Java Applications Programming",
                        Description = "Introduction to GUI programming in Java.",
                        Date = DateTime.Now.AddDays(14),
                        Location = "T123",
                        BannerUrl = "",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "MIKE", Email = "Mike@test.com" },
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "BOB", Email = "Bob@test.com" }
                        }
                    },
                    new Event
                    {
                        Title = "Calculus",
                        Description = "Derivatives, integrals, other calculus-related topics.",
                        Date = DateTime.Now.AddDays(21),
                        Location = "A123",
                        BannerUrl = "",
                        Attendees = new List<Attendee>
                        {
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "Big Ed", Email = "BigEd@test.com" },
                            new Attendee { Id = Guid.NewGuid().ToString(), Name = "Shelly Johnson", Email = "Shelly@test.com" }
                        }
                    }
                };

                context.Events.AddRange(events);
                context.SaveChanges();
            }
        }

        private static async Task EnsureUser(UserManager<IdentityUser> userManager,
            string email, string password, string? role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (role != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}