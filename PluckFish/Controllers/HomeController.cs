using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PluckFish.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPickingListRepository pickingListRepository;
        private readonly IProductRepository productRepository;

        public HomeController(ILogger<HomeController> logger, IPickingListRepository pickingListRepository, IProductRepository productRepository)
        {
            _logger = logger;
            this.pickingListRepository = pickingListRepository;
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var retval = new HomeViewModel();

            retval.PickingLists = pickingListRepository.GetAllPickingList();
            PickingList test =  pickingListRepository.GetPickingList(1);
            return View(retval);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OnPost()
        {
            return View();
        }

        public IActionResult Test()
        {
            List<PickingList> list = pickingListRepository.GetAllPickingList();
            return Ok(list); // <-- Let ASP.NET handle serialization
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<PickingList> PickingLists { get; set; }
    }
}
