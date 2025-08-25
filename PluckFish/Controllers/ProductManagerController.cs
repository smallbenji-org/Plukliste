using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;

namespace PluckFish.Controllers
{
    public class ProductManagerController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductManagerController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] string prodName, [FromForm] string prodID)
        {
            productRepository.AddProduct(new Models.Product { Name = prodName, ProductID = prodID });

            return RedirectToAction("Index");
        }
    }
}
