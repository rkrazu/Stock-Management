using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class Invoice
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string InvoiceNumber { get; set; }
        [DataMember] public Guid? CustomerId { get; set; }
        [DataMember] public string CustomerName { get; set; }
        [DataMember] public string CustomerPhone { get; set; }
        [DataMember] public string CustomerAddress { get; set; }
        [DataMember] public List<InvoiceLineItem> Items { get; set; } = new List<InvoiceLineItem>();
        [DataMember] public decimal DiscountAmount { get; set; }
        [DataMember] public decimal TotalAmount { get; set; }
        [DataMember] public decimal AmountPaid { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public Guid? TransactionId { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataMember] public OperationalStatus Status { get; set; } = OperationalStatus.Active;
        [DataMember] public DateTime? VoidedAt { get; set; }
        [DataMember] public string VoidReason { get; set; }

        public bool IsActive => Status == OperationalStatus.Active;
        public decimal SubTotal => Items?.Sum(i => i.LineTotal) ?? 0;
        public decimal BalanceDue => IsActive ? Math.Max(0, TotalAmount - AmountPaid) : 0;
    }
}
