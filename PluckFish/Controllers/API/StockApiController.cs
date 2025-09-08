using Microsoft.AspNetCore.Mvc;
using PluckFish.Attributes;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Controllers.API
{
    [ApiController]
    [Route("api/v1/StockApiController")]
    [ApiTokenAuth]
    public class StockApiController : Controller
    {
        private readonly IStockRepository stockRepository;

        public StockApiController(IStockRepository stockRepository)
        {
            this.stockRepository = stockRepository;
        }

        [HttpGet("getstock")]
        public IActionResult GetStock()
        {
            List <Item> stockItems = stockRepository.GetStock();
            return Ok(stockItems);
        }
    }
}
