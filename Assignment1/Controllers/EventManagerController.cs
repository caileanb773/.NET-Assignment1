using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller 
    {
        List<Event> MyEvents = new List<Event>();

        // constructor makes the dummy events and adds them to the list
        public EventManagerController()
        {
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
        }

        // return index view, showing the table of events
        public IActionResult Index()
        {
            return View(MyEvents);
        }

        // show the manageattendees view, showing the specific attendees for a given event's ID
        public IActionResult ManageAttendees(int Id)
        {
            // for each event in MyEvents, check if its id matches the passed id
            Event MyEvent = MyEvents.FirstOrDefault(e => e.Id == Id);

            // return that specific event view
            return View(MyEvent);
        }

        // overloaded version that accepst and attendee object
        [HttpPost]
        public IActionResult ManageAttendees(int Id, Attendee Attendee)
        {
            return View();
        }
    }
}
