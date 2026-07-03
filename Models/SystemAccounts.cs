using System;

namespace Stock_Managemnet.Models
{
    public static class SystemAccounts
    {
        public static readonly Guid CashId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0001");
        public static readonly Guid BankId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0002");
        public static readonly Guid AccountsReceivableId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0010");
        public static readonly Guid SalesRevenueId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0020");
        public static readonly Guid OwnerEquityId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0030");
        public static readonly Guid FactoryRentId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0051");
        public static readonly Guid EmployeeSalaryId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0052");
        public static readonly Guid SnacksId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0053");
        public static readonly Guid TransportId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0054");
        public static readonly Guid OtherExpenseId = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAA0055");

        public const string CashCode = "1000";
        public const string BankCode = "1010";
        public const string AccountsReceivableCode = "1100";
        public const string SalesRevenueCode = "4000";
        public const string OwnerEquityCode = "3000";
        public const string FactoryRentCode = "5100";
        public const string EmployeeSalaryCode = "5110";
        public const string SnacksCode = "5120";
        public const string TransportCode = "5130";
        public const string OtherExpenseCode = "5190";
    }
}
