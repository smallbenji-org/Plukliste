using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Diagnostics;

namespace PluckFish.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthRepository authRepository;
        private readonly IPickingListRepository pickingListRepository;
        private readonly IStockRepository stockRepository;

        public HomeController(ILogger<HomeController> logger, IAuthRepository authRepository, IPickingListRepository pickingListRepository, IStockRepository stockRepository)
        {
            _logger = logger;
            this.authRepository = authRepository;
            this.pickingListRepository = pickingListRepository;
            this.stockRepository = stockRepository;
        }

        public IActionResult Index()
        {
            var retval = new HomeViewModel();

            retval.Users = authRepository.GetAll().Count;
            retval.PickingLists = pickingListRepository.GetAllPickingList().Count;
            retval.Products = stockRepository.GetStock().Count;
            retval.TotalStock = stockRepository.GetStock().Sum(x => x.Amount);

            return View(retval);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public int Users { get; set; }
        public int PickingLists { get; set; }
        public int Products { get; set; }
        public int TotalStock { get; set; }
    }
}
