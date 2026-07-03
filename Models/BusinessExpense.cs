using System;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class BusinessExpense
    {
        [DataMember] public Guid Id { get; set; } = Guid.NewGuid();
        [DataMember] public Guid ExpenseAccountId { get; set; }
        [DataMember] public string ExpenseAccountName { get; set; }
        [DataMember] public Guid CashAccountId { get; set; }
        [DataMember] public string CashAccountName { get; set; }
        [DataMember] public decimal Amount { get; set; }
        [DataMember] public string Reference { get; set; }
        [DataMember] public string Notes { get; set; }
        [DataMember] public DateTime PaidAt { get; set; } = DateTime.Now;
    }

}
