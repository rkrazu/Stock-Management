using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class CustomerPayment
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public Guid CustomerId { get; set; }
        [DataMember] public string CustomerName { get; set; }
        [DataMember] public Guid? InvoiceId { get; set; }
        [DataMember] public string InvoiceNumber { get; set; }
        [DataMember] public Guid CashAccountId { get; set; }
        [DataMember] public string CashAccountName { get; set; }
        [DataMember] public decimal Amount { get; set; }
        [DataMember] public string PaymentMethod { get; set; }
        [DataMember] public string Reference { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public DateTime PaidAt { get; set; } = DateTime.Now;
        [DataMember] public bool IsVoided { get; set; }
        [DataMember] public DateTime? VoidedAt { get; set; }
        /// <summary>Payment received before any invoice exists (customer advance).</summary>
        [DataMember] public bool IsAdvance { get; set; }
        [DataMember] public string ReceiptRelativePath { get; set; }
        [DataMember] public string ReceiptOriginalName { get; set; }
        [DataMember] public string ReceiptContentType { get; set; }
        [DataMember] public long ReceiptSizeBytes { get; set; }
        [DataMember] public string ReceiptSha256 { get; set; }

        public bool IsActive => !IsVoided;
        public bool HasReceipt => !string.IsNullOrWhiteSpace(ReceiptRelativePath);
    }
}
