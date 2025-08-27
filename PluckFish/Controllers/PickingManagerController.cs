using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PluckFish.Controllers
{
    public class PickingManagerController : Controller
    {
        private readonly ILogger<PickingManagerController> _logger;
        private readonly IPickingListRepository pickingListRepository;
        private readonly IProductRepository productRepository;

        public PickingManagerController(ILogger<PickingManagerController> logger, IPickingListRepository pickingListRepository, IProductRepository productRepository)
        {
            _logger = logger;
            this.pickingListRepository = pickingListRepository;
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var retval = new PickingListViewModel();

            retval.PickingLists = pickingListRepository.GetAllPickingList();
            return View(retval);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("PickingList/{Id}")]
        public IActionResult GetPickingList(int Id)
        {
            var retval = new PickingListViewModel();
            retval.PickingLists = pickingListRepository.GetAllPickingList();
            retval.PickingListSelected = true;
            retval.SelectedPickingList = pickingListRepository.GetPickingList(Id);

            return View("Index", retval);
        }

        [Route("HandlePickingList")]
        public IActionResult HandlePickingList([FromForm] string id)
        {
            return RedirectToAction(nameof(GetPickingList), new { Id = id });
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

    public class PickingListViewModel
    {
        public List<PickingList> PickingLists { get; set; }
        public bool PickingListSelected { get; set; } = false;
        public PickingList SelectedPickingList { get; set; }
    }
}