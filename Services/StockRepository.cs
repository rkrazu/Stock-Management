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
                if (Data.Transactions == null) Data.Transactions = new List<StockTransaction>();
                if (Data.ProductionRecipes == null) Data.ProductionRecipes = new List<ProductionRecipe>();
                if (Data.ProductionOrders == null) Data.ProductionOrders = new List<ProductionOrder>();
                if (Data.Invoices == null) Data.Invoices = new List<Invoice>();
                if (Data.Accounts == null) Data.Accounts = new List<Account>();
                if (Data.JournalEntries == null) Data.JournalEntries = new List<JournalEntry>();
                if (Data.CustomerPayments == null) Data.CustomerPayments = new List<CustomerPayment>();
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
            Data.Products.RemoveAll(p => p.Id == id);
            Data.Transactions.RemoveAll(t => t.ProductId == id);
            foreach (var invoice in Data.Invoices)
                invoice.Items?.RemoveAll(i => i.ProductId == id);
            Data.Invoices.RemoveAll(i => i.Items == null || i.Items.Count == 0);
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
                if (cashAccount == null || (cashAccount.Id != SystemAccounts.CashId && cashAccount.Id != SystemAccounts.BankId))
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

        public decimal GetCustomerBalance(Guid customerId) =>
            _accounting.GetCustomerBalance(Data, customerId);

        public decimal GetTotalOutstanding() =>
            _accounting.GetTotalOutstanding(Data);

        public IEnumerable<CustomerDueRow> GetCustomerDueReport(string term = null) =>
            _accounting.GetCustomerDueReport(Data, term);

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

        public decimal GetAccountBalance(Guid accountId) =>
            _accounting.GetAccountBalance(Data, accountId);

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

        public string AdjustStock(Guid productId, TransactionType type, int quantity, string notes, Guid? customerId = null)
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
                CustomerId = customer?.Id,
                CustomerName = customer?.Name,
                IsSale = false,
                Timestamp = DateTime.Now
            });

            Save();
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

        public IEnumerable<string> GetProductCategories()
        {
            return Data.Products
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
                return "Recipe not found.";

            if (batchQuantity <= 0)
                return "Quantity must be greater than zero.";

            if (recipe.Materials == null || recipe.Materials.Count == 0)
                return "Recipe has no materials.";

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
                var error = AdjustStock(material.ProductId, TransactionType.StockOut, required, notePrefix);
                if (error != null)
                    return error;
            }

            var stockInError = AdjustStock(output.Id, TransactionType.StockIn, batchQuantity, notePrefix);
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
