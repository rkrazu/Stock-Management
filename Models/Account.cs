using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class Account
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public string Code { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public AccountType Type { get; set; }
        [DataMember] public bool IsSystem { get; set; }
        [DataMember] public bool IsActive { get; set; } = true;
        [DataMember] public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
