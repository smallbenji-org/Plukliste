using Microsoft.AspNetCore.Mvc;
using PluckFish.Components;
using PluckFish.Interfaces;
using PluckFish.Models;
using System.Collections.Generic;

namespace PluckFish.Controllers
{
    public class StockManagerController : Controller
    {
        private readonly IStockRepository stockRepository;

        public StockManagerController(IStockRepository stockRepository)
        {
            this.stockRepository = stockRepository;
        }
        private (List<Item>, int) getPage(int nextPage, string filter = "All")
        {
            List<Item> items = stockRepository.getStock();
            if (filter == "VisVare")
            {
                items = items.Where(x => !x.RestVare).ToList();
            }
            else if (filter == "VisRestVare")
            {
                items = items.Where(x => x.RestVare).ToList();
            }

            int pageSize = 25;
            int totalPages = (int)Math.Ceiling(decimal.Divide(items.Count, pageSize));

            if (nextPage > totalPages) { nextPage = totalPages; } else if (nextPage <= 0) { nextPage = 1; }

            int startIndex = (nextPage - 1) * pageSize;
            var pageItems = items
            .Skip(startIndex)
            .Take(pageSize)
            .ToList();
            return (pageItems, nextPage);
        }

        public IActionResult Index()
        {
            StockViewModel retval = new StockViewModel();
            (retval.stockInventory, retval.currentPage) = getPage(0);
            return View(retval);
        }

        public IActionResult ScrollPage(int nextPage, string filter = "All")
        {
            StockViewModel retval = new StockViewModel();
            (retval.stockInventory, retval.currentPage) = getPage(nextPage, filter);
            retval.filter = filter;
            return View("Index", retval);
        }

        [HttpPost]
        public IActionResult Result(string prodId, int amount, bool restVare)
        {
            Item item = new Item();
            item.Product = new Product();

            item.RestVare = restVare;

            item.Product.ProductID = prodId;
            item.Amount = amount;
            stockRepository.saveStock(item);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult VisVare(string prodId, int amount, bool restVare)
        {
            StockViewModel retval = new StockViewModel();
            retval.filter = "VisVare";
            (retval.stockInventory, retval.currentPage) = getPage(retval.currentPage, retval.filter);
            return View("Index", retval);
        }
        public IActionResult VisRestVare(string prodId, int amount, bool restVare)
        {
            StockViewModel retval = new StockViewModel();
            retval.filter = "VisRestVare";
            (retval.stockInventory, retval.currentPage) = getPage(retval.currentPage, retval.filter);
            return View("Index", retval);
        }

        }
    public class StockViewModel()
    {
        public List<Item> stockInventory { get; set; } = new List<Item>();
        public int currentPage { get; set; } = 1;
        public string filter { get; set; } = "All";
    }
}