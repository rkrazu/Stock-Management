using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class JournalEntry
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public DateTime EntryDate { get; set; } = DateTime.Now;
        [DataMember] public JournalReferenceType ReferenceType { get; set; }
        [DataMember] public Guid? ReferenceId { get; set; }
        [DataMember] public string ReferenceNumber { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataMember] public List<JournalLine> Lines { get; set; } = new List<JournalLine>();
    }
}
