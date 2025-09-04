using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;

namespace PluckFish.Controllers
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public IFormFile File { get; set; }
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

        public IActionResult ImportProduct([FromForm] IFormFile file)
        {
            Product product = new Product();

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".json")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string jsonContent = reader.ReadToEnd();
                    try
                    {
                        product = JsonSerializer.Deserialize<Product>(jsonContent);
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest("Invalid JSON format: " + ex.Message);
                    }
                }
                if (product != null)
                {
                    productRepository.AddProduct(product);
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Failed to deserialize the product.");
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
                            product = productRepository.getProduct(prodId);
                        }
                    }
                }
                if (product != null)
                {
                    productRepository.AddProduct(product);
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Failed to deserialize the product.");
                }
            }
            else if (extension == ".xml")
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Product));
                        product = (Product)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid XML format: " + ex.Message);
                }
                if (product != null)
                {
                    productRepository.AddProduct(product);
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest("Failed to deserialize the product.");
                }
            }
            else
            {
                return BadRequest("Unsupported file format.");
            }
        }

        [HttpPost]
        public IActionResult Index([FromForm] string prodName, [FromForm] string prodID)
        {
            productRepository.AddProduct(new Product { Name = prodName, ProductID = prodID });

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