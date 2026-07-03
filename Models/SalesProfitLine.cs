using System;

namespace Stock_Managemnet.Models
{
    public class SalesProfitLine
    {
        public DateTime Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal CostAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal MarginPercent { get; set; }
    }

    public class SalesProfitSummary
    {
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal MarginPercent { get; set; }
    }
}
