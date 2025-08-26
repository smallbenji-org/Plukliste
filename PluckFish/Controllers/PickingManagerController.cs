using Microsoft.AspNetCore.Mvc;

namespace PluckFish.Controllers
{
    public class PickingManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
