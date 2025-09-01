using PluckFish.Models;
using System.Collections.Generic;

namespace PluckFish.ViewModels
{
    public class BestillingManagerViewModel
    {
        public IEnumerable<Item> Varer { get; set; } = new List<Item>();
    }
}