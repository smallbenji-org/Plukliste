using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluckFish.Controllers
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }

    [Authorize]
    public class ProductManagerController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductManagerController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        private (List<Product> pageItems, int currentPage, int totalPages) GetPage(int nextPage)
        {
            List<Product> products = productRepository.getAll().ToList();

            int pageSize = 15;
            int totalPages = (int)Math.Ceiling(decimal.Divide(products.Count, pageSize));

            if (nextPage > totalPages) { nextPage = totalPages; } else if (nextPage <= 0) { nextPage = 1; }

            int startIndex = (nextPage - 1) * pageSize;
            var pageItems = products
            .Skip(startIndex)
            .Take(pageSize)
            .ToList();
            return (pageItems, nextPage, totalPages);
        }

        public IActionResult Index()
        {
            var viewModel = new ProductViewModel();
            (viewModel.Products, viewModel.CurrentPage, viewModel.TotalPages) = GetPage(1);
            return View(viewModel);
        }

        public IActionResult GetProductTable(int nextPage)
        {
            var model = new ProductViewModel();
            (model.Products, model.CurrentPage, model.TotalPages) = GetPage(nextPage);
            return PartialView("~/Views/ProductManager/_ProductTablePartial.cshtml", model);
        }

        public IActionResult ScrollPage(int nextPage)
        {
            var viewModel = new ProductViewModel();
            (viewModel.Products, viewModel.CurrentPage, viewModel.TotalPages) = GetPage(nextPage);
            return View("Index", viewModel);
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