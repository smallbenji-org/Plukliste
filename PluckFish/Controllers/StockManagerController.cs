using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Text.Json;
using System.Xml.Serialization;

namespace PluckFish.Controllers
{
    [Authorize]
    public class StockManagerController : Controller
    {
        private readonly IStockRepository stockRepository;
        private readonly IProductRepository productRepository;

        public StockManagerController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            this.stockRepository = stockRepository;
            this.productRepository = productRepository;
        }
        private (List<Item> pageItems, int currentPage, int totalPages) getPage(int nextPage, string filter = "All", string searchText = "")
        {
            List<Item> items = stockRepository.GetStock();
            if (filter == "VisVare")
            {
                items = items.Where(x => !x.RestVare).ToList();
            }
            else if (filter == "VisRestVare")
            {
                items = items.Where(x => x.RestVare).ToList();
            }

            if (searchText != "")
            {
                items = items.Where(x => x.Product.Name.ToLowerInvariant().Contains(searchText.ToLowerInvariant())).ToList();
            }

            int pageSize = 15;
            int totalPages = (int)Math.Ceiling(decimal.Divide(items.Count, pageSize));

            if (nextPage > totalPages) { nextPage = totalPages; } else if (nextPage <= 0) { nextPage = 1; }

            int startIndex = (nextPage - 1) * pageSize;
            var pageItems = items
            .Skip(startIndex)
            .Take(pageSize)
            .ToList();
            return (pageItems, nextPage, totalPages);
        }

        public IActionResult Index()
        {
            StockViewModel retval = new StockViewModel();
            (retval.stockInventory, retval.currentPage, retval.TotalPages) = getPage(0);
            return View(retval);
        }

        public IActionResult GetStockTable(int nextPage, string filter = "All", string searchText = "")
        {
            var model = new StockViewModel();
            (model.stockInventory, model.currentPage, model.TotalPages) = getPage(nextPage, filter, searchText);
            model.filter = filter;
            return PartialView("_StockTablePartial", model);
        }

        public IActionResult ScrollPage(int nextPage, string filter = "All")
        {
            StockViewModel retval = new StockViewModel();
            (retval.stockInventory, retval.currentPage, retval.TotalPages) = getPage(nextPage, filter);
            retval.filter = filter;
            return View("Index", retval);
        }

        [HttpPost]
        public IActionResult ItemOrder([FromForm] string order)
        {
            StockViewModel retval = new StockViewModel();
            (retval.stockInventory, retval.currentPage, retval.TotalPages) = getPage(0);


            List<Item> items = stockRepository.GetStock();

            switch (order)
            {
                case "Newest":
                    retval.stockInventory.Reverse();
                    break;
                case "Oldest":
                    break;
                case "Highest":
                    retval.stockInventory = retval.stockInventory.OrderByDescending(x => x.Amount).ToList();
                    break;
                case "Lowest":
                    retval.stockInventory = retval.stockInventory.OrderBy(x => x.Amount).ToList();
                    break;
            }
            return View("Index", retval);
        }

        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded or the file is empty.");
            }

            List<Item> items = new List<Item>();

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".json")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string jsonContent = reader.ReadToEnd();
                    try
                    {
                        items = JsonSerializer.Deserialize<List<Item>>(jsonContent);
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest("Invalid JSON format: " + ex.Message);
                    }
                }
                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        stockRepository.SaveStock(item);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Failed to deserialize the items.");
                }
            }
            else if (extension == ".csv")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string[] lines = reader.ReadToEnd().Split('\n');

                    foreach (string line in lines.Skip(1)) // drenge vi skipper altså headeren
                    {
                        string[] values = line.Split(',');

                        string prodId = values[0].Trim();
                        if (!int.TryParse(values[1].Trim(), out int amount)) { amount = 1; }
                        string typeStr = values[2].Trim().ToLower();
                        string restVareStr = values[3].Trim().ToLower();
                        ItemType type = typeStr == "print" ? ItemType.Print : ItemType.Fysisk;
                        bool restVare = restVareStr == "true";
                        Product product = null;
                        try
                        {
                            product = productRepository.getProduct(prodId);
                        }
                        catch (InvalidOperationException)
                        {
                            product = null;
                        }

                        if (product != null)
                        {
                            Item item = new Item
                            {
                                Product = product,
                                Amount = amount,
                                Type = type,
                                RestVare = restVare
                            };
                            items.Add(item);
                        }

                    }
                    if (items != null)
                    {
                        foreach (Item item in items)
                        {
                            stockRepository.SaveStock(item);
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return BadRequest("Failed to deserialize the items.");
                    }
                }
            }
            else if (extension == ".xml")
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
                        items = (List<Item>)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid XML format: " + ex.Message);
                }
                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        stockRepository.SaveStock(item);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Failed to deserialize the items.");
                }
            }
            else
            {
                return BadRequest("Unsupported file format.");
            }
        }

        [HttpPost]
        public IActionResult Result(string prodId, int amount, bool restVare)
        {
            Item item = new Item();
            item.Product = new Product();

            item.RestVare = restVare;

            item.Product.ProductID = prodId;
            item.Amount = amount;
            stockRepository.SaveStock(item);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult VisVare(string prodId, int amount, bool restVare)
        {
            StockViewModel retval = new StockViewModel();
            retval.filter = "VisVare";
            (retval.stockInventory, retval.currentPage, retval.TotalPages) = getPage(retval.currentPage, retval.filter);
            return View("Index", retval);
        }
        public IActionResult VisRestVare(string prodId, int amount, bool restVare)
        {
            StockViewModel retval = new StockViewModel();
            retval.filter = "VisRestVare";
            (retval.stockInventory, retval.currentPage, retval.TotalPages) = getPage(retval.currentPage, retval.filter);
            return View("Index", retval);
        }

    }
    public class StockViewModel()
    {
        public List<Item> stockInventory { get; set; } = new List<Item>();
        public int currentPage { get; set; } = 1;
        public string filter { get; set; } = "All";
        public int TotalPages { get; set; }
    }
}