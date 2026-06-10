using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    public enum TransactionType
    {
        StockIn,
        StockOut
    }

    [DataContract]
    public class StockTransaction
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string InvoiceNumber { get; set; }
        [DataMember] public bool IsSale { get; set; }
        [DataMember] public Guid ProductId { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public string ProductSku { get; set; }
        [DataMember] public TransactionType Type { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal TotalValue { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public Guid? CustomerId { get; set; }
        [DataMember] public string CustomerName { get; set; }
        [DataMember] public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
