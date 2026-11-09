using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller 
    {
        private readonly ApplicationDbContext _context;

        public EventManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // return index view, showing the table of events
        public IActionResult Index()
        {
            return View(_context.Events.ToList());
        }

        // show the manageattendees view, showing the specific attendees for a given event's ID
        public IActionResult ManageAttendees(int id)
        {
            Event eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // overloaded version that accepst and attendee object
        [HttpPost]
        public IActionResult ManageAttendees(int id, Attendee attendee)
        {
            Event eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            attendee.Id = Guid.NewGuid().ToString();
            attendee.EventId = id;

            eventItem.Attendees.Add(attendee);
            _context.SaveChanges();

            return View(eventItem);
        }

        // display the create event form
        public IActionResult Create()
        {
            return View();
        }

        // handle the submitted create event form
        [HttpPost]
        public IActionResult Create(Event eventItem)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(eventItem);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(eventItem);
        }

        // display the edit event form
        public IActionResult Edit(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // handle the submitted edit form
        [HttpPost]
        public IActionResult Edit(int id, Event eventItem)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Events.Update(eventItem);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(eventItem);
        }

    }
}
