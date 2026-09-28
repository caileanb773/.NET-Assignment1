using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller 
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
