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

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(pickingListRepository.GetAllPickingList());
        }

        [HttpGet("{id}")]
        public IActionResult GetPickinglist(int id)
        {
            var pickingList = pickingListRepository.GetPickingList(id);

            pickingList.Lines = pickingListRepository.GetPickingListItems(id);

            return Ok(pickingList);
        }

        [HttpDelete("{id}/items")]
        public IActionResult DeleteProductFromPickingList(int id, [FromBody] Item item)
        {
            try
            {
                var pickingList = pickingListRepository.GetPickingList(id);

                pickingListRepository.DeleteProductFromPickingList(pickingList, item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/items")]
        public IActionResult GetItemsFromPickinglist(int id)
        {
            return Ok(pickingListRepository.GetPickingListItems(id));
        }


        [HttpPost("{id}/items")]
        public IActionResult AddProductToPickingList(int id, [FromBody] Item item)
        {
            try
            {
                var pickingList = pickingListRepository.GetPickingList(id);

                pickingListRepository.AddProductToPickingList(pickingList, item);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("{id}/toggle")]
        public IActionResult TogglePickingList(int id)
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

        [HttpPut("{id}/items/{productId}")]
        public IActionResult EditProduct(int id, string productId, [FromBody] Item item)
        {
            try
            {
                var pickingList = pickingListRepository.GetPickingList(id);

                pickingListRepository.UpdateItemInPickingList(pickingList, item, item.Amount);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult EditPickinglist([FromBody] PickingList pl)
        {
            try
            {
                pickingListRepository.UpdatePickingList(pl);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}