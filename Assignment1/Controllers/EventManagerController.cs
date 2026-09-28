using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller 
    {
        public IActionResult Index()
        {
            List<Event> MyEvents = new List<Event>();

            Attendee Mike = new Attendee() { Email = "a@test.com", Name = "Mike" };
            Attendee Bob = new Attendee() { Email = "b@test.com", Name = "Bob" };

            Event DotNetLec = new Event()
            {
                Id = 1,
                Title = ".NET Lecture",
                Date = DateTime.Now,
                Location = "A1202",
                Attendees = new List<Attendee> { Mike, Bob },

            };

            Event RealTimeProgLec = new Event()
            {
                Id = 2,
                Title = "Real-Time Programming Lecture",
                Date = DateTime.Now,
                Location = "CA412",
                Attendees = new List<Attendee> { Mike, Bob },

            };

            MyEvents.Add(DotNetLec);
            MyEvents.Add(RealTimeProgLec);

            return View(MyEvents);
        }
    }
}
