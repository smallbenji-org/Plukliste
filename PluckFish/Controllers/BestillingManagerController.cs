using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.ViewModels;

namespace PluckFish.Controllers
{
    [Authorize]
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
