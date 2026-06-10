using System;

namespace Stock_Managemnet.Models
{
    public class StockOutRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid? CustomerId { get; set; }
        public string Notes { get; set; }
    }
}
