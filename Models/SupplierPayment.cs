using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class SupplierPayment
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public Guid SupplierId { get; set; }
        [DataMember] public string SupplierName { get; set; }
        [DataMember] public Guid CashAccountId { get; set; }
        [DataMember] public string CashAccountName { get; set; }
        [DataMember] public decimal Amount { get; set; }
        [DataMember] public string PaymentMethod { get; set; }
        [DataMember] public string Reference { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public DateTime PaidAt { get; set; } = DateTime.Now;
        [DataMember] public bool IsVoided { get; set; }
        [DataMember] public DateTime? VoidedAt { get; set; }

        public bool IsActive => !IsVoided;
    }
}
