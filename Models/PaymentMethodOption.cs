using System;

namespace Stock_Managemnet.Models
{
    /// <summary>
    /// Option for payment Method dropdowns: Cash or a configured bank account.
    /// </summary>
    public class PaymentMethodOption
    {
        public Guid CashAccountId { get; set; }
        public Guid? BankAccountId { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name ?? string.Empty;
    }
}
