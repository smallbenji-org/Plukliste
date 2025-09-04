using Microsoft.AspNetCore.Mvc;

namespace PluckFish.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Tokenhåndtering";
            return View();
        }
    }
}
