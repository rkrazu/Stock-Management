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
                if (Data.Transactions == null) Data.Transactions = new List<StockTransaction>();
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

        public string AdjustStock(Guid productId, TransactionType type, int quantity, string notes)
        {
            if (quantity <= 0)
                return "Quantity must be greater than zero.";

            var product = GetProduct(productId);
            if (product == null)
                return "Product not found.";

            if (type == TransactionType.StockOut && product.Quantity < quantity)
                return $"Insufficient stock. Available: {product.Quantity}";

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
                Notes = notes ?? string.Empty,
                Timestamp = DateTime.Now
            });

            Save();
            return null;
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

        public int LowStockCount => Data.Products.Count(p => p.IsLowStock);
        public decimal TotalInventoryValue => Data.Products.Sum(p => p.StockValue);
    }
}
