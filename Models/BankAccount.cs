using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class BankAccount
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string Name { get; set; }
        [DataMember] public string AccountNumber { get; set; }
        [DataMember] public string Branch { get; set; }
        [DataMember] public string Notes { get; set; }
        /// <summary>Linked Chart of Accounts asset account used for journals.</summary>
        [DataMember] public Guid GlAccountId { get; set; }
        [DataMember] public bool IsActive { get; set; } = true;
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
