using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;

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
            IEnumerable<Product> products = productRepository.getAll();
            return View(products);
        }

        [HttpPost]
        public IActionResult Index([FromForm] string prodName, [FromForm] string prodID)
        {
            productRepository.AddProduct(new Models.Product { Name = prodName, ProductID = prodID });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string prodID)
        {
            productRepository.DeleteProduct(new Product { ProductID = prodID });

            return RedirectToAction("Index");
        }
    }
}