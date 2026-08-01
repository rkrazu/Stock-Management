using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Stock_Managemnet.Data;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Services
{
    public class StockRepository
    {
        private readonly StockDatabase _database = new StockDatabase();
        private readonly AccountingService _accounting = new AccountingService();

        public StockData Data { get; private set; } = new StockData();

        public void Load()
        {
            try
            {
                DatabaseInitializer.EnsureCreated();
                Data = _database.LoadAll();

                if (Data.Products == null) Data.Products = new List<Product>();
                if (Data.Customers == null) Data.Customers = new List<Customer>();
                if (Data.Suppliers == null) Data.Suppliers = new List<Supplier>();
                if (Data.Transactions == null) Data.Transactions = new List<StockTransaction>();
                if (Data.ProductionRecipes == null) Data.ProductionRecipes = new List<ProductionRecipe>();
                if (Data.ProductionOrders == null) Data.ProductionOrders = new List<ProductionOrder>();
                if (Data.Invoices == null) Data.Invoices = new List<Invoice>();
                if (Data.Accounts == null) Data.Accounts = new List<Account>();
                if (Data.BankAccounts == null) Data.BankAccounts = new List<BankAccount>();
                if (Data.JournalEntries == null) Data.JournalEntries = new List<JournalEntry>();
                if (Data.CustomerPayments == null) Data.CustomerPayments = new List<CustomerPayment>();
                if (Data.SupplierPayments == null) Data.SupplierPayments = new List<SupplierPayment>();
                if (Data.BusinessExpenses == null) Data.BusinessExpenses = new List<BusinessExpense>();
                if (Data.NextInvoiceNumber < 1) Data.NextInvoiceNumber = 1;
                if (Data.NextProductionNumber < 1) Data.NextProductionNumber = 1;

                var accountsBefore = Data.Accounts.Count;
                var journalsBefore = Data.JournalEntries.Count;
                _accounting.EnsureInitialized(Data);
                if (Data.Accounts.Count > accountsBefore || Data.JournalEntries.Count > journalsBefore)
                    Save();

                EnsureInvoiceSequence();
                EnsureProductionSequence();
                ReceiptStorageService.EnsureRootExists();
            }
            catch (Exception ex)
            {
                Data = new StockData();
                MessageBox.Show(
                    "Could not connect to SQL Server or load data.\r\n\r\n" + ex.Message +
                    "\r\n\r\nCheck App.config connection string and ensure SQL Server is running.",
                    BrandAssets.AppDisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        public void Save()
        {
            _database.SaveAll(Data);
        }

        public Product GetProduct(Guid id) =>
            Data.Products.FirstOrDefault(p => p.Id == id);

        public bool SkuExists(string sku, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return false;
            return Data.Products.Any(p =>
                string.Equals(p.Sku?.Trim(), sku.Trim(), StringComparison.OrdinalIgnoreCase) &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public void AddProduct(Product product)
        {
            product.LastUpdated = DateTime.Now;
            Data.Products.Add(product);
            Save();
        }

        public void UpdateProduct(Product product)
        {
            var existing = GetProduct(product.Id);
            if (existing == null) return;

            existing.Sku = product.Sku;
            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.ProductType = product.ProductType;
            existing.UnitPrice = product.UnitPrice;
            existing.ReorderLevel = product.ReorderLevel;
            existing.LastUpdated = DateTime.Now;
            Save();
        }

        public void DeleteProduct(Guid id)
        {
            // Only remove the product master record.
            // Keep invoices, line items, payments, journals, and stock history —
            // those rows already store product name/SKU snapshots and must remain
            // for account balance / customer due integrity.
            if (GetProduct(id) == null)
                return;

            Data.Products.RemoveAll(p => p.Id == id);

            // Recipes referencing this product can no longer be produced.
            Data.ProductionRecipes.RemoveAll(r => r.OutputProductId == id);
            foreach (var recipe in Data.ProductionRecipes)
                recipe.Materials?.RemoveAll(m => m.ProductId == id);

            Save();
        }

        public string ValidateStockOut(StockOutRequest request)
        {
            if (request == null)
                return "Invalid stock out request.";

            if (request.Lines == null || request.Lines.Count == 0)
                return "Add at least one product.";

            foreach (var line in request.Lines)
            {
                if (line.Quantity <= 0)
                    return "Quantity must be greater than zero.";

                var product = GetProduct(line.ProductId);
                if (product == null)
                    return "Product not found.";

                if (product.Quantity < line.Quantity)
                    return $"Insufficient stock for {product.Name}. Available: {product.Quantity}";
            }

            if (request.CustomerId.HasValue && GetCustomer(request.CustomerId.Value) == null)
                return "Customer not found.";

            if (!request.CustomerId.HasValue)
                return "Select a customer for sales stock out.";

            var totalAmount = ComputeStockOutTotal(request);
            if (request.AmountPaidAtSale < 0)
                return "Paid amount cannot be negative.";

            if (request.AmountPaidAtSale > totalAmount)
                return $"Paid amount cannot exceed invoice total ({totalAmount:C2}).";

            if (request.AmountPaidAtSale > 0)
            {
                if (!request.CashAccountId.HasValue)
                    return "Select a cash or bank account for the payment received.";

                var cashAccount = Data.Accounts.FirstOrDefault(a => a.Id == request.CashAccountId.Value);
                if (cashAccount == null || !_accounting.IsCashOrBankAccount(Data, cashAccount.Id))
                    return "Select a valid cash or bank account.";
            }

            return null;
        }

        private decimal ComputeStockOutTotal(StockOutRequest request)
        {
            decimal total = 0;
            foreach (var line in request.Lines)
            {
                var product = GetProduct(line.ProductId);
                if (product == null)
                    continue;
                total += product.UnitPrice * line.Quantity;
            }

            return total;
        }

        public Invoice BuildStockOutInvoicePreview(StockOutRequest request)
        {
            var customer = request.CustomerId.HasValue ? GetCustomer(request.CustomerId.Value) : null;
            var items = new List<InvoiceLineItem>();
            decimal totalAmount = 0;

            foreach (var line in request.Lines)
            {
                var product = GetProduct(line.ProductId);
                if (product == null)
                    continue;

                var unitPrice = product.UnitPrice;
                var lineTotal = unitPrice * line.Quantity;
                totalAmount += lineTotal;
                items.Add(new InvoiceLineItem
                {
                    ProductId = product.Id,
                    ProductSku = product.Sku,
                    ProductName = product.Name,
                    ProductCategory = product.Category ?? string.Empty,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    UnitCost = AccountingService.ResolveProductSaleCost(Data, product),
                    LineTotal = lineTotal
                });
            }

            return new Invoice
            {
                CustomerId = customer?.Id,
                CustomerName = customer?.Name,
                CustomerPhone = customer?.Phone,
                CustomerAddress = customer?.Address,
                Notes = request.Notes ?? string.Empty,
                CreatedAt = DateTime.Now,
                TotalAmount = totalAmount,
                AmountPaid = request.AmountPaidAtSale,
                Items = items
            };
        }

        public string CompleteStockOut(StockOutRequest request)
        {
            var error = ValidateStockOut(request);
            if (error != null)
                return error;

            var customer = request.CustomerId.HasValue ? GetCustomer(request.CustomerId.Value) : null;
            var invoiceNumber = GenerateInvoiceNumber();
            var items = new List<InvoiceLineItem>();
            Guid? firstTransactionId = null;
            decimal totalAmount = 0;

            foreach (var line in request.Lines)
            {
                var product = GetProduct(line.ProductId);
                var unitPrice = product.UnitPrice;
                var lineTotal = unitPrice * line.Quantity;
                var transactionId = Guid.NewGuid();
                if (!firstTransactionId.HasValue)
                    firstTransactionId = transactionId;

                product.Quantity -= line.Quantity;
                product.LastUpdated = DateTime.Now;
                totalAmount += lineTotal;

                Data.Transactions.Insert(0, new StockTransaction
                {
                    Id = transactionId,
                    InvoiceNumber = invoiceNumber,
                    IsSale = true,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSku = product.Sku,
                    Type = TransactionType.StockOut,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    TotalValue = lineTotal,
                    Notes = request.Notes ?? string.Empty,
                    CustomerId = customer?.Id,
                    CustomerName = customer?.Name,
                    Timestamp = DateTime.Now
                });

                items.Add(new InvoiceLineItem
                {
                    ProductId = product.Id,
                    ProductSku = product.Sku,
                    ProductName = product.Name,
                    ProductCategory = product.Category ?? string.Empty,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    UnitCost = AccountingService.ResolveProductSaleCost(Data, product),
                    LineTotal = lineTotal
                });
            }

            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                CustomerId = customer?.Id,
                CustomerName = customer?.Name ?? string.Empty,
                CustomerPhone = customer?.Phone ?? string.Empty,
                CustomerAddress = customer?.Address ?? string.Empty,
                Notes = request.Notes ?? string.Empty,
                TotalAmount = totalAmount,
                AmountPaid = request.AmountPaidAtSale,
                TransactionId = firstTransactionId,
                CreatedAt = DateTime.Now,
                Items = items
            };

            Data.Invoices.Insert(0, invoice);
            _accounting.PostSale(Data, invoice);

            if (request.AmountPaidAtSale > 0)
            {
                var paymentError = _accounting.RecordPayment(Data, new CustomerPayment
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    InvoiceId = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    CashAccountId = request.CashAccountId.Value,
                    Amount = request.AmountPaidAtSale,
                    PaymentMethod = "At Sale",
                    Reference = invoice.InvoiceNumber,
                    Notes = "Payment received at sale",
                    PaidAt = DateTime.Now
                }, updateInvoiceAmounts: false);

                if (paymentError != null)
                    return paymentError;
            }

            Save();
            return null;
        }

        public string ReceivePayment(CustomerPayment payment)
        {
            var error = _accounting.ReceivePayment(Data, payment);
            if (error != null)
                return error;

            Save();
            return null;
        }

        public string AttachReceiptToCustomerPayment(CustomerPayment payment, string sourceImagePath)
        {
            if (payment == null)
                return "Invalid payment.";

            try
            {
                if (payment.Id == Guid.Empty)
                    payment.Id = Guid.NewGuid();

                var saved = ReceiptStorageService.SaveImage(sourceImagePath, payment.Id, payment.PaidAt);
                payment.ReceiptRelativePath = saved.RelativePath;
                payment.ReceiptOriginalName = saved.OriginalName;
                payment.ReceiptContentType = saved.ContentType;
                payment.ReceiptSizeBytes = saved.SizeBytes;
                payment.ReceiptSha256 = saved.Sha256;
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AttachReceiptToSupplierPayment(SupplierPayment payment, string sourceImagePath)
        {
            if (payment == null)
                return "Invalid payment.";

            try
            {
                if (payment.Id == Guid.Empty)
                    payment.Id = Guid.NewGuid();

                var saved = ReceiptStorageService.SaveImage(sourceImagePath, payment.Id, payment.PaidAt);
                payment.ReceiptRelativePath = saved.RelativePath;
                payment.ReceiptOriginalName = saved.OriginalName;
                payment.ReceiptContentType = saved.ContentType;
                payment.ReceiptSizeBytes = saved.SizeBytes;
                payment.ReceiptSha256 = saved.Sha256;
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public CustomerPayment GetCustomerPayment(Guid paymentId) =>
            Data.CustomerPayments?.FirstOrDefault(p => p.Id == paymentId);

        public SupplierPayment GetSupplierPayment(Guid paymentId) =>
            Data.SupplierPayments?.FirstOrDefault(p => p.Id == paymentId);

        public string OpenPaymentReceipt(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                    return "No receipt photo is attached.";

                if (!ReceiptStorageService.Exists(relativePath))
                    return "Receipt photo file is missing from disk.";

                ReceiptStorageService.Open(relativePath);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public decimal GetCustomerBalance(Guid customerId) =>
            _accounting.GetCustomerBalance(Data, customerId);

        public decimal GetTotalOutstanding() =>
            _accounting.GetTotalOutstanding(Data);

        public IEnumerable<CustomerDueRow> GetCustomerDueReport(string term = null) =>
            _accounting.GetCustomerDueReport(Data, term);

        public IEnumerable<CustomerLedgerRow> GetCustomerLedger(Guid customerId) =>
            _accounting.GetCustomerLedger(Data, customerId);

        public IEnumerable<CashLedgerRow> GetCashLedger(
            Guid? accountId = null,
            DateTime? from = null,
            DateTime? to = null,
            Guid? customerId = null,
            Guid? expenseAccountId = null) =>
            _accounting.GetCashLedger(Data, accountId, from, to, customerId, expenseAccountId);

        public IEnumerable<Account> GetAccounts() =>
            Data.Accounts.Where(a => a.IsActive).OrderBy(a => a.Code);

        public IEnumerable<Invoice> GetOpenInvoices(Guid customerId) =>
            _accounting.GetOpenInvoices(Data, customerId);

        public IEnumerable<Account> GetCashAndBankAccounts() =>
            _accounting.GetCashAndBankAccounts(Data);

        public IEnumerable<PaymentMethodOption> GetPaymentMethodOptions()
        {
            var options = new List<PaymentMethodOption>
            {
                new PaymentMethodOption
                {
                    CashAccountId = SystemAccounts.CashId,
                    Name = "Cash"
                }
            };

            foreach (var bank in GetBankAccounts())
            {
                options.Add(new PaymentMethodOption
                {
                    CashAccountId = bank.GlAccountId,
                    BankAccountId = bank.Id,
                    Name = bank.Name
                });
            }

            // Keep legacy system Bank available when no named bank accounts exist yet,
            // or always as a fallback destination.
            if (!options.Any(o => o.CashAccountId == SystemAccounts.BankId))
            {
                options.Add(new PaymentMethodOption
                {
                    CashAccountId = SystemAccounts.BankId,
                    Name = "Bank"
                });
            }

            return options;
        }

        public IEnumerable<BankAccount> GetBankAccounts() =>
            (Data.BankAccounts ?? Enumerable.Empty<BankAccount>())
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name);

        public BankAccount GetBankAccount(Guid id) =>
            Data.BankAccounts?.FirstOrDefault(b => b.Id == id);

        public IEnumerable<BankAccountRow> GetBankAccountRows()
        {
            return GetBankAccounts().Select(b => new BankAccountRow
            {
                BankAccountId = b.Id,
                GlAccountId = b.GlAccountId,
                Name = b.Name,
                AccountNumber = b.AccountNumber ?? string.Empty,
                Branch = b.Branch ?? string.Empty,
                Balance = _accounting.GetBankDisplayBalance(Data, b.GlAccountId)
            });
        }

        public IEnumerable<BankLedgerRow> GetBankLedger(Guid bankAccountId)
        {
            var bank = GetBankAccount(bankAccountId);
            if (bank == null)
                return Enumerable.Empty<BankLedgerRow>();

            return _accounting.GetBankLedger(Data, bank.GlAccountId);
        }

        public string AddBankAccount(BankAccount bank)
        {
            if (bank == null)
                return "Invalid bank account.";

            var name = bank.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                return "Bank account name is required.";

            if (Data.BankAccounts.Any(b => b.IsActive
                && string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase)))
                return "A bank account with this name already exists.";

            bank.Id = bank.Id == Guid.Empty ? Guid.NewGuid() : bank.Id;
            bank.Name = name;
            bank.AccountNumber = bank.AccountNumber?.Trim();
            bank.Branch = bank.Branch?.Trim();
            bank.Notes = bank.Notes?.Trim();
            bank.IsActive = true;
            bank.CreatedAt = DateTime.Now;
            bank.GlAccountId = Guid.NewGuid();

            Data.Accounts.Add(new Account
            {
                Id = bank.GlAccountId,
                Code = AllocateBankAccountCode(),
                Name = name,
                Type = AccountType.Asset,
                IsSystem = false,
                IsActive = true,
                CreatedAt = DateTime.Now
            });

            Data.BankAccounts.Add(bank);
            Save();
            return null;
        }

        public string UpdateBankAccount(BankAccount bank)
        {
            if (bank == null)
                return "Invalid bank account.";

            var existing = GetBankAccount(bank.Id);
            if (existing == null)
                return "Bank account not found.";

            var name = bank.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                return "Bank account name is required.";

            if (Data.BankAccounts.Any(b => b.IsActive
                && b.Id != bank.Id
                && string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase)))
                return "A bank account with this name already exists.";

            existing.Name = name;
            existing.AccountNumber = bank.AccountNumber?.Trim();
            existing.Branch = bank.Branch?.Trim();
            existing.Notes = bank.Notes?.Trim();

            var gl = Data.Accounts.FirstOrDefault(a => a.Id == existing.GlAccountId);
            if (gl != null)
                gl.Name = name;

            Save();
            return null;
        }

        public string DeleteBankAccount(Guid id)
        {
            var bank = GetBankAccount(id);
            if (bank == null)
                return "Bank account not found.";

            var hasJournal = Data.JournalEntries
                .SelectMany(e => e.Lines ?? Enumerable.Empty<JournalLine>())
                .Any(l => l.AccountId == bank.GlAccountId);
            if (hasJournal)
                return "Cannot delete a bank account that has ledger transactions.";

            var hasSupplierPayments = Data.SupplierPayments
                .Any(p => p.IsActive && p.CashAccountId == bank.GlAccountId);
            if (hasSupplierPayments)
                return "Cannot delete a bank account that has supplier payments.";

            var hasCustomerPayments = Data.CustomerPayments
                .Any(p => p.IsActive && p.CashAccountId == bank.GlAccountId);
            if (hasCustomerPayments)
                return "Cannot delete a bank account that has customer payments.";

            Data.BankAccounts.RemoveAll(b => b.Id == id);
            Data.Accounts.RemoveAll(a => a.Id == bank.GlAccountId);
            Save();
            return null;
        }

        private string AllocateBankAccountCode()
        {
            var used = new HashSet<string>(
                Data.Accounts.Where(a => !string.IsNullOrWhiteSpace(a.Code)).Select(a => a.Code),
                StringComparer.OrdinalIgnoreCase);

            for (var i = 1011; i < 1999; i++)
            {
                var code = i.ToString();
                if (!used.Contains(code))
                    return code;
            }

            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
        }

        public decimal GetAccountBalance(Guid accountId) =>
            _accounting.GetAccountBalance(Data, accountId);

        public decimal GetCashOrBankDisplayBalance(Guid accountId) =>
            _accounting.GetBankDisplayBalance(Data, accountId);

        public SalesProfitSummary GetSalesProfitSummary(DateTime? from = null, DateTime? to = null, string term = null) =>
            _accounting.GetSalesProfitSummary(Data, from, to, term);

        public IEnumerable<SalesProfitLine> GetSalesProfitLines(DateTime? from = null, DateTime? to = null, string term = null) =>
            _accounting.GetSalesProfitLines(Data, from, to, term);

        public string RecordExpense(BusinessExpense expense)
        {
            var error = _accounting.RecordExpense(Data, expense);
            if (error != null)
                return error;

            Save();
            return null;
        }

        public IEnumerable<Account> GetExpenseAccounts() =>
            _accounting.GetExpenseAccounts(Data);

        public IEnumerable<BusinessExpense> GetBusinessExpenses(
            DateTime? from = null,
            DateTime? to = null,
            string term = null,
            Guid? expenseAccountId = null) =>
            _accounting.GetBusinessExpenses(Data, from, to, term, expenseAccountId);

        public decimal GetTotalExpenses(DateTime? from = null, DateTime? to = null) =>
            _accounting.GetTotalExpenses(Data, from, to);

        public Invoice GetInvoice(Guid id) =>
            Data.Invoices.FirstOrDefault(i => i.Id == id);

        public IEnumerable<Invoice> SearchInvoices(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.Invoices.OrderByDescending(i => i.CreatedAt);

            term = term.Trim();
            return Data.Invoices
                .Where(i =>
                    (i.InvoiceNumber != null && i.InvoiceNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (i.CustomerName != null && i.CustomerName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (i.CustomerPhone != null && i.CustomerPhone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (i.Notes != null && i.Notes.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    i.Items.Any(item =>
                        (item.ProductSku != null && item.ProductSku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (item.ProductName != null && item.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)))
                .OrderByDescending(i => i.CreatedAt);
        }

        public string AdjustStock(
            Guid productId,
            TransactionType type,
            int quantity,
            string notes,
            Guid? customerId = null,
            Guid? supplierId = null)
        {
            var error = ApplyStockChange(
                productId,
                type,
                quantity,
                notes,
                customerId: customerId,
                supplierId: supplierId);
            if (error != null)
                return error;

            Save();
            return null;
        }

        public string StockInMultiple(IEnumerable<(Guid ProductId, int Quantity)> lines, string notes, Guid? supplierId = null)
        {
            if (lines == null)
                return "No products selected.";

            var lineList = lines.Where(l => l.Quantity > 0).ToList();
            if (lineList.Count == 0)
                return "Add at least one product to stock in.";

            foreach (var line in lineList)
            {
                var error = ApplyStockChange(
                    line.ProductId,
                    TransactionType.StockIn,
                    line.Quantity,
                    notes,
                    supplierId: supplierId);
                if (error != null)
                    return error;
            }

            Save();
            return null;
        }

        private string ApplyStockChange(
            Guid productId,
            TransactionType type,
            int quantity,
            string notes,
            string invoiceNumber = null,
            bool isSale = false,
            Guid? customerId = null,
            string customerName = null,
            Guid? supplierId = null,
            string supplierName = null)
        {
            if (quantity <= 0)
                return "Quantity must be greater than zero.";

            var product = GetProduct(productId);
            if (product == null)
                return "Product not found.";

            if (type == TransactionType.StockOut && product.Quantity < quantity)
                return $"Insufficient stock. Available: {product.Quantity}";

            Customer customer = null;
            if (customerId.HasValue)
            {
                customer = GetCustomer(customerId.Value);
                if (customer == null)
                    return "Customer not found.";
            }

            Supplier supplier = null;
            if (supplierId.HasValue)
            {
                supplier = GetSupplier(supplierId.Value);
                if (supplier == null)
                    return "Supplier not found.";
            }

            if (type == TransactionType.StockIn)
                product.Quantity += quantity;
            else
                product.Quantity -= quantity;

            product.LastUpdated = DateTime.Now;

            Data.Transactions.Insert(0, new StockTransaction
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSku = product.Sku,
                Type = type,
                Quantity = quantity,
                UnitPrice = product.UnitPrice,
                TotalValue = product.UnitPrice * quantity,
                Notes = notes ?? string.Empty,
                InvoiceNumber = invoiceNumber,
                IsSale = isSale,
                CustomerId = customer?.Id ?? customerId,
                CustomerName = customer?.Name ?? customerName,
                SupplierId = supplier?.Id ?? supplierId,
                SupplierName = supplier?.Name ?? supplierName,
                Timestamp = DateTime.Now
            });

            return null;
        }

        public Customer GetCustomer(Guid id) =>
            Data.Customers.FirstOrDefault(c => c.Id == id);

        public bool PhoneExists(string phone, Guid? excludeId = null)
        {
            var normalized = NormalizePhone(phone);
            if (string.IsNullOrEmpty(normalized)) return false;

            return Data.Customers.Any(c =>
                NormalizePhone(c.Phone) == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        private static string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
            return new string(phone.Where(char.IsDigit).ToArray());
        }

        public void AddCustomer(Customer customer)
        {
            customer.CreatedAt = DateTime.Now;
            Data.Customers.Add(customer);
            Save();
        }

        public void UpdateCustomer(Customer customer)
        {
            var existing = GetCustomer(customer.Id);
            if (existing == null) return;

            existing.Name = customer.Name;
            existing.Address = customer.Address;
            existing.Phone = customer.Phone;
            existing.Email = customer.Email;
            Save();
        }

        public void DeleteCustomer(Guid id)
        {
            Data.Customers.RemoveAll(c => c.Id == id);
            foreach (var t in Data.Transactions.Where(t => t.CustomerId == id))
            {
                t.CustomerId = null;
                t.CustomerName = null;
            }
            Save();
        }

        public IEnumerable<Customer> SearchCustomers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.Customers.OrderBy(c => c.Name);

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var phonePart))
            {
                return Data.Customers
                    .Where(c => MatchesTextPart(c.Name, namePart) && MatchesPhonePart(c.Phone, phonePart))
                    .OrderBy(c => c.Name);
            }

            return Data.Customers
                .Where(c =>
                    MatchesTextPart(c.Name, term) ||
                    MatchesTextPart(c.Phone, term) ||
                    MatchesTextPart(c.Email, term) ||
                    MatchesTextPart(c.Address, term))
                .OrderBy(c => c.Name);
        }

        public bool CustomerMatchesSearchTerm(Customer customer, string term)
        {
            if (customer == null)
                return false;

            if (string.IsNullOrWhiteSpace(term))
                return true;

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var phonePart))
                return MatchesTextPart(customer.Name, namePart) && MatchesPhonePart(customer.Phone, phonePart);

            return MatchesTextPart(customer.Name, term) ||
                   MatchesTextPart(customer.Phone, term) ||
                   MatchesTextPart(customer.Email, term) ||
                   MatchesTextPart(customer.Address, term);
        }

        public Supplier GetSupplier(Guid id) =>
            Data.Suppliers.FirstOrDefault(s => s.Id == id);

        public bool SupplierPhoneExists(string phone, Guid? excludeId = null)
        {
            var normalized = NormalizePhone(phone);
            if (string.IsNullOrEmpty(normalized)) return false;

            return Data.Suppliers.Any(s =>
                NormalizePhone(s.Phone) == normalized &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }

        public void AddSupplier(Supplier supplier)
        {
            supplier.CreatedAt = DateTime.Now;
            Data.Suppliers.Add(supplier);
            Save();
        }

        public void UpdateSupplier(Supplier supplier)
        {
            var existing = GetSupplier(supplier.Id);
            if (existing == null) return;

            existing.Name = supplier.Name;
            existing.Address = supplier.Address;
            existing.Phone = supplier.Phone;
            existing.Email = supplier.Email;
            Save();
        }

        public void DeleteSupplier(Guid id)
        {
            Data.Suppliers.RemoveAll(s => s.Id == id);
            Data.SupplierPayments.RemoveAll(p => p.SupplierId == id);
            foreach (var t in Data.Transactions.Where(t => t.SupplierId == id))
            {
                t.SupplierId = null;
                t.SupplierName = null;
            }
            Save();
        }

        public IEnumerable<Supplier> SearchSuppliers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.Suppliers.OrderBy(s => s.Name);

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var phonePart))
            {
                return Data.Suppliers
                    .Where(s => MatchesTextPart(s.Name, namePart) && MatchesPhonePart(s.Phone, phonePart))
                    .OrderBy(s => s.Name);
            }

            return Data.Suppliers
                .Where(s =>
                    MatchesTextPart(s.Name, term) ||
                    MatchesTextPart(s.Phone, term) ||
                    MatchesTextPart(s.Email, term) ||
                    MatchesTextPart(s.Address, term))
                .OrderBy(s => s.Name);
        }

        public bool SupplierMatchesSearchTerm(Supplier supplier, string term)
        {
            if (supplier == null)
                return false;

            if (string.IsNullOrWhiteSpace(term))
                return true;

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var phonePart))
                return MatchesTextPart(supplier.Name, namePart) && MatchesPhonePart(supplier.Phone, phonePart);

            return MatchesTextPart(supplier.Name, term) ||
                   MatchesTextPart(supplier.Phone, term) ||
                   MatchesTextPart(supplier.Email, term) ||
                   MatchesTextPart(supplier.Address, term);
        }

        public decimal GetSupplierBalance(Guid supplierId)
        {
            var purchased = Data.Transactions
                .Where(t => t.Type == TransactionType.StockIn && t.SupplierId == supplierId)
                .Sum(t => t.TotalValue);
            var paid = Data.SupplierPayments
                .Where(p => p.IsActive && p.SupplierId == supplierId)
                .Sum(p => p.Amount);
            return Math.Max(0, purchased - paid);
        }

        public IEnumerable<SupplierDueRow> GetSupplierDueReport(string term = null)
        {
            var rows = Data.Suppliers.Select(supplier =>
            {
                var purchases = Data.Transactions
                    .Where(t => t.Type == TransactionType.StockIn && t.SupplierId == supplier.Id)
                    .ToList();
                var totalPurchased = purchases.Sum(t => t.TotalValue);
                var totalPaid = Data.SupplierPayments
                    .Where(p => p.IsActive && p.SupplierId == supplier.Id)
                    .Sum(p => p.Amount);
                return new SupplierDueRow
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.Name,
                    Phone = supplier.Phone ?? string.Empty,
                    TotalPurchased = totalPurchased,
                    TotalPaid = totalPaid,
                    BalanceDue = Math.Max(0, totalPurchased - totalPaid),
                    PurchaseCount = purchases.Count
                };
            }).Where(r => r.TotalPurchased > 0 || r.TotalPaid > 0 || r.BalanceDue > 0);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                rows = rows.Where(r =>
                    (r.SupplierName != null && r.SupplierName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r.Phone != null && r.Phone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return rows.OrderByDescending(r => r.BalanceDue).ThenBy(r => r.SupplierName);
        }

        public IEnumerable<SupplierLedgerRow> GetSupplierLedger(Guid supplierId)
        {
            var entries = new List<(DateTime SortDate, int SortOrder, SupplierLedgerRow Row)>();

            foreach (var txn in Data.Transactions.Where(t =>
                t.Type == TransactionType.StockIn && t.SupplierId == supplierId))
            {
                entries.Add((txn.Timestamp, 0, new SupplierLedgerRow
                {
                    Date = txn.Timestamp,
                    EntryType = "Debit",
                    Description = $"Stock in: {txn.ProductName} x{txn.Quantity}",
                    Reference = txn.ProductSku ?? string.Empty,
                    Debit = txn.TotalValue,
                    Credit = 0
                }));
            }

            foreach (var payment in Data.SupplierPayments.Where(p => p.SupplierId == supplierId && p.IsActive))
            {
                var method = string.IsNullOrWhiteSpace(payment.PaymentMethod) ? "Payment" : payment.PaymentMethod;
                entries.Add((payment.PaidAt, 1, new SupplierLedgerRow
                {
                    Date = payment.PaidAt,
                    EntryType = "Credit",
                    Description = $"{method} paid to supplier",
                    Reference = payment.Reference ?? string.Empty,
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

        public string RecordSupplierPayment(SupplierPayment payment)
        {
            if (payment == null)
                return "Invalid payment.";

            if (payment.Amount <= 0)
                return "Payment amount must be greater than zero.";

            var supplier = GetSupplier(payment.SupplierId);
            if (supplier == null)
                return "Supplier not found.";

            if (!_accounting.IsCashOrBankAccount(Data, payment.CashAccountId))
                return "Select a cash or bank account.";

            var cashAccount = Data.Accounts.FirstOrDefault(a => a.Id == payment.CashAccountId);
            if (cashAccount == null)
                return "Select a cash or bank account.";

            var balance = GetSupplierBalance(payment.SupplierId);
            if (payment.Amount > balance)
                return $"Payment exceeds supplier due ({balance:C2}).";

            payment.Id = payment.Id == Guid.Empty ? Guid.NewGuid() : payment.Id;
            payment.SupplierName = supplier.Name;
            payment.CashAccountName = cashAccount.Name;
            payment.IsVoided = false;
            payment.VoidedAt = null;
            if (payment.PaidAt == default)
                payment.PaidAt = DateTime.Now;

            Data.SupplierPayments.Insert(0, payment);
            Save();
            return null;
        }

        public string VoidCustomerPayment(Guid paymentId, string reason)
        {
            var payment = Data.CustomerPayments.FirstOrDefault(p => p.Id == paymentId);
            if (payment == null)
                return "Payment not found.";

            var error = _accounting.VoidPayment(Data, payment, reason);
            if (error != null)
                return error;

            ReallocateCustomerInvoicePayments(payment.CustomerId);
            Save();
            return null;
        }

        public string VoidSupplierPayment(Guid paymentId, string reason)
        {
            var payment = Data.SupplierPayments.FirstOrDefault(p => p.Id == paymentId);
            if (payment == null)
                return "Payment not found.";

            if (!payment.IsActive)
                return "Payment is already voided.";

            payment.IsVoided = true;
            payment.VoidedAt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                var note = reason.Trim();
                payment.Notes = string.IsNullOrWhiteSpace(payment.Notes)
                    ? $"Voided: {note}"
                    : $"{payment.Notes}\r\nVoided: {note}";
            }

            Save();
            return null;
        }

        private void ReallocateCustomerInvoicePayments(Guid customerId)
        {
            var invoices = Data.Invoices
                .Where(i => i.IsActive && i.CustomerId == customerId)
                .OrderBy(i => i.CreatedAt)
                .ToList();

            foreach (var invoice in invoices)
                invoice.AmountPaid = 0;

            foreach (var payment in Data.CustomerPayments
                .Where(p => p.IsActive && p.CustomerId == customerId && p.Amount > 0)
                .OrderBy(p => p.PaidAt))
            {
                var remaining = payment.Amount;

                if (payment.InvoiceId.HasValue)
                {
                    var target = invoices.FirstOrDefault(i => i.Id == payment.InvoiceId.Value);
                    if (target != null)
                    {
                        var applied = Math.Min(remaining, Math.Max(0, target.TotalAmount - target.AmountPaid));
                        target.AmountPaid += applied;
                        remaining -= applied;
                    }
                }

                if (remaining <= 0)
                    continue;

                foreach (var invoice in invoices.Where(i => i.BalanceDue > 0))
                {
                    if (remaining <= 0)
                        break;

                    var applied = Math.Min(remaining, invoice.BalanceDue);
                    invoice.AmountPaid += applied;
                    remaining -= applied;
                }
            }
        }

        private static bool TryParseDisplaySearch(string term, out string namePart, out string phonePart)
        {
            namePart = string.Empty;
            phonePart = string.Empty;

            var separators = new[] { " — ", " – ", " -- ", " - ", "—", "–", "--" };
            foreach (var separator in separators.OrderByDescending(s => s.Length))
            {
                var index = term.IndexOf(separator, StringComparison.Ordinal);
                if (index < 0)
                    continue;

                namePart = term.Substring(0, index).Trim();
                phonePart = term.Substring(index + separator.Length).Trim();
                return true;
            }

            var trailingSeparators = new[] { " —", " –", " --", " -", "—", "–", "--", "-" };
            foreach (var separator in trailingSeparators.OrderByDescending(s => s.Length))
            {
                if (!term.EndsWith(separator, StringComparison.Ordinal))
                    continue;

                namePart = term.Substring(0, term.Length - separator.Length).Trim();
                phonePart = string.Empty;
                return true;
            }

            return false;
        }

        private static bool MatchesTextPart(string value, string part)
        {
            if (string.IsNullOrEmpty(part))
                return true;

            return !string.IsNullOrEmpty(value) &&
                   value.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool MatchesPhonePart(string phone, string phonePart)
        {
            if (string.IsNullOrEmpty(phonePart))
                return true;

            if (string.IsNullOrEmpty(phone))
                return false;

            if (phone.IndexOf(phonePart, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            var partDigits = NormalizePhone(phonePart);
            if (partDigits.Length == 0)
                return true;

            var phoneDigits = NormalizePhone(phone);
            return phoneDigits.IndexOf(partDigits, StringComparison.Ordinal) >= 0;
        }

        public IEnumerable<Product> SearchProducts(string term, ProductType? typeFilter = null, string categoryFilter = null)
        {
            IEnumerable<Product> query = Data.Products;

            if (typeFilter.HasValue)
                query = query.Where(p => p.ProductType == typeFilter.Value);

            if (!string.IsNullOrWhiteSpace(categoryFilter))
            {
                categoryFilter = categoryFilter.Trim();
                query = query.Where(p =>
                    !string.IsNullOrWhiteSpace(p.Category) &&
                    string.Equals(p.Category.Trim(), categoryFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(term))
                return query.OrderBy(p => p.Name);

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var skuPart))
            {
                return query
                    .Where(p => MatchesTextPart(p.Name, namePart) && MatchesTextPart(p.Sku, skuPart))
                    .OrderBy(p => p.Name);
            }

            return query
                .Where(p =>
                    MatchesTextPart(p.Name, term) ||
                    MatchesTextPart(p.Sku, term) ||
                    MatchesTextPart(p.Category, term) ||
                    MatchesTextPart(ProductTypeLabels.ToLabel(p.ProductType), term))
                .OrderBy(p => p.Name);
        }

        public IEnumerable<string> GetProductCategories(ProductType? productType = null)
        {
            var products = Data.Products.AsEnumerable();
            if (productType.HasValue)
                products = products.Where(p => p.ProductType == productType.Value);

            return products
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase);
        }

        public bool ProductMatchesSearchTerm(Product product, string term)
        {
            if (product == null)
                return false;

            if (string.IsNullOrWhiteSpace(term))
                return true;

            term = term.Trim();

            if (TryParseDisplaySearch(term, out var namePart, out var skuPart))
                return MatchesTextPart(product.Name, namePart) && MatchesTextPart(product.Sku, skuPart);

            return MatchesTextPart(product.Name, term) ||
                   MatchesTextPart(product.Sku, term) ||
                   MatchesTextPart(product.Category, term);
        }

        public IEnumerable<StockTransaction> SearchTransactions(
            string term,
            bool filterIn,
            bool filterOut,
            DateTime? fromDate,
            DateTime? toDate)
        {
            IEnumerable<StockTransaction> query = Data.Transactions;

            if (filterIn != filterOut)
            {
                var type = filterIn ? TransactionType.StockIn : TransactionType.StockOut;
                query = query.Where(t => t.Type == type);
            }

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                query = query.Where(t => t.Timestamp >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.Timestamp <= to);
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(t =>
                    (t.InvoiceNumber != null && t.InvoiceNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (t.ProductSku != null && t.ProductSku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (t.ProductName != null && t.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (t.CustomerName != null && t.CustomerName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (t.Notes != null && t.Notes.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            return query.OrderByDescending(t => t.Timestamp);
        }

        private string GenerateInvoiceNumber()
        {
            string invoice;
            do
            {
                invoice = $"INV-{Data.NextInvoiceNumber:D6}";
                Data.NextInvoiceNumber++;
            }
            while (Data.Transactions.Any(t =>
                       string.Equals(t.InvoiceNumber, invoice, StringComparison.OrdinalIgnoreCase)) ||
                   Data.Invoices.Any(i =>
                       string.Equals(i.InvoiceNumber, invoice, StringComparison.OrdinalIgnoreCase)));

            return invoice;
        }

        private void EnsureInvoiceSequence()
        {
            var max = 0;

            foreach (var invoice in Data.Invoices)
            {
                if (TryParseInvoiceSequence(invoice.InvoiceNumber, out var sequence) && sequence > max)
                    max = sequence;
            }

            foreach (var transaction in Data.Transactions.Where(t => t.IsSale))
            {
                if (TryParseInvoiceSequence(transaction.InvoiceNumber, out var sequence) && sequence > max)
                    max = sequence;
            }

            if (Data.NextInvoiceNumber <= max)
                Data.NextInvoiceNumber = max + 1;
        }

        private static bool TryParseInvoiceSequence(string invoiceNumber, out int sequence)
        {
            sequence = 0;
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                return false;

            var parts = invoiceNumber.Trim().Split('-');
            if (parts.Length == 0)
                return false;

            return int.TryParse(parts[parts.Length - 1], out sequence);
        }

        public ProductionRecipe GetRecipe(Guid id) =>
            Data.ProductionRecipes.FirstOrDefault(r => r.Id == id);

        public void AddRecipe(ProductionRecipe recipe)
        {
            recipe.CreatedAt = DateTime.Now;
            Data.ProductionRecipes.Add(recipe);
            Save();
        }

        public void UpdateRecipe(ProductionRecipe recipe)
        {
            var existing = GetRecipe(recipe.Id);
            if (existing == null) return;

            existing.Name = recipe.Name;
            existing.OutputProductId = recipe.OutputProductId;
            existing.OutputProductSku = recipe.OutputProductSku;
            existing.OutputProductName = recipe.OutputProductName;
            existing.Materials = recipe.Materials ?? new List<ProductionMaterial>();
            Save();
        }

        public void DeleteRecipe(Guid id)
        {
            Data.ProductionRecipes.RemoveAll(r => r.Id == id);
            Save();
        }

        public IEnumerable<ProductionRecipe> SearchRecipes(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.ProductionRecipes.OrderBy(r => r.Name);

            term = term.Trim();
            return Data.ProductionRecipes
                .Where(r =>
                    (r.Name != null && r.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r.OutputProductSku != null && r.OutputProductSku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (r.OutputProductName != null && r.OutputProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderBy(r => r.Name);
        }

        public IEnumerable<ProductionOrder> SearchProductionOrders(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.ProductionOrders.OrderByDescending(o => o.Timestamp);

            term = term.Trim();
            return Data.ProductionOrders
                .Where(o =>
                    (o.ProductionNumber != null && o.ProductionNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (o.RecipeName != null && o.RecipeName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (o.OutputProductSku != null && o.OutputProductSku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (o.OutputProductName != null && o.OutputProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderByDescending(o => o.Timestamp);
        }

        public string RunProduction(ProductionRecipe recipe, int batchQuantity, string notes)
        {
            if (recipe == null)
                return "Production not found.";

            if (batchQuantity <= 0)
                return "Quantity must be greater than zero.";

            if (recipe.Materials == null || recipe.Materials.Count == 0)
                return "Production has no materials.";

            var output = GetProduct(recipe.OutputProductId);
            if (output == null)
                return "Output product not found.";

            var oldQuantity = output.Quantity;
            var oldUnitCost = output.UnitCost;
            decimal batchMaterialCost = 0;

            foreach (var material in recipe.Materials)
            {
                if (material.QuantityPerUnit <= 0)
                    return $"Invalid quantity for material '{material.ProductName}'.";

                var product = GetProduct(material.ProductId);
                if (product == null)
                    return $"Material '{material.ProductName}' not found.";

                var required = material.QuantityPerUnit * batchQuantity;
                if (product.Quantity < required)
                    return $"Insufficient '{product.Name}'. Required: {required}, Available: {product.Quantity}";

                batchMaterialCost += product.UnitPrice * required;
            }

            var batchUnitCost = batchQuantity > 0 ? batchMaterialCost / batchQuantity : 0;

            var productionNumber = GenerateProductionNumber();
            var notePrefix = $"Production {productionNumber}";

            foreach (var material in recipe.Materials)
            {
                var required = material.QuantityPerUnit * batchQuantity;
                var error = ApplyStockChange(material.ProductId, TransactionType.StockOut, required, notePrefix);
                if (error != null)
                    return error;
            }

            var stockInError = ApplyStockChange(output.Id, TransactionType.StockIn, batchQuantity, notePrefix);
            if (stockInError != null)
                return stockInError;

            output = GetProduct(output.Id);
            var unitPrice = output?.UnitPrice ?? 0;
            if (output != null)
            {
                var newQuantity = output.Quantity;
                output.UnitCost = newQuantity > 0
                    ? ((oldQuantity * oldUnitCost) + (batchQuantity * batchUnitCost)) / newQuantity
                    : batchUnitCost;
            }

            Data.ProductionOrders.Insert(0, new ProductionOrder
            {
                ProductionNumber = productionNumber,
                RecipeId = recipe.Id,
                RecipeName = recipe.Name,
                OutputProductId = recipe.OutputProductId,
                OutputProductSku = recipe.OutputProductSku,
                OutputProductName = recipe.OutputProductName,
                QuantityProduced = batchQuantity,
                OutputUnitPrice = unitPrice,
                TotalOutputValue = unitPrice * batchQuantity,
                PriorOutputQuantity = oldQuantity,
                PriorOutputUnitCost = oldUnitCost,
                BatchUnitCost = batchUnitCost,
                MaterialsUsed = recipe.Materials.Select(m => new ProductionMaterial
                {
                    ProductId = m.ProductId,
                    ProductSku = m.ProductSku,
                    ProductName = m.ProductName,
                    QuantityPerUnit = m.QuantityPerUnit * batchQuantity
                }).ToList(),
                Notes = notes ?? string.Empty,
                Timestamp = DateTime.Now
            });

            Save();
            return null;
        }

        public ProductionOrder GetProductionOrder(Guid id) =>
            Data.ProductionOrders.FirstOrDefault(o => o.Id == id);

        public string VoidInvoice(Guid invoiceId, string reason)
        {
            var invoice = GetInvoice(invoiceId);
            if (invoice == null)
                return "Invoice not found.";

            if (!invoice.IsActive)
                return "Invoice is already voided. Restore it first before voiding again.";

            var payments = Data.CustomerPayments
                .Where(p => p.InvoiceId == invoice.Id && p.IsActive)
                .ToList();

            var accountingError = _accounting.ValidateVoidSale(Data, invoice);
            if (accountingError != null)
                return accountingError;

            foreach (var payment in payments)
            {
                var paymentValidationError = _accounting.ValidateVoidPayment(Data, payment);
                if (paymentValidationError != null)
                    return paymentValidationError;
            }

            foreach (var payment in payments)
            {
                var paymentError = _accounting.VoidPayment(Data, payment, reason);
                if (paymentError != null)
                    return paymentError;

                invoice.AmountPaid = Math.Max(0, invoice.AmountPaid - payment.Amount);
            }

            foreach (var line in invoice.Items ?? Enumerable.Empty<InvoiceLineItem>())
            {
                var note = $"Void sale {invoice.InvoiceNumber}";
                var stockError = ApplyStockChange(
                    line.ProductId,
                    TransactionType.StockIn,
                    line.Quantity,
                    note,
                    invoiceNumber: invoice.InvoiceNumber);
                if (stockError != null)
                    return stockError;
            }

            accountingError = _accounting.VoidSale(Data, invoice, reason);
            if (accountingError != null)
                return accountingError;

            invoice.Status = OperationalStatus.Voided;
            invoice.VoidedAt = DateTime.Now;
            invoice.VoidReason = reason?.Trim() ?? string.Empty;
            invoice.AmountPaid = 0;

            Save();
            return null;
        }

        public string ReverseProduction(Guid productionOrderId, string reason)
        {
            var order = GetProductionOrder(productionOrderId);
            if (order == null)
                return "Production run not found.";

            if (!order.IsActive)
                return "Production run is already reversed.";

            var output = GetProduct(order.OutputProductId);
            if (output == null)
                return "Output product not found.";

            if (output.Quantity < order.QuantityProduced)
            {
                return $"Cannot reverse production: only {output.Quantity} of {order.QuantityProduced} produced units remain in stock. " +
                       "Void the related sale first, then reverse production.";
            }

            var reversalNote = $"Reversal of {order.ProductionNumber}";
            var outputError = ApplyStockChange(
                output.Id,
                TransactionType.StockOut,
                order.QuantityProduced,
                reversalNote);
            if (outputError != null)
                return outputError;

            output.UnitCost = order.PriorOutputUnitCost;
            output.LastUpdated = DateTime.Now;

            foreach (var material in order.MaterialsUsed ?? Enumerable.Empty<ProductionMaterial>())
            {
                if (material.QuantityPerUnit <= 0)
                    continue;

                var materialError = ApplyStockChange(
                    material.ProductId,
                    TransactionType.StockIn,
                    material.QuantityPerUnit,
                    reversalNote);
                if (materialError != null)
                    return materialError;
            }

            order.Status = OperationalStatus.Voided;
            order.VoidedAt = DateTime.Now;
            order.VoidReason = reason?.Trim() ?? string.Empty;

            Save();
            return null;
        }

        public string RestoreInvoice(Guid invoiceId, string reason)
        {
            var invoice = GetInvoice(invoiceId);
            if (invoice == null)
                return "Invoice not found.";

            if (invoice.IsActive)
                return "Only voided invoices can be restored.";

            var accountingError = _accounting.ValidateReinstateSale(Data, invoice);
            if (accountingError != null)
                return accountingError;

            foreach (var line in invoice.Items ?? Enumerable.Empty<InvoiceLineItem>())
            {
                var product = GetProduct(line.ProductId);
                if (product == null)
                    return $"Product not found for line '{line.ProductName}'.";

                if (product.Quantity < line.Quantity)
                {
                    return $"Cannot restore sale: insufficient stock for {product.Name}. " +
                           $"Required: {line.Quantity}, Available: {product.Quantity}.";
                }
            }

            var voidedPayments = Data.CustomerPayments
                .Where(p => p.InvoiceId == invoice.Id && !p.IsActive)
                .ToList();

            foreach (var payment in voidedPayments)
            {
                var paymentValidationError = _accounting.ValidateReinstatePayment(Data, payment);
                if (paymentValidationError != null)
                    return paymentValidationError;
            }

            foreach (var line in invoice.Items ?? Enumerable.Empty<InvoiceLineItem>())
            {
                var note = $"Restore sale {invoice.InvoiceNumber}";
                var stockError = ApplyStockChange(
                    line.ProductId,
                    TransactionType.StockOut,
                    line.Quantity,
                    note,
                    invoiceNumber: invoice.InvoiceNumber,
                    isSale: true,
                    customerId: invoice.CustomerId,
                    customerName: invoice.CustomerName);
                if (stockError != null)
                    return stockError;
            }

            foreach (var payment in voidedPayments)
            {
                var paymentError = _accounting.ReinstatePayment(Data, payment, invoice, reason);
                if (paymentError != null)
                    return paymentError;
            }

            accountingError = _accounting.ReinstateSale(Data, invoice, reason);
            if (accountingError != null)
                return accountingError;

            invoice.Status = OperationalStatus.Active;
            invoice.VoidedAt = null;
            invoice.VoidReason = null;

            Save();
            return null;
        }

        public string RestoreProduction(Guid productionOrderId, string reason)
        {
            var order = GetProductionOrder(productionOrderId);
            if (order == null)
                return "Production run not found.";

            if (order.IsActive)
                return "Only reversed production runs can be restored.";

            var output = GetProduct(order.OutputProductId);
            if (output == null)
                return "Output product not found.";

            foreach (var material in order.MaterialsUsed ?? Enumerable.Empty<ProductionMaterial>())
            {
                if (material.QuantityPerUnit <= 0)
                    continue;

                var product = GetProduct(material.ProductId);
                if (product == null)
                    return $"Material '{material.ProductName}' not found.";

                if (product.Quantity < material.QuantityPerUnit)
                {
                    return $"Cannot restore production: insufficient '{product.Name}'. " +
                           $"Required: {material.QuantityPerUnit}, Available: {product.Quantity}.";
                }
            }

            var restoreNote = $"Restore {order.ProductionNumber}";
            var oldQuantity = output.Quantity;
            var oldUnitCost = output.UnitCost;
            var batchQuantity = order.QuantityProduced;
            var batchUnitCost = order.BatchUnitCost;

            foreach (var material in order.MaterialsUsed ?? Enumerable.Empty<ProductionMaterial>())
            {
                if (material.QuantityPerUnit <= 0)
                    continue;

                var materialError = ApplyStockChange(
                    material.ProductId,
                    TransactionType.StockOut,
                    material.QuantityPerUnit,
                    restoreNote);
                if (materialError != null)
                    return materialError;
            }

            var outputError = ApplyStockChange(output.Id, TransactionType.StockIn, batchQuantity, restoreNote);
            if (outputError != null)
                return outputError;

            output = GetProduct(output.Id);
            if (output != null)
            {
                var newQuantity = output.Quantity;
                output.UnitCost = newQuantity > 0 && batchUnitCost > 0
                    ? ((oldQuantity * oldUnitCost) + (batchQuantity * batchUnitCost)) / newQuantity
                    : batchUnitCost;
                output.LastUpdated = DateTime.Now;
            }

            order.Status = OperationalStatus.Active;
            order.VoidedAt = null;
            order.VoidReason = null;

            Save();
            return null;
        }

        private string GenerateProductionNumber()
        {
            string productionNumber;
            do
            {
                productionNumber = $"PRO-{Data.NextProductionNumber:D6}";
                Data.NextProductionNumber++;
            }
            while (Data.ProductionOrders.Any(o =>
                string.Equals(o.ProductionNumber, productionNumber, StringComparison.OrdinalIgnoreCase)));

            return productionNumber;
        }

        private void EnsureProductionSequence()
        {
            var max = 0;
            foreach (var order in Data.ProductionOrders)
            {
                if (TryParseProductionSequence(order.ProductionNumber, out var sequence) && sequence > max)
                    max = sequence;
            }

            if (Data.NextProductionNumber <= max)
                Data.NextProductionNumber = max + 1;
        }

        private static bool TryParseProductionSequence(string productionNumber, out int sequence)
        {
            sequence = 0;
            if (string.IsNullOrWhiteSpace(productionNumber))
                return false;

            var parts = productionNumber.Trim().Split('-');
            if (parts.Length == 0)
                return false;

            return int.TryParse(parts[parts.Length - 1], out sequence);
        }

        public int ProductCount => Data.Products.Count;
        public int LowStockCount => Data.Products.Count(p => p.IsLowStock);
        public decimal TotalInventoryValue => Data.Products.Sum(p => p.StockValue);
    }
}
