namespace Stock_Managemnet.Models
{
    public enum JournalReferenceType
    {
        Manual = 0,
        Invoice = 1,
        Payment = 2,
        CashIn = 3,
        Expense = 4,
        InvoiceVoid = 5,
        PaymentVoid = 6,
        InvoiceReinstate = 7,
        PaymentReinstate = 8
    }
}
