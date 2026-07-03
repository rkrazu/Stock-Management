using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class Product
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string Sku { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Category { get; set; }
        [DataMember] public ProductType ProductType { get; set; } = ProductType.FG;
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal UnitCost { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public int ReorderLevel { get; set; }
        [DataMember] public DateTime LastUpdated { get; set; } = DateTime.Now;

        public bool IsLowStock => Quantity <= ReorderLevel;
        public decimal StockValue => UnitPrice * Quantity;
    }
}
