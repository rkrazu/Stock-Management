using System;

namespace Stock_Managemnet.Models
{
    public class CustomerDueRow
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public int OpenInvoiceCount { get; set; }
    }
}
