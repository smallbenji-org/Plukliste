using Microsoft.AspNetCore.Mvc;
using PluckFish.Interfaces;
using PluckFish.Interfaces.API;
using PluckFish.Models;

namespace PluckFish.Controllers.API
{
    [ApiController]
    [Route("api/v1/StockApiController")]
    public class StockApiController : Controller
    {
        private readonly IVerificationApi apiRepository;
        private readonly IStockRepository stockRepository;

        public StockApiController(IVerificationApi ApiRepository, IStockRepository StockRepository)
        {
            this.apiRepository = ApiRepository;
            this.stockRepository = StockRepository;
        }

        [HttpGet("getstock")]
        public IActionResult GetStock()
        {
            var token = HttpContext.Request.Headers["Global-Api-Key"].ToString(); 
            bool hasAccess = apiRepository.Verify(token);
            if (!hasAccess) { return BadRequest("You provided an invalid (or expired) Global-Api-Key"); }

            List<Item> stockItems = stockRepository.GetStock();
            return Ok(stockItems);
        }
    }
}
