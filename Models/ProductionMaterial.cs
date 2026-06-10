using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class ProductionMaterial
    {
        [DataMember] public Guid ProductId { get; set; }
        [DataMember] public string ProductSku { get; set; }
        [DataMember] public string ProductName { get; set; }
        [DataMember] public int QuantityPerUnit { get; set; }
    }
}
