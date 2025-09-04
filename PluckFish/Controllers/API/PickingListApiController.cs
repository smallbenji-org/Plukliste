using Microsoft.AspNetCore.Mvc;
using PluckFish.Attributes;
using PluckFish.Components;
using PluckFish.Interfaces;
using PluckFish.Models;

namespace PluckFish.Controllers
{
    [ApiController]
    [Route("api/v1/pickinglist")]
    [ApiTokenAuth]
    public class PickingListApiController : Controller
    {
        private readonly IPickingListRepository pickingListRepository;
        private readonly StockHelper stockHelper;
        private readonly IStockRepository stockRepository;

        public PickingListApiController(IPickingListRepository pickingListRepository, StockHelper stockHelper, IStockRepository stockRepository)
        {
            this.pickingListRepository = pickingListRepository;
            this.stockHelper = stockHelper;
            this.stockRepository = stockRepository;
        }

        [HttpGet("GetPickingLists")]
        public IActionResult GetPickingLists()
        {
            return Ok(pickingListRepository.GetAllPickingList());
        }

        [HttpGet("GetPickinglist")]
        public IActionResult GetPickinglist([FromBody] int pickinglistId)
        {
            return Ok(pickingListRepository.GetPickingList(pickinglistId));
        }

        [HttpDelete("DeleteProductFromPickingList/{pickingListId}")]
        public IActionResult DeleteProductFromPickingList(int pickingListId, [FromBody] Item item)
        {
            try
            {
                var pickingList = pickingListRepository.GetPickingList(pickingListId);

                pickingListRepository.DeleteProductFromPickingList(pickingList, item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("AddProductToPickingList/{pickingListId}")]
        public IActionResult AddProductToPickingList(int pickingListId, [FromBody] Item item)
        {
            try
            {
                var pickingList = pickingListRepository.GetPickingList(pickingListId);

                pickingListRepository.AddProductToPickingList(pickingList, item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("TogglePickinglistItem")]
        public IActionResult TogglePickingListItem([FromBody] int id)
        {
            var pickinglist = pickingListRepository.GetPickingList(id);

            pickinglist.Lines = pickingListRepository.GetPickingListItems(pickinglist.Id);

            if (pickinglist == null)
            {
                return BadRequest();
            }

            pickinglist.IsDone = !pickinglist.IsDone;

            pickingListRepository.UpdatePickingList(pickinglist);

            stockHelper.RemoveStockFromPickingList(pickinglist);

            return Ok();
        }

        [HttpPost("AddProductToPickingList/{listId}")]
        public IActionResult AddProductToPickingList([FromBody] string prodId, int listId)
        {
            PickingList pickingList = pickingListRepository.GetPickingList(listId);
            Item item = stockRepository.GetItemStock(prodId);

            if (!item.RestVare)
            {
                int sum = pickingListRepository.GetSumOfItemInAllPickingLists(prodId);
                if (item.Amount <= sum) { return BadRequest("Not enough in stock"); }
            }

            item.Amount = 1;
            pickingListRepository.AddProductToPickingList(pickingList, item);

            return Ok();
        }

        [HttpPost("CreatePickingList")]
        public IActionResult CreatePickingList([FromBody] PickingList pickingList)
        {
            try
            {
                pickingListRepository.AddPickingList(pickingList);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}