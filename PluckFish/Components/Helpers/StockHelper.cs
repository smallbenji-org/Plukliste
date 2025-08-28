using PluckFish.Interfaces;
using PluckFish.Models;
using System.Data;
using System.Data.Common;

namespace PluckFish.Components
{
    public class StockHelper
    {
        private readonly IStockRepository stockRepository;

        public StockHelper(IStockRepository stockRepository)
        {

            this.stockRepository = stockRepository;
        }

        public void RemoveStockFromPickingList(PickingList pickingList)
        {
            bool status = stockRepository.RetractMultiStock(pickingList.Lines);
            //foreach (var item in pickingList.Lines)
            //{
            //    stockRepository.RetractStock(item.Product.ProductID, item.Amount);
            //}
        }
    }
}