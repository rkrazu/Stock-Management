using System;

namespace Stock_Managemnet.Models
{
    public class BankAccountRow
    {
        public Guid BankAccountId { get; set; }
        public Guid GlAccountId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public decimal Balance { get; set; }
    }
}
