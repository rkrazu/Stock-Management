using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class JournalLine
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public Guid JournalEntryId { get; set; }
        [DataMember] public Guid AccountId { get; set; }
        [DataMember] public string AccountCode { get; set; }
        [DataMember] public string AccountName { get; set; }
        [DataMember] public Guid? CustomerId { get; set; }
        [DataMember] public string CustomerName { get; set; }
        [DataMember] public decimal Debit { get; set; }
        [DataMember] public decimal Credit { get; set; }
    }
}
