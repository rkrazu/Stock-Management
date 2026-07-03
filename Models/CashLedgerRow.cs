using System;

namespace Stock_Managemnet.Models
{
    public class CashLedgerRow
    {
        public DateTime Date { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
