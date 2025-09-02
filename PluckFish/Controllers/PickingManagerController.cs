using Microsoft.AspNetCore.Mvc;
using PluckFish.Components;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Diagnostics;
using System.Text.Json;

namespace PluckFish.Controllers
{
    public class PickingManagerController : Controller
    {
        private readonly ILogger<PickingManagerController> _logger;
        private readonly StockHelper stockHelper;
        private readonly IPickingListRepository pickingListRepository;
        private readonly IStockRepository stockRepository;
        private readonly IProductRepository productRepository;

        public PickingManagerController(
            ILogger<PickingManagerController> logger,
            StockHelper stockHelper,
            IPickingListRepository pickingListRepository,
            IStockRepository stockRepository,
            IProductRepository productRepository
        )
        {
            _logger = logger;
            this.stockHelper = stockHelper;
            this.pickingListRepository = pickingListRepository;
            this.stockRepository = stockRepository;
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

            retval.SelectedPickingList.Lines = pickingListRepository.GetPickingListItems(retval.SelectedPickingList.Id);

            return View("Index", retval);
        }

        [HttpPost]
        public IActionResult Toggle(int id)
        {
            var pickingList = pickingListRepository.GetPickingList(id);
            pickingList.Lines = pickingListRepository.GetPickingListItems(pickingList.Id);
            if (pickingList == null)
            {
                return NotFound();
            }
            pickingList.IsDone = !pickingList.IsDone;

            pickingListRepository.UpdatePickingList(pickingList); // safe isDone state

            stockHelper.RemoveStockFromPickingList(pickingList);

            return RedirectToAction("Index");
        }

        [HttpPost("EditPickinglist/AddProductToPickingList")]
        public IActionResult AddProductToPickingList(string prodId, int listId)
        {
            PickingList pickingList = pickingListRepository.GetPickingList(listId);
            Item item = stockRepository.GetItemStock(prodId);
          
            if (!item.RestVare)
            {
                int sum = pickingListRepository.GetSumOfItemInAllPickingLists(prodId);
                if (item.Amount <= sum) { return BadRequest(); }
            }

            item.Amount = 1;
            pickingListRepository.AddProductToPickingList(pickingList, item);

            return Ok();
        }

        [Route("HandlePickingList")]
        public IActionResult HandlePickingList([FromForm] string id)
        {
            return RedirectToAction(nameof(GetPickingList), new { Id = id });
        }

        [Route("CreatePickinglist")]
        public IActionResult CreatePickingList()
        {
            return View();
        }

        [HttpPost("CreatePickinglist")]
        public IActionResult CreatePickingList([FromForm] string name, [FromForm] string shipping, [FromForm] string address)
        {
            PickingList pickingList = new PickingList();

            pickingList.Name = name;
            pickingList.Forsendelse = shipping;
            pickingList.Adresse = address;

            pickingListRepository.AddPickingList(pickingList);
            int lastId = pickingListRepository.GetAllPickingList().Last().Id;

            return RedirectToAction(nameof(GetPickingList), new { Id = lastId });
        }

        [Route("EditPickinglist/{id}")]
        public IActionResult EditPickingList(int id)
        {
            EditPickingListViewModel retval = new EditPickingListViewModel();

            retval.CurrentPickingList = pickingListRepository.GetPickingList(id);
            retval.Items = pickingListRepository.GetPickingListItems(id);
            retval.Products = productRepository.getAll().ToList();

            return View(retval);
        }

        [HttpPost]
        public IActionResult RemoveProductFromPickingList([FromForm] string productId, [FromForm] int pickingListId)
        {
            List<PickingList> pickingLists = pickingListRepository.GetAllPickingList();
            PickingList pickingList = pickingLists.FirstOrDefault(pl => pl.Id == pickingListId);
            pickingList.Lines = pickingListRepository.GetPickingListItems(pickingListId);
            Item item = pickingList.Lines.FirstOrDefault(i => i.Product.ProductID == productId);
            if (pickingList != null && item != null)
            {
                pickingListRepository.DeleteProductFromPickingList(pickingList, item);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(EditPickingList), new { id = pickingList.Id });
        }

        [HttpPost]
        public IActionResult SaveProductInPickingList([FromForm] string productId, [FromForm] int pickingListId, [FromForm] int productAmount)
        {
            List<PickingList> pickingLists = pickingListRepository.GetAllPickingList();
            PickingList pickingList = pickingLists.FirstOrDefault(pl => pl.Id == pickingListId);
            pickingList.Lines = pickingListRepository.GetPickingListItems(pickingListId);
            Item item = pickingList.Lines.FirstOrDefault(i => i.Product.ProductID == productId);

            if (pickingList != null && pickingList != null)
            {
                pickingListRepository.UpdateItemInPickingList(pickingList, item, productAmount);
            }

            return RedirectToAction(nameof(EditPickingList), new { id = pickingList.Id });
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

    public class EditPickingListViewModel
    {
        public List<Product> Products { get; set; }
        public List<Item> Items { get; set; }
        public PickingList CurrentPickingList { get; set; }
    }
}