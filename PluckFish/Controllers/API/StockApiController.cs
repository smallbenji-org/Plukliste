using Microsoft.AspNetCore.Mvc;
using PluckFish.Attributes;
using PluckFish.Interfaces;
using PluckFish.Interfaces.API;
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

        [HttpGet("GetStock")]
        public IActionResult GetStock()
        {
            List <Item> stockItems = stockRepository.GetStock();
            return Ok(stockItems);
        }

        [HttpGet("GetItemStock/{prodId}")]
        public IActionResult GetItemStock(string prodId)
        {
            Item stockItem = stockRepository.GetItemStock(prodId);
            return Ok(stockItem);
        }

        [HttpGet("StockExist/{prodId}")]
        public IActionResult StockExist(string prodId)
        {
            bool exists = stockRepository.StockExist(prodId);
            return Ok(exists);
        }

        [HttpPut("SaveStock")]
        public IActionResult SaveStock([FromBody] Item item)
        {
            try
            {
                stockRepository.SaveStock(item);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }            
        }

        [HttpPut("RetractStock/{prodId}")]
        public IActionResult RetractStock(string prodId, int retractAmount)
        {
            try
            {
                stockRepository.RetractStock(prodId, retractAmount);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("RetractMultiStock")]
        public IActionResult RetractMultiStock([FromBody] List<Item> items)
        {
            try
            {
                stockRepository.RetractMultiStock(items);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
