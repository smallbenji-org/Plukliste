using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Components;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Web;
using System.Xml.Serialization;

namespace PluckFish.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult Index()
        {
            var retval = new PickingListViewModel();

            retval.PickingLists = pickingListRepository.GetAllPickingList();
            return View(retval);
        }


        [HttpGet]
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

        [HttpPost]
        public IActionResult AddProductToPickingList(string prodId, int listId)
        {
            PickingList pickingList = pickingListRepository.GetPickingList(listId);
            Item item = stockRepository.GetItemStock(prodId);

            if (!item.RestVare)
            {
                int sum = pickingListRepository.GetSumOfItemInAllPickingLists(prodId);
                if (item.Amount <= sum) { return BadRequest("Not enough in stock"); }
            }

            item.Amount = 1;
            pickingListRepository.AddProductToPickingList(pickingList, item);

            return RedirectToAction(nameof(EditPickingList), new { id = listId });
        }


        [HttpPost]
        public IActionResult HandlePickingList([FromForm] string id)
        {
            return RedirectToAction(nameof(GetPickingList), new { Id = id });
        }
     
        [Route("CreatePickinglist")]
        public IActionResult CreatePickingList()
        {
            return View();
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult EditPickingList(int id)
        {
            EditPickingListViewModel retval = new EditPickingListViewModel();

            retval.CurrentPickingList = pickingListRepository.GetPickingList(id);
            retval.Items = pickingListRepository.GetPickingListItems(id);
            //retval.Products = productRepository.getAll().ToList();
            List<Item> pickingListItems = productRepository.GetSumOfUsedItemsInPickingLists();
            List<Item> stockItems = stockRepository.GetStock();

            List<Product> products = new List<Product>();
            foreach (Item item in stockItems)
            {
                if (item.RestVare) { products.Add(item.Product); continue; }

                Item listItem = pickingListItems
                .FirstOrDefault(x => x.Product.ProductID == item.Product.ProductID);

                if (listItem == null) { if (item.Amount > 0) { products.Add(item.Product); } continue; }

                if (item.Amount > listItem.Amount) { products.Add(listItem.Product); }
            }
            retval.Products = products;

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
                int sumOfItem = pickingListRepository.GetSumOfItemInAllPickingLists(productId);
                Item stockItem = stockRepository.GetItemStock(productId);
                if (!stockItem.RestVare && stockItem.Amount < (sumOfItem - item.Amount) + productAmount) { return RedirectToAction(nameof(EditPickingList), new { id = pickingListId }); }


                pickingListRepository.UpdateItemInPickingList(pickingList, item, productAmount);
            }

            return RedirectToAction(nameof(EditPickingList), new { id = pickingList.Id });
        }

        [HttpPost("ExportPickingList")]
        public IActionResult ExportPickingList([FromForm] int id, [FromForm] string formatType)
        {
            PickingList pickingList = pickingListRepository.GetPickingList(id);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "PickingLists");

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "PickingLists")))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "PickingLists");
                Directory.CreateDirectory(path);
            }

            if (formatType == ".json")
            {
                using (StreamWriter sw = new StreamWriter($"{path}/PickingList{id}.json"))
                {
                    string jsonString = JsonSerializer.Serialize(pickingList, new JsonSerializerOptions { WriteIndented = true });
                    sw.Write(jsonString);
                }
            }
            else if (formatType == ".csv")
            {
                using (StreamWriter sw = new StreamWriter($"{path}/PickingList{id}.csv"))
                {
                    sw.Write($"{pickingList.Id},{pickingList.Name},{pickingList.Forsendelse},{pickingList.Adresse},{pickingList.IsDone}");

                    sw.WriteLine("ProductID,Amount,Type,RestVare");
                    foreach (var item in pickingList.Lines)
                    {
                        sw.WriteLine($"{item.Product.ProductID},{item.Amount},{item.Type},{item.RestVare}");
                    }
                }
            }
            else if (formatType == ".xml")
            {
                using (StreamWriter sw = new StreamWriter($"{path}/PickingList{id}.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PickingList));
                    serializer.Serialize(sw, pickingList);
                }
            }
            return RedirectToAction(nameof(GetPickingList), new { Id = id });
        }


        [HttpPost]
        public IActionResult ImportPickingList([FromForm] IFormFile file)
        {
            PickingList pickingList = new PickingList();

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".json")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string jsonContent = reader.ReadToEnd();
                    try
                    {
                        pickingList = JsonSerializer.Deserialize<PickingList>(jsonContent);
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest("Invalid JSON format: " + ex.Message);
                    }
                }
                if (pickingList != null)
                {
                    pickingListRepository.AddPickingList(pickingList);
                    int lastId = pickingListRepository.GetAllPickingList().Last().Id;
                    return RedirectToAction(nameof(GetPickingList), new { Id = lastId });
                }
                else
                {
                    return BadRequest("Failed to deserialize the picking list.");
                }
            }
            else if (extension == ".csv")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    bool isFirstLine = true;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }
                        string[] values = line.Split(',');
                        if (values.Length >= 4)
                        {
                            string prodId = values[0].Trim();
                            if (!int.TryParse(values[1].Trim(), out int amount)) { amount = 1; }
                            string typeStr = values[2].Trim().ToLower();
                            string restVareStr = values[3].Trim().ToLower();
                            ItemType type = typeStr == "print" ? ItemType.Print : ItemType.Fysisk;
                            bool restVare = restVareStr == "true";
                            Product product = productRepository.getProduct(prodId);
                            if (product != null)
                            {
                                Item item = new Item
                                {
                                    Product = product,
                                    Amount = amount,
                                    Type = type,
                                    RestVare = restVare
                                };
                                pickingList.AddItem(item);
                            }
                        }
                    }
                }
                if (pickingList != null)
                {
                    pickingListRepository.AddPickingList(pickingList);
                    int lastId = pickingListRepository.GetAllPickingList().Last().Id;
                    return RedirectToAction(nameof(GetPickingList), new { Id = lastId });
                }
                else
                {
                    return BadRequest("Failed to deserialize the picking list.");
                }
            }
            else if (extension == ".xml")
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(PickingList));
                        pickingList = (PickingList)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid XML format: " + ex.Message);
                }
                if (pickingList != null)
                {
                    pickingListRepository.AddPickingList(pickingList);
                    int lastId = pickingListRepository.GetAllPickingList().Last().Id;
                    return RedirectToAction(nameof(GetPickingList), new { Id = lastId });
                }
                else
                {
                    return BadRequest("Failed to deserialize the picking list.");
                }
            }
            else
            {
                return BadRequest("Unsupported file format.");
            }
        }

        public IActionResult SaveProductInPickingList([FromForm] string productId, [FromForm] int pickingListId)
        {
            return View();
        }

        [HttpPost("PickingManager/EditPickingList/SaveAll")]
        public IActionResult SaveAll([FromBody] Dictionary<string, string> values)
        {
            var uri = new Uri(Request.Headers.Referer);

            var query = HttpUtility.ParseQueryString(uri.Query);

            var pickinglistId = query["id"];

            var pickinglist = pickingListRepository.GetPickingList(int.Parse(pickinglistId));    

            foreach (var item in values)
            {
                pickingListRepository.UpdateItemInPickingList(pickinglist, new Item()
                {
                    Product = new Product
                    {
                        ProductID = item.Key
                    }
                }, int.Parse(item.Value));
            }

            return Ok(values);
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
        public IFormFile File { get; set; }
    }

    public class EditPickingListViewModel
    {
        public List<Product> Products { get; set; }
        public List<Item> Items { get; set; }
        public PickingList CurrentPickingList { get; set; }
    }

}