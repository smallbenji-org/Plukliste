using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.ViewModels;
using System.Linq;

namespace PluckFish.Controllers
{
    public class BestillingManagerController : Controller
    {
        private readonly IStockRepository _stockRepository;

        public BestillingManagerController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public IActionResult Index()
        {
            var varer = _stockRepository.GetStock().Take(15);

            var viewModel = new BestillingManagerViewModel
            {
                Varer = varer
            };

            return View(viewModel);
        }
    }
}
