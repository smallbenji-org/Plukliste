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

        public IActionResult Index()
        {
            StockViewModel retval = new StockViewModel();
            retval.stockInventory = stockRepository.getStock();          
            return View(retval);
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

            return View();
        }
    }

    public class StockViewModel()
    {
        public List<Item> stockInventory;
    }
}