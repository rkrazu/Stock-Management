using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class InvoiceLineItem
    {
        [DataMember] public Guid ProductId { get; set; }
        [DataMember] public string ProductSku { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public string ProductCategory { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal LineTotal { get; set; }
    }
}
