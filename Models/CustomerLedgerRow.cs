using System;

namespace Stock_Managemnet.Models
{
    public class CustomerLedgerRow
    {
        public DateTime Date { get; set; }
        public string EntryType { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        /// <summary>Set for payment (Credit) rows so the UI can void them.</summary>
        public Guid? PaymentId { get; set; }
        public bool HasReceipt { get; set; }
    }
}
