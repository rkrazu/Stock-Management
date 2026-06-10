using System;
using System.Collections.Generic;
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
        [DataMember] public decimal TotalAmount { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public Guid? TransactionId { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
