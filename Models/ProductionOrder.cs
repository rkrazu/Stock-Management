using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class ProductionOrder
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string ProductionNumber { get; set; }
        [DataMember] public Guid? RecipeId { get; set; }
        [DataMember] public string RecipeName { get; set; }
        [DataMember] public Guid OutputProductId { get; set; }
        [DataMember] public string OutputProductSku { get; set; }
        [DataMember] public string OutputProductName { get; set; }
        [DataMember] public int QuantityProduced { get; set; }
        [DataMember] public decimal OutputUnitPrice { get; set; }
        [DataMember] public decimal TotalOutputValue { get; set; }
        [DataMember] public List<ProductionMaterial> MaterialsUsed { get; set; } = new List<ProductionMaterial>();
        [DataMember] public string Notes { get; set; }
        [DataMember] public DateTime Timestamp { get; set; } = DateTime.Now;
        [DataMember] public OperationalStatus Status { get; set; } = OperationalStatus.Active;
        [DataMember] public DateTime? VoidedAt { get; set; }
        [DataMember] public string VoidReason { get; set; }
        [DataMember] public int PriorOutputQuantity { get; set; }
        [DataMember] public decimal PriorOutputUnitCost { get; set; }
        [DataMember] public decimal BatchUnitCost { get; set; }

        public bool IsActive => Status == OperationalStatus.Active;
    }
}
