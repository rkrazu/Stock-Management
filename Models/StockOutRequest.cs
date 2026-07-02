using System;
using System.Collections.Generic;

namespace Stock_Managemnet.Models
{
    public class StockOutRequest
    {
        public Guid? CustomerId { get; set; }
        public string Notes { get; set; }
        public List<StockOutLineItem> Lines { get; set; } = new List<StockOutLineItem>();
    }
}
