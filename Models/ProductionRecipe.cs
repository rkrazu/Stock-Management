using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class ProductionRecipe
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string Name { get; set; }
        [DataMember] public Guid OutputProductId { get; set; }
        [DataMember] public string OutputProductSku { get; set; }
        [DataMember] public string OutputProductName { get; set; }
        [DataMember] public List<ProductionMaterial> Materials { get; set; } = new List<ProductionMaterial>();
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
