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

        private (List<Product> pageItems, int currentPage, int totalPages) GetPage(int nextPage, string searchText = "")
        {
            List<Product> products = productRepository.getAll().ToList();

            if (searchText != "")
            {
                products = products.Where(x => x.Name.ToLowerInvariant().Contains(searchText.ToLowerInvariant())).ToList();
            }

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

        public IActionResult GetProductTable(int nextPage, string searchText = "")
        {
            var model = new ProductViewModel();
            (model.Products, model.CurrentPage, model.TotalPages) = GetPage(nextPage, searchText);
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
            List<Product> product = new List<Product>();

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".json")
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    string jsonContent = reader.ReadToEnd();
                    try
                    {
                        product = JsonSerializer.Deserialize<List<Product>>(jsonContent);
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest("Invalid JSON format: " + ex.Message);
                    }
                }
                if (product != null)
                {
                    foreach (Product prod in product)
                    {
                        productRepository.AddProduct(prod);
                    }
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
                    string[] lines = reader.ReadToEnd().Split('\n');
                    
                    foreach (string line in lines.Skip(1)) // drenge vi skipper altså headeren
                    {
                        string[] values = line.Split(',');

                        Product prod = new Product
                        {
                            ProductID = values[0],
                            Name = values[1]
                        };
                        product.Add(prod);
                    }

                }
                if (product != null)
                {
                    foreach (Product prod in product)
                    {
                        productRepository.AddProduct(prod);
                    }
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
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Product>));
                        product = (List<Product>)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid XML format: " + ex.Message);
                }
                if (product != null)
                {
                    foreach (Product prod in product)
                    {
                        productRepository.AddProduct(prod);
                    }
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