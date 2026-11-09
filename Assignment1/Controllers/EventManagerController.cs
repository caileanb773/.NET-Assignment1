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
        [HttpGet("/events/{eventId}/attendees")]
        public IActionResult ManageAttendees(int eventId)
        {
            Event eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // overloaded version that accepst and attendee object
        [HttpPost("/events/{eventId}/attendees/create")]
        public IActionResult ManageAttendees(int eventId, Attendee attendee)
        {
            Console.WriteLine("POST ManageAttendees reached!");
            Event eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            attendee.Id = Guid.NewGuid().ToString();
            attendee.EventId = eventId;

            eventItem.Attendees.Add(attendee);
            _context.SaveChanges();

            return Redirect($"/events/{eventId}/attendees");
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

        // handle event deletion
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //////////////////////////////////
        ///
        /// Attendee-specific functions
        /// 
        //////////////////////////////////

        // display attendee edit form
        public IActionResult EditAttendee(int eventId, string attendeeId)
        {
            var attendee = _context.Attendees.FirstOrDefault(a =>
                a.Id == attendeeId && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            return View(attendee);
        }

        // save attendee changes
        [HttpPost]
        public IActionResult EditAttendee(int eventId, string attendeeId, Attendee attendee)
        {
            var existing = _context.Attendees.FirstOrDefault(a =>
                a.Id == attendeeId && a.EventId == eventId);

            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existing.Name = attendee.Name;
                existing.Email = attendee.Email;

                _context.SaveChanges();

                return Redirect($"/events/{eventId}/attendees");
            }

            return View(attendee);
        }

        // delete attendee
        [HttpPost]
        public IActionResult DeleteAttendee(int eventId, string attendeeId)
        {
            var attendee = _context.Attendees.FirstOrDefault(a =>
                a.Id == attendeeId && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);
            _context.SaveChanges();

            return Redirect($"/events/{eventId}/attendees");
        }

    }
}
