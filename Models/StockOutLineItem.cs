using System;

namespace Stock_Managemnet.Models
{
    public class StockOutLineItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
