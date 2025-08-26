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
    }

    public class StockViewModel()
    {
        public List<Item> stockInventory;
        public string ProdId = null;
    }
}