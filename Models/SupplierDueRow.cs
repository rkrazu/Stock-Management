using System;

namespace Stock_Managemnet.Models
{
    public class SupplierDueRow
    {
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Phone { get; set; }
        public decimal TotalPurchased { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public int PurchaseCount { get; set; }
    }
}
