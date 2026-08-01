using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class StockData
    {
        [DataMember] public List<Product> Products { get; set; } = new List<Product>();
        [DataMember] public List<Customer> Customers { get; set; } = new List<Customer>();
        [DataMember] public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        [DataMember] public List<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
        [DataMember] public List<ProductionRecipe> ProductionRecipes { get; set; } = new List<ProductionRecipe>();
        [DataMember] public List<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
        [DataMember] public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        [DataMember] public List<Account> Accounts { get; set; } = new List<Account>();
        [DataMember] public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        [DataMember] public List<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
        [DataMember] public List<CustomerPayment> CustomerPayments { get; set; } = new List<CustomerPayment>();
        [DataMember] public List<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();
        [DataMember] public List<BusinessExpense> BusinessExpenses { get; set; } = new List<BusinessExpense>();
        [DataMember] public int NextInvoiceNumber { get; set; } = 1;
        [DataMember] public int NextProductionNumber { get; set; } = 1;
    }
}
