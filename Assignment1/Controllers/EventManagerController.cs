using Assignment1.Data;
using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment1.Controllers
{

    [Authorize]
    public class EventManagerController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;

        public EventManagerController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // return index view, showing the table of events. only anonymous action
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_context.Events.ToList());
        }

        // show the manageattendees view, showing the specific attendees for a given event's ID
        [Authorize(Roles = "Organizer")]
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
        [Authorize(Roles = "Organizer")]
        [HttpPost("/events/{eventId}/attendees/create")]
        public IActionResult ManageAttendees(int eventId, Attendee attendee)
        {
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

        // register action that users from both groups can use
        [Authorize]
        [HttpPost("/events/{eventId}/register")]
        public IActionResult Register(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == eventId);

            if (eventItem == null) return NotFound();

            // prevent duplicate registrations
            if (eventItem.Attendees.Any(a => a.UserId == userId))
            {
                return RedirectToAction("EventDetails", new { id = eventId });
            }

            var attendee = new Attendee
            {
                Id = Guid.NewGuid().ToString(),
                EventId = eventId,
                UserId = userId,
                Name = User.Identity!.Name!,
                Email = User.FindFirstValue(ClaimTypes.Email)!
            };

            _context.Attendees.Add(attendee);
            _context.SaveChanges();

            return RedirectToAction("EventDetails", new { id = eventId });
        }

        [Authorize]
        [HttpPost("/events/{eventId}/unregister")]
        public IActionResult Unregister(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attendee = _context.Attendees.FirstOrDefault(
                a => a.UserId == userId && a.EventId == eventId);

            if (attendee != null)
            {
                _context.Attendees.Remove(attendee);
                _context.SaveChanges();
            }

            return RedirectToAction("EventDetails", new { id = eventId });
        }

        // display the create event form
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // handle the submitted create event form, handle images
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> Create(Event eventItem, IFormFile? bannerImage)
        {
            if (ModelState.IsValid)
            {
                if (bannerImage != null)
                {
                    eventItem.BannerUrl = await _blobStorageService.UploadImageAsync(bannerImage);
                }

                _context.Events.Add(eventItem);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            } 
            else
            {
                return BadRequest(ModelState);
            }

                //return View(eventItem);
        }

        // display the edit event form
        [Authorize(Roles = "Organizer")]
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
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Event eventItem, IFormFile? bannerImage)
        {
            if (id != eventItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (bannerImage != null)
                {
                    eventItem.BannerUrl = await _blobStorageService.UploadImageAsync(bannerImage);
                }

                _context.Events.Update(eventItem);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            } 
            else
            {
                return BadRequest(ModelState);
            }

                return View(eventItem);
        }

        // handle event deletion
        [Authorize(Roles = "Organizer")]
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

        [Authorize]
        public IActionResult EventDetails(int id)
        {
            var eventItem = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (eventItem == null) return NotFound();

            return View(eventItem);
        }


        //////////////////////////////////
        ///
        /// Attendee-specific functions
        /// 
        //////////////////////////////////

        // display attendee edit form
        [Authorize(Roles = "Organizer")]
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
        [Authorize(Roles = "Organizer")]
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
        [Authorize(Roles = "Organizer")]
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
