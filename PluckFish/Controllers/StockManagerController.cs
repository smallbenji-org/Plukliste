using Microsoft.AspNetCore.Mvc;

namespace PluckFish.Controllers
{
    public class StockManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
