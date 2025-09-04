using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductApiController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductApiController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpGet("getproducts")]
        public IActionResult GetProducts()
        {
            return Ok(productRepository.getAll());
        }

        [HttpPut("addproduct")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            try
            {
                productRepository.AddProduct(product);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("deleteproduct")]
        public IActionResult DeleteProduct([FromBody] string productId)
        {
            try
            {
                productRepository.DeleteProduct(new Product
                {
                    ProductID = productId
                });

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}