namespace Stock_Managemnet.Models
{
    public enum AccountType
    {
        Asset = 0,
        Liability = 1,
        Equity = 2,
        Income = 3,
        Expense = 4
    }

    public static class AccountTypeLabels
    {
        public static string ToLabel(AccountType type)
        {
            switch (type)
            {
                case AccountType.Liability: return "Liability";
                case AccountType.Equity: return "Equity";
                case AccountType.Income: return "Income";
                case AccountType.Expense: return "Expense";
                default: return "Asset";
            }
        }
    }
}
