using System;
using System.Collections.Generic;
using System.Linq;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Services
{
    public class AccountingService
    {
        public void EnsureInitialized(StockData data)
        {
            if (data.Accounts == null) data.Accounts = new List<Account>();
            if (data.BankAccounts == null) data.BankAccounts = new List<BankAccount>();
            if (data.JournalEntries == null) data.JournalEntries = new List<JournalEntry>();
            if (data.CustomerPayments == null) data.CustomerPayments = new List<CustomerPayment>();
            if (data.BusinessExpenses == null) data.BusinessExpenses = new List<BusinessExpense>();

            if (data.Accounts.Count == 0)
                SeedChartOfAccounts(data);
            else
                EnsureExpenseAccounts(data);

            BackfillSalePostings(data);
        }

        public void SeedChartOfAccounts(StockData data)
        {
            data.Accounts.AddRange(new[]
            {
                CreateAccount(SystemAccounts.CashId, SystemAccounts.CashCode, "Cash", AccountType.Asset),
                CreateAccount(SystemAccounts.BankId, SystemAccounts.BankCode, "Bank", AccountType.Asset),
                CreateAccount(SystemAccounts.AccountsReceivableId, SystemAccounts.AccountsReceivableCode, "Accounts Receivable", AccountType.Asset),
                CreateAccount(SystemAccounts.OwnerEquityId, SystemAccounts.OwnerEquityCode, "Owner Equity", AccountType.Equity),
                CreateAccount(SystemAccounts.SalesRevenueId, SystemAccounts.SalesRevenueCode, "Sales Revenue", AccountType.Income)
            });
            EnsureExpenseAccounts(data);
        }

        public void EnsureExpenseAccounts(StockData data)
        {
            AddExpenseAccountIfMissing(data, SystemAccounts.FactoryRentId, SystemAccounts.FactoryRentCode, "Factory Rent");
            AddExpenseAccountIfMissing(data, SystemAccounts.EmployeeSalaryId, SystemAccounts.EmployeeSalaryCode, "Employee Salary");
            AddExpenseAccountIfMissing(data, SystemAccounts.SnacksId, SystemAccounts.SnacksCode, "Snacks & Refreshments");
            AddExpenseAccountIfMissing(data, SystemAccounts.TransportId, SystemAccounts.TransportCode, "Transport");
            AddExpenseAccountIfMissing(data, SystemAccounts.OtherExpenseId, SystemAccounts.OtherExpenseCode, "Other Expense");
        }

        private static void AddExpenseAccountIfMissing(StockData data, Guid id, string code, string name)
        {
            if (data.Accounts.Any(a => a.Id == id))
                return;

            data.Accounts.Add(CreateAccount(id, code, name, AccountType.Expense));
        }

        private static Account CreateAccount(Guid id, string code, string name, AccountType type) =>
            new Account
            {
                Id = id,
                Code = code,
                Name = name,
                Type = type,
                IsSystem = true,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

        public void BackfillSalePostings(StockData data)
        {
            foreach (var invoice in data.Invoices.Where(i => i.IsActive && i.CustomerId.HasValue && i.TotalAmount > 0))
            {
                if (HasJournalForReference(data, JournalReferenceType.Invoice, invoice.Id))
                    continue;

                PostSale(data, invoice);
            }
        }

        public void PostSale(StockData data, Invoice invoice)
        {
            if (invoice == null || invoice.TotalAmount <= 0)
                return;

            if (HasJournalForReference(data, JournalReferenceType.Invoice, invoice.Id))
                return;

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            var sales = GetAccount(data, SystemAccounts.SalesRevenueId);
            if (ar == null || sales == null)
                return;

            var entry = new JournalEntry
            {
                EntryDate = invoice.CreatedAt,
                ReferenceType = JournalReferenceType.Invoice,
                ReferenceId = invoice.Id,
                ReferenceNumber = invoice.InvoiceNumber,
                Description = $"Sale - {invoice.InvoiceNumber}",
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(ar, invoice.CustomerId, invoice.CustomerName, invoice.TotalAmount, 0),
                    CreateLine(sales, null, null, 0, invoice.TotalAmount)
                }
            };

            data.JournalEntries.Insert(0, entry);
        }

        public string ValidateVoidSale(StockData data, Invoice invoice)
        {
            if (invoice == null)
                return "Invoice not found.";

            if (!invoice.IsActive)
                return "Invoice is already voided.";

            if (!HasJournalForReference(data, JournalReferenceType.Invoice, invoice.Id))
                return "Sale journal entry not found for this invoice.";

            if (HasUnsettledInvoiceVoid(data, invoice.Id))
                return "Invoice has already been reversed in accounts.";

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            var sales = GetAccount(data, SystemAccounts.SalesRevenueId);
            if (ar == null || sales == null)
                return "Required accounts are missing.";

            return null;
        }

        public string VoidSale(StockData data, Invoice invoice, string reason)
        {
            var validationError = ValidateVoidSale(data, invoice);
            if (validationError != null)
                return validationError;

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            var sales = GetAccount(data, SystemAccounts.SalesRevenueId);

            var entry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                ReferenceType = JournalReferenceType.InvoiceVoid,
                ReferenceId = invoice.Id,
                ReferenceNumber = invoice.InvoiceNumber,
                Description = $"Void sale - {invoice.InvoiceNumber}" + (string.IsNullOrWhiteSpace(reason) ? string.Empty : $" ({reason.Trim()})"),
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(ar, invoice.CustomerId, invoice.CustomerName, 0, invoice.TotalAmount),
                    CreateLine(sales, null, null, invoice.TotalAmount, 0)
                }
            };

            data.JournalEntries.Insert(0, entry);
            return null;
        }

        public string ValidateVoidPayment(StockData data, CustomerPayment payment)
        {
            if (payment == null)
                return "Payment not found.";

            if (!payment.IsActive)
                return "Payment is already voided.";

            if (HasUnsettledPaymentVoid(data, payment.Id))
                return "Payment has already been reversed in accounts.";

            var cashAccount = GetAccount(data, payment.CashAccountId);
            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            if (cashAccount == null || ar == null)
                return "Required accounts are missing.";

            return null;
        }

        public string VoidPayment(StockData data, CustomerPayment payment, string reason)
        {
            var validationError = ValidateVoidPayment(data, payment);
            if (validationError != null)
                return validationError;

            var cashAccount = GetAccount(data, payment.CashAccountId);
            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);

            var entry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                ReferenceType = JournalReferenceType.PaymentVoid,
                ReferenceId = payment.Id,
                ReferenceNumber = payment.InvoiceNumber ?? payment.Reference,
                Description = $"Void payment - {payment.CustomerName}" + (string.IsNullOrWhiteSpace(reason) ? string.Empty : $" ({reason.Trim()})"),
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(cashAccount, null, null, 0, payment.Amount),
                    CreateLine(ar, payment.CustomerId, payment.CustomerName, payment.Amount, 0)
                }
            };

            data.JournalEntries.Insert(0, entry);
            payment.IsVoided = true;
            payment.VoidedAt = DateTime.Now;
            return null;
        }

        public string ValidateReinstateSale(StockData data, Invoice invoice)
        {
            if (invoice == null)
                return "Invoice not found.";

            if (invoice.IsActive)
                return "Invoice is already active.";

            if (!HasUnsettledInvoiceVoid(data, invoice.Id))
                return "This invoice is not voided in accounts or has already been restored.";

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            var sales = GetAccount(data, SystemAccounts.SalesRevenueId);
            if (ar == null || sales == null)
                return "Required accounts are missing.";

            return null;
        }

        public string ReinstateSale(StockData data, Invoice invoice, string reason)
        {
            var validationError = ValidateReinstateSale(data, invoice);
            if (validationError != null)
                return validationError;

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            var sales = GetAccount(data, SystemAccounts.SalesRevenueId);

            var entry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                ReferenceType = JournalReferenceType.InvoiceReinstate,
                ReferenceId = invoice.Id,
                ReferenceNumber = invoice.InvoiceNumber,
                Description = $"Restore sale - {invoice.InvoiceNumber}" + (string.IsNullOrWhiteSpace(reason) ? string.Empty : $" ({reason.Trim()})"),
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(ar, invoice.CustomerId, invoice.CustomerName, invoice.TotalAmount, 0),
                    CreateLine(sales, null, null, 0, invoice.TotalAmount)
                }
            };

            data.JournalEntries.Insert(0, entry);
            return null;
        }

        public string ValidateReinstatePayment(StockData data, CustomerPayment payment)
        {
            if (payment == null)
                return "Payment not found.";

            if (payment.IsActive)
                return "Payment is already active.";

            if (!HasUnsettledPaymentVoid(data, payment.Id))
                return "This payment is not voided in accounts or has already been restored.";

            var cashAccount = GetAccount(data, payment.CashAccountId);
            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            if (cashAccount == null || ar == null)
                return "Required accounts are missing.";

            return null;
        }

        public string ReinstatePayment(StockData data, CustomerPayment payment, Invoice invoice, string reason)
        {
            var validationError = ValidateReinstatePayment(data, payment);
            if (validationError != null)
                return validationError;

            var cashAccount = GetAccount(data, payment.CashAccountId);
            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);

            var entry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                ReferenceType = JournalReferenceType.PaymentReinstate,
                ReferenceId = payment.Id,
                ReferenceNumber = payment.InvoiceNumber ?? payment.Reference,
                Description = $"Restore payment - {payment.CustomerName}" + (string.IsNullOrWhiteSpace(reason) ? string.Empty : $" ({reason.Trim()})"),
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(cashAccount, null, null, payment.Amount, 0),
                    CreateLine(ar, payment.CustomerId, payment.CustomerName, 0, payment.Amount)
                }
            };

            data.JournalEntries.Insert(0, entry);
            payment.IsVoided = false;
            payment.VoidedAt = null;

            if (invoice != null)
                invoice.AmountPaid += payment.Amount;

            return null;
        }

        public string ReceivePayment(StockData data, CustomerPayment payment) =>
            RecordPayment(data, payment, updateInvoiceAmounts: true);

        public string RecordPayment(StockData data, CustomerPayment payment, bool updateInvoiceAmounts = true)
        {
            if (payment == null)
                return "Invalid payment.";

            if (payment.Amount <= 0)
                return "Payment amount must be greater than zero.";

            var customer = data.Customers.FirstOrDefault(c => c.Id == payment.CustomerId);
            if (customer == null)
                return "Customer not found.";

            payment.CustomerName = customer.Name;

            var cashAccount = GetAccount(data, payment.CashAccountId);
            if (cashAccount == null || !IsCashOrBankAccount(data, cashAccount.Id))
                return "Select a cash or bank account.";

            payment.CashAccountName = cashAccount.Name;

            Invoice invoice = null;
            if (payment.IsAdvance)
            {
                payment.InvoiceId = null;
                payment.InvoiceNumber = "ADVANCE";
            }
            else if (payment.InvoiceId.HasValue)
            {
                invoice = data.Invoices.FirstOrDefault(i => i.Id == payment.InvoiceId.Value);
                if (invoice == null)
                    return "Invoice not found.";

                if (!invoice.IsActive)
                    return "Cannot record payment against a voided invoice.";

                if (invoice.CustomerId != payment.CustomerId)
                    return "Invoice does not belong to the selected customer.";

                if (updateInvoiceAmounts && payment.Amount > invoice.BalanceDue)
                    return $"Payment exceeds balance due ({invoice.BalanceDue:C2}).";

                payment.InvoiceNumber = invoice.InvoiceNumber;
            }
            else if (updateInvoiceAmounts)
            {
                var openBalance = GetCustomerBalance(data, payment.CustomerId);
                if (openBalance > 0 && payment.Amount > openBalance)
                    return $"Payment exceeds customer balance ({openBalance:C2}).";
            }

            var ar = GetAccount(data, SystemAccounts.AccountsReceivableId);
            if (ar == null)
                return "Accounts Receivable account is missing.";

            var entry = new JournalEntry
            {
                EntryDate = payment.PaidAt,
                ReferenceType = JournalReferenceType.Payment,
                ReferenceId = payment.Id,
                ReferenceNumber = payment.InvoiceNumber ?? payment.Reference,
                Description = !updateInvoiceAmounts
                    ? $"Payment at sale - {customer.Name}"
                    : payment.IsAdvance
                        ? $"Advance payment received - {customer.Name}"
                        : $"Payment received - {customer.Name}",
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(cashAccount, null, null, payment.Amount, 0),
                    CreateLine(ar, payment.CustomerId, customer.Name, 0, payment.Amount)
                }
            };

            data.JournalEntries.Insert(0, entry);
            data.CustomerPayments.Insert(0, payment);

            if (!updateInvoiceAmounts)
                return null;

            if (payment.IsAdvance)
                return null;

            if (invoice != null)
                invoice.AmountPaid += payment.Amount;
            else
                AllocatePaymentToInvoices(data, payment);

            return null;
        }

        public string RecordExpense(StockData data, BusinessExpense expense)
        {
            if (expense == null)
                return "Invalid expense.";

            if (expense.Amount <= 0)
                return "Expense amount must be greater than zero.";

            var expenseAccount = GetAccount(data, expense.ExpenseAccountId);
            if (expenseAccount == null || expenseAccount.Type != AccountType.Expense)
                return "Select an expense category.";

            expense.ExpenseAccountName = expenseAccount.Name;

            var cashAccount = GetAccount(data, expense.CashAccountId);
            if (cashAccount == null || !IsCashOrBankAccount(data, cashAccount.Id))
                return "Select a cash or bank account.";

            expense.CashAccountName = cashAccount.Name;

            var entry = new JournalEntry
            {
                EntryDate = expense.PaidAt,
                ReferenceType = JournalReferenceType.Expense,
                ReferenceId = expense.Id,
                ReferenceNumber = expense.Reference,
                Description = $"Expense - {expenseAccount.Name}",
                CreatedAt = DateTime.Now,
                Lines = new List<JournalLine>
                {
                    CreateLine(expenseAccount, null, null, expense.Amount, 0),
                    CreateLine(cashAccount, null, null, 0, expense.Amount)
                }
            };

            data.JournalEntries.Insert(0, entry);
            data.BusinessExpenses.Insert(0, expense);
            return null;
        }

        public IEnumerable<Account> GetExpenseAccounts(StockData data) =>
            data.Accounts
                .Where(a => a.IsActive && a.Type == AccountType.Expense)
                .OrderBy(a => a.Code);

        public IEnumerable<BusinessExpense> GetBusinessExpenses(
            StockData data,
            DateTime? from = null,
            DateTime? to = null,
            string term = null,
            Guid? expenseAccountId = null)
        {
            var query = data.BusinessExpenses.AsEnumerable();

            if (from.HasValue)
                query = query.Where(e => e.PaidAt.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(e => e.PaidAt.Date <= to.Value.Date);

            if (expenseAccountId.HasValue)
                query = query.Where(e => e.ExpenseAccountId == expenseAccountId.Value);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(e =>
                    (e.ExpenseAccountName != null && e.ExpenseAccountName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (e.Reference != null && e.Reference.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (e.Notes != null && e.Notes.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (e.CashAccountName != null && e.CashAccountName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return query.OrderByDescending(e => e.PaidAt);
        }

        public decimal GetTotalExpenses(StockData data, DateTime? from = null, DateTime? to = null) =>
            GetBusinessExpenses(data, from, to).Sum(e => e.Amount);

        private static void AllocatePaymentToInvoices(StockData data, CustomerPayment payment)
        {
            var remaining = payment.Amount;
            foreach (var invoice in data.Invoices
                .Where(i => i.IsActive && i.CustomerId == payment.CustomerId && i.BalanceDue > 0)
                .OrderBy(i => i.CreatedAt))
            {
                if (remaining <= 0)
                    break;

                var applied = Math.Min(remaining, invoice.BalanceDue);
                invoice.AmountPaid += applied;
                remaining -= applied;
            }
        }

        public decimal GetAccountBalance(StockData data, Guid accountId)
        {
            var account = GetAccount(data, accountId);
            if (account == null)
                return 0;

            decimal debits = 0;
            decimal credits = 0;
            foreach (var line in GetAllLines(data).Where(l => l.AccountId == accountId))
            {
                debits += line.Debit;
                credits += line.Credit;
            }

            return IsDebitNormal(account.Type) ? debits - credits : credits - debits;
        }

        public decimal GetCustomerBalance(StockData data, Guid customerId)
        {
            var invoiced = data.Invoices
                .Where(i => i.IsActive && i.CustomerId == customerId)
                .Sum(i => i.TotalAmount);
            var paid = (data.CustomerPayments ?? Enumerable.Empty<CustomerPayment>())
                .Where(p => p.IsActive && p.CustomerId == customerId)
                .Sum(p => p.Amount);
            return invoiced - paid;
        }

        public decimal GetTotalOutstanding(StockData data) =>
            data.Customers.Sum(c => Math.Max(0, GetCustomerBalance(data, c.Id)));

        public IEnumerable<CustomerDueRow> GetCustomerDueReport(StockData data, string term = null)
        {
            var rows = data.Customers.Select(customer =>
            {
                var invoices = data.Invoices.Where(i => i.IsActive && i.CustomerId == customer.Id).ToList();
                var totalInvoiced = invoices.Sum(i => i.TotalAmount);
                var totalPaid = (data.CustomerPayments ?? Enumerable.Empty<CustomerPayment>())
                    .Where(p => p.IsActive && p.CustomerId == customer.Id)
                    .Sum(p => p.Amount);
                var balance = totalInvoiced - totalPaid;
                return new CustomerDueRow
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    Phone = customer.Phone ?? string.Empty,
                    TotalInvoiced = totalInvoiced,
                    TotalPaid = totalPaid,
                    BalanceDue = balance,
                    OpenInvoiceCount = invoices.Count(i => i.BalanceDue > 0)
                };
            }).Where(r => r.TotalInvoiced > 0 || r.TotalPaid > 0 || r.BalanceDue != 0);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                rows = rows.Where(r =>
                    (r.CustomerName != null && r.CustomerName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r.Phone != null && r.Phone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return rows.OrderByDescending(r => r.BalanceDue).ThenBy(r => r.CustomerName);
        }

        public IEnumerable<CustomerLedgerRow> GetCustomerLedger(StockData data, Guid customerId)
        {
            var entries = new List<(DateTime SortDate, int SortOrder, CustomerLedgerRow Row)>();

            foreach (var invoice in data.Invoices.Where(i => i.CustomerId == customerId && i.IsActive))
            {
                entries.Add((invoice.CreatedAt, 0, new CustomerLedgerRow
                {
                    Date = invoice.CreatedAt,
                    EntryType = "Debit",
                    Description = BuildInvoiceDescription(invoice),
                    Reference = invoice.InvoiceNumber ?? string.Empty,
                    Debit = invoice.TotalAmount,
                    Credit = 0
                }));
            }

            foreach (var payment in data.CustomerPayments.Where(p => p.CustomerId == customerId && p.IsActive))
            {
                entries.Add((payment.PaidAt, 1, new CustomerLedgerRow
                {
                    Date = payment.PaidAt,
                    EntryType = "Credit",
                    Description = BuildPaymentDescription(payment),
                    Reference = GetPaymentReference(payment),
                    Debit = 0,
                    Credit = payment.Amount,
                    PaymentId = payment.Id,
                    HasReceipt = payment.HasReceipt
                }));
            }

            decimal balance = 0;
            foreach (var entry in entries.OrderBy(e => e.SortDate).ThenBy(e => e.SortOrder))
            {
                balance += entry.Row.Debit - entry.Row.Credit;
                entry.Row.Balance = balance;
                yield return entry.Row;
            }
        }

        private static string BuildInvoiceDescription(Invoice invoice)
        {
            if (invoice?.Items == null || invoice.Items.Count == 0)
                return "Sale invoice (due added)";

            if (invoice.Items.Count == 1)
            {
                var item = invoice.Items[0];
                var name = item.ProductName ?? item.ProductSku ?? "item";
                return $"Sale: {name} x{item.Quantity}";
            }

            return $"Sale invoice ({invoice.Items.Count} items)";
        }

        private static string BuildPaymentDescription(CustomerPayment payment)
        {
            var method = string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Payment" : payment.PaymentMethod;
            if (payment.IsAdvance)
                return $"{method} advance payment received";

            if (!string.IsNullOrWhiteSpace(payment.InvoiceNumber))
                return $"{method} received against {payment.InvoiceNumber}";

            return $"{method} received";
        }

        private static string GetPaymentReference(CustomerPayment payment)
        {
            if (!string.IsNullOrWhiteSpace(payment.Reference))
                return payment.Reference;

            if (payment.IsAdvance)
                return "ADVANCE";

            if (!string.IsNullOrWhiteSpace(payment.InvoiceNumber))
                return payment.InvoiceNumber;

            return payment.Id.ToString("N").Substring(0, 8).ToUpperInvariant();
        }

        public IEnumerable<CashLedgerRow> GetCashLedger(
            StockData data,
            Guid? accountId = null,
            DateTime? from = null,
            DateTime? to = null,
            Guid? customerId = null,
            Guid? expenseAccountId = null)
        {
            var cashAccountIds = GetLiquidAccountIds(data);
            if (accountId.HasValue)
                cashAccountIds = new HashSet<Guid> { accountId.Value };

            var expenseAccountIds = new HashSet<Guid>(
                data.Accounts.Where(a => a.IsActive && a.Type == AccountType.Expense).Select(a => a.Id));

            var accountNames = data.Accounts.ToDictionary(a => a.Id, a => a.Name);
            var rows = new List<(DateTime Date, int Sort, CashLedgerRow Row)>();

            foreach (var entry in data.JournalEntries.OrderBy(e => e.EntryDate).ThenBy(e => e.CreatedAt))
            {
                if (from.HasValue && entry.EntryDate.Date < from.Value.Date)
                    continue;
                if (to.HasValue && entry.EntryDate.Date > to.Value.Date)
                    continue;

                if (customerId.HasValue)
                {
                    var entryCustomerId = ResolveLedgerCustomerId(entry);
                    if (!entryCustomerId.HasValue || entryCustomerId.Value != customerId.Value)
                        continue;
                }

                var expenseLine = entry.Lines?.FirstOrDefault(l => expenseAccountIds.Contains(l.AccountId));
                if (expenseAccountId.HasValue && expenseAccountId.Value != Guid.Empty)
                {
                    if (expenseLine == null || expenseLine.AccountId != expenseAccountId.Value)
                        continue;
                }

                foreach (var line in entry.Lines.Where(l => cashAccountIds.Contains(l.AccountId)))
                {
                    rows.Add((entry.EntryDate, 0, new CashLedgerRow
                    {
                        Date = entry.EntryDate,
                        AccountName = line.AccountName,
                        Description = entry.Description,
                        Reference = entry.ReferenceNumber ?? string.Empty,
                        Debit = line.Debit,
                        Credit = line.Credit
                    }));
                }
            }

            // Supplier payments are record-only (no AP journal) but still leave the bank/cash.
            // Skip when customer/expense filters are active — those views are journal-only.
            if (!customerId.HasValue
                && (!expenseAccountId.HasValue || expenseAccountId.Value == Guid.Empty))
            {
                foreach (var payment in (data.SupplierPayments ?? Enumerable.Empty<SupplierPayment>())
                    .Where(p => p.IsActive && p.Amount > 0 && cashAccountIds.Contains(p.CashAccountId)))
                {
                    if (from.HasValue && payment.PaidAt.Date < from.Value.Date)
                        continue;
                    if (to.HasValue && payment.PaidAt.Date > to.Value.Date)
                        continue;

                    string accountName;
                    if (!accountNames.TryGetValue(payment.CashAccountId, out accountName)
                        || string.IsNullOrWhiteSpace(accountName))
                        accountName = payment.CashAccountName ?? "Cash/Bank";

                    var method = string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Payment" : payment.PaymentMethod;
                    rows.Add((payment.PaidAt, 1, new CashLedgerRow
                    {
                        Date = payment.PaidAt,
                        AccountName = accountName,
                        Description = $"{method} paid to supplier {payment.SupplierName}",
                        Reference = payment.Reference ?? string.Empty,
                        Debit = 0,
                        Credit = payment.Amount
                    }));
                }
            }

            decimal running = 0;
            var ordered = rows.OrderBy(r => r.Date).ThenBy(r => r.Sort).Select(r => r.Row).ToList();
            foreach (var row in ordered)
            {
                running += row.Debit - row.Credit;
                row.RunningBalance = running;
            }

            return ordered;
        }

        private static Guid? ResolveLedgerCustomerId(JournalEntry entry) =>
            entry?.Lines?.FirstOrDefault(l => l.CustomerId.HasValue)?.CustomerId;

        public bool IsCashOrBankAccount(StockData data, Guid accountId)
        {
            if (accountId == SystemAccounts.CashId || accountId == SystemAccounts.BankId)
                return true;

            return data?.BankAccounts != null
                && data.BankAccounts.Any(b => b.IsActive && b.GlAccountId == accountId);
        }

        public HashSet<Guid> GetLiquidAccountIds(StockData data)
        {
            var ids = new HashSet<Guid> { SystemAccounts.CashId, SystemAccounts.BankId };
            if (data?.BankAccounts == null)
                return ids;

            foreach (var bank in data.BankAccounts.Where(b => b.IsActive))
                ids.Add(bank.GlAccountId);

            return ids;
        }

        public IEnumerable<Account> GetCashAndBankAccounts(StockData data)
        {
            var ids = GetLiquidAccountIds(data);
            return data.Accounts
                .Where(a => a.IsActive && ids.Contains(a.Id))
                .OrderBy(a => a.Id == SystemAccounts.CashId ? 0 : a.Id == SystemAccounts.BankId ? 1 : 2)
                .ThenBy(a => a.Name);
        }

        public IEnumerable<BankLedgerRow> GetBankLedger(StockData data, Guid glAccountId)
        {
            var rows = new List<(DateTime Date, int Sort, BankLedgerRow Row)>();

            foreach (var entry in data.JournalEntries ?? Enumerable.Empty<JournalEntry>())
            {
                foreach (var line in entry.Lines ?? Enumerable.Empty<JournalLine>())
                {
                    if (line.AccountId != glAccountId)
                        continue;

                    rows.Add((entry.EntryDate, 0, new BankLedgerRow
                    {
                        Date = entry.EntryDate,
                        EntryType = line.Debit > 0 ? "In" : "Out",
                        Description = entry.Description ?? string.Empty,
                        Reference = entry.ReferenceNumber ?? string.Empty,
                        Debit = line.Debit,
                        Credit = line.Credit
                    }));
                }
            }

            // Supplier payments are record-only (no AP journal) but still reduce bank cash.
            foreach (var payment in (data.SupplierPayments ?? Enumerable.Empty<SupplierPayment>())
                .Where(p => p.IsActive && p.CashAccountId == glAccountId && p.Amount > 0))
            {
                var method = string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Payment" : payment.PaymentMethod;
                rows.Add((payment.PaidAt, 1, new BankLedgerRow
                {
                    Date = payment.PaidAt,
                    EntryType = "Out",
                    Description = $"{method} to supplier {payment.SupplierName}",
                    Reference = string.IsNullOrWhiteSpace(payment.Reference)
                        ? payment.Id.ToString("N").Substring(0, 8).ToUpperInvariant()
                        : payment.Reference,
                    Debit = 0,
                    Credit = payment.Amount
                }));
            }

            decimal running = 0;
            var ordered = rows.OrderBy(r => r.Date).ThenBy(r => r.Sort).Select(r => r.Row).ToList();
            foreach (var row in ordered)
            {
                running += row.Debit - row.Credit;
                row.Balance = running;
            }

            return ordered;
        }

        public decimal GetBankDisplayBalance(StockData data, Guid glAccountId)
        {
            var journalBalance = GetAccountBalance(data, glAccountId);
            var supplierOut = (data.SupplierPayments ?? Enumerable.Empty<SupplierPayment>())
                .Where(p => p.IsActive && p.CashAccountId == glAccountId)
                .Sum(p => p.Amount);
            return journalBalance - supplierOut;
        }

        public IEnumerable<Invoice> GetOpenInvoices(StockData data, Guid customerId) =>
            data.Invoices
                .Where(i => i.IsActive && i.CustomerId == customerId && i.BalanceDue > 0)
                .OrderByDescending(i => i.CreatedAt);

        public SalesProfitSummary GetSalesProfitSummary(StockData data, DateTime? from = null, DateTime? to = null, string term = null)
        {
            var lines = GetSalesProfitLines(data, from, to, term).ToList();
            var totalSales = lines.Sum(l => l.SaleAmount);
            var totalCost = lines.Sum(l => l.CostAmount);
            var profit = totalSales - totalCost;

            return new SalesProfitSummary
            {
                TotalSales = totalSales,
                TotalCost = totalCost,
                GrossProfit = profit,
                MarginPercent = totalSales > 0 ? profit / totalSales * 100 : 0
            };
        }

        public IEnumerable<SalesProfitLine> GetSalesProfitLines(StockData data, DateTime? from = null, DateTime? to = null, string term = null)
        {
            term = term?.Trim();
            var rows = new List<SalesProfitLine>();

            foreach (var invoice in data.Invoices.Where(i => i.IsActive).OrderByDescending(i => i.CreatedAt))
            {
                if (from.HasValue && invoice.CreatedAt.Date < from.Value.Date)
                    continue;
                if (to.HasValue && invoice.CreatedAt.Date > to.Value.Date)
                    continue;

                if (!string.IsNullOrWhiteSpace(term))
                {
                    var matchesInvoice =
                        (invoice.InvoiceNumber != null && invoice.InvoiceNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (invoice.CustomerName != null && invoice.CustomerName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!matchesInvoice && (invoice.Items == null || !invoice.Items.Any(item =>
                            (item.ProductName != null && item.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (item.ProductSku != null && item.ProductSku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))))
                        continue;
                }

                var items = (invoice.Items ?? Enumerable.Empty<InvoiceLineItem>()).ToList();
                var subTotal = items.Sum(i => i.LineTotal);
                var discount = Math.Max(0, invoice.DiscountAmount);
                decimal allocatedDiscount = 0;

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var unitCost = ResolveLineUnitCost(data, item);
                    var costAmount = unitCost * item.Quantity;
                    decimal lineDiscount = 0;
                    if (discount > 0 && subTotal > 0)
                    {
                        if (i == items.Count - 1)
                            lineDiscount = discount - allocatedDiscount;
                        else
                        {
                            lineDiscount = Math.Round(discount * (item.LineTotal / subTotal), 2, MidpointRounding.AwayFromZero);
                            allocatedDiscount += lineDiscount;
                        }
                    }

                    var saleAmount = Math.Max(0, item.LineTotal - lineDiscount);
                    var profit = saleAmount - costAmount;

                    rows.Add(new SalesProfitLine
                    {
                        Date = invoice.CreatedAt,
                        InvoiceNumber = invoice.InvoiceNumber,
                        CustomerName = invoice.CustomerName ?? string.Empty,
                        ProductName = item.ProductName ?? item.ProductSku ?? string.Empty,
                        Quantity = item.Quantity,
                        SaleAmount = saleAmount,
                        CostAmount = costAmount,
                        Profit = profit,
                        MarginPercent = saleAmount > 0 ? profit / saleAmount * 100 : 0
                    });
                }
            }

            return rows;
        }

        public static decimal ResolveLineUnitCost(StockData data, InvoiceLineItem line)
        {
            if (line == null)
                return 0;

            if (line.UnitCost > 0)
                return line.UnitCost;

            var product = data.Products.FirstOrDefault(p => p.Id == line.ProductId);
            return ResolveProductSaleCost(data, product);
        }

        public static decimal ResolveProductSaleCost(StockData data, Product product)
        {
            if (product == null)
                return 0;

            if (product.ProductType == ProductType.RawMaterial)
                return product.UnitPrice;

            if (product.UnitCost > 0)
                return product.UnitCost;

            return CalculateRecipeUnitCost(data, product.Id);
        }

        public static decimal CalculateRecipeUnitCost(StockData data, Guid finishedProductId)
        {
            var recipe = data.ProductionRecipes?.FirstOrDefault(r => r.OutputProductId == finishedProductId);
            if (recipe?.Materials == null || recipe.Materials.Count == 0)
                return 0;

            decimal cost = 0;
            foreach (var material in recipe.Materials)
            {
                var rawMaterial = data.Products.FirstOrDefault(p => p.Id == material.ProductId);
                if (rawMaterial == null)
                    continue;

                cost += rawMaterial.UnitPrice * material.QuantityPerUnit;
            }

            return cost;
        }

        private static bool HasJournalForReference(StockData data, JournalReferenceType type, Guid referenceId) =>
            data.JournalEntries.Any(e => e.ReferenceType == type && e.ReferenceId == referenceId);

        private static int CountJournalsForReference(StockData data, JournalReferenceType type, Guid referenceId) =>
            data.JournalEntries.Count(e => e.ReferenceType == type && e.ReferenceId == referenceId);

        private static bool HasUnsettledInvoiceVoid(StockData data, Guid invoiceId)
        {
            var voidCount = CountJournalsForReference(data, JournalReferenceType.InvoiceVoid, invoiceId);
            var reinstateCount = CountJournalsForReference(data, JournalReferenceType.InvoiceReinstate, invoiceId);
            return voidCount > reinstateCount;
        }

        private static bool HasUnsettledPaymentVoid(StockData data, Guid paymentId)
        {
            var voidCount = CountJournalsForReference(data, JournalReferenceType.PaymentVoid, paymentId);
            var reinstateCount = CountJournalsForReference(data, JournalReferenceType.PaymentReinstate, paymentId);
            return voidCount > reinstateCount;
        }

        private static Account GetAccount(StockData data, Guid accountId) =>
            data.Accounts.FirstOrDefault(a => a.Id == accountId);

        private static IEnumerable<JournalLine> GetAllLines(StockData data) =>
            data.JournalEntries.SelectMany(e => e.Lines ?? Enumerable.Empty<JournalLine>());

        private static JournalLine CreateLine(Account account, Guid? customerId, string customerName, decimal debit, decimal credit) =>
            new JournalLine
            {
                AccountId = account.Id,
                AccountCode = account.Code,
                AccountName = account.Name,
                CustomerId = customerId,
                CustomerName = customerName ?? string.Empty,
                Debit = debit,
                Credit = credit
            };

        private static bool IsDebitNormal(AccountType type) =>
            type == AccountType.Asset || type == AccountType.Expense;
    }
}
