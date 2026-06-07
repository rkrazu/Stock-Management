using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Services
{
    public class StockRepository
    {
        private static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StockManagement",
            "stockdata.json");

        public StockData Data { get; private set; } = new StockData();

        public void Load()
        {
            try
            {
                if (!File.Exists(DataPath))
                {
                    Data = new StockData();
                    return;
                }

                using (var stream = File.OpenRead(DataPath))
                {
                    var serializer = new DataContractJsonSerializer(typeof(StockData));
                    Data = (StockData)serializer.ReadObject(stream) ?? new StockData();
                }

                if (Data.Products == null) Data.Products = new List<Product>();
                if (Data.Customers == null) Data.Customers = new List<Customer>();
                if (Data.Transactions == null) Data.Transactions = new List<StockTransaction>();
                if (Data.NextInvoiceNumber < 1) Data.NextInvoiceNumber = 1;

                MigrateTransactions();
            }
            catch
            {
                Data = new StockData();
                MessageBox.Show(
                    "Could not load saved data. Starting with an empty inventory.",
                    "Stock Management",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        public void Save()
        {
            var dir = Path.GetDirectoryName(DataPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var stream = File.Create(DataPath))
            {
                var serializer = new DataContractJsonSerializer(typeof(StockData));
                serializer.WriteObject(stream, Data);
            }
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
            existing.UnitPrice = product.UnitPrice;
            existing.ReorderLevel = product.ReorderLevel;
            existing.LastUpdated = DateTime.Now;
            Save();
        }

        public void DeleteProduct(Guid id)
        {
            Data.Products.RemoveAll(p => p.Id == id);
            Data.Transactions.RemoveAll(t => t.ProductId == id);
            Save();
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
                InvoiceNumber = GenerateInvoiceNumber(),
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
            return Data.Customers
                .Where(c =>
                    (c.Name != null && c.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.Phone != null && c.Phone.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.Email != null && c.Email.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (c.Address != null && c.Address.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderBy(c => c.Name);
        }

        public IEnumerable<Product> SearchProducts(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Data.Products.OrderBy(p => p.Name);

            term = term.Trim();
            return Data.Products
                .Where(p =>
                    (p.Name != null && p.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Sku != null && p.Sku.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Category != null && p.Category.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderBy(p => p.Name);
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

        private void MigrateTransactions()
        {
            EnsureInvoiceSequence();

            var changed = false;
            foreach (var transaction in Data.Transactions.OrderBy(t => t.Timestamp))
            {
                if (string.IsNullOrWhiteSpace(transaction.InvoiceNumber))
                {
                    transaction.InvoiceNumber = GenerateInvoiceNumber();
                    changed = true;
                }

                if (transaction.TotalValue == 0 && transaction.UnitPrice == 0)
                {
                    var product = GetProduct(transaction.ProductId);
                    if (product != null)
                    {
                        transaction.UnitPrice = product.UnitPrice;
                        transaction.TotalValue = transaction.Quantity * product.UnitPrice;
                        changed = true;
                    }
                }
            }

            if (changed)
                Save();
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
                string.Equals(t.InvoiceNumber, invoice, StringComparison.OrdinalIgnoreCase)));

            return invoice;
        }

        private void EnsureInvoiceSequence()
        {
            var max = 0;
            foreach (var transaction in Data.Transactions)
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

        public int LowStockCount => Data.Products.Count(p => p.IsLowStock);
        public decimal TotalInventoryValue => Data.Products.Sum(p => p.StockValue);
    }
}
