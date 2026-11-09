using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.Events.Any())
            {
                return;
            }

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
}