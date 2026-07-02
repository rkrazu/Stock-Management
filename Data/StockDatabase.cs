using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Data
{
    public class StockDatabase
    {
        public StockData LoadAll()
        {
            var data = new StockData();

            using (var connection = OpenConnection())
            {
                LoadSettings(connection, data);
                data.Products = LoadProducts(connection);
                data.Customers = LoadCustomers(connection);
                data.Transactions = LoadTransactions(connection);
                data.Invoices = LoadInvoices(connection);
                data.ProductionRecipes = LoadRecipes(connection);
                data.ProductionOrders = LoadProductionOrders(connection);
            }

            return data;
        }

        public void SaveAll(StockData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var connection = OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        ClearTables(connection, transaction);
                        InsertSettings(connection, transaction, data);
                        InsertProducts(connection, transaction, data.Products);
                        InsertCustomers(connection, transaction, data.Customers);
                        InsertTransactions(connection, transaction, data.Transactions);
                        InsertInvoices(connection, transaction, data.Invoices);
                        InsertRecipes(connection, transaction, data.ProductionRecipes);
                        InsertProductionOrders(connection, transaction, data.ProductionOrders);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(DatabaseInitializer.ConnectionString);
            connection.Open();
            return connection;
        }

        private static void LoadSettings(SqlConnection connection, StockData data)
        {
            data.NextInvoiceNumber = 1;
            data.NextProductionNumber = 1;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT SettingKey, SettingValue FROM AppSettings";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var key = reader.GetString(0);
                        var value = reader.GetInt32(1);
                        if (key == "NextInvoiceNumber") data.NextInvoiceNumber = value;
                        if (key == "NextProductionNumber") data.NextProductionNumber = value;
                    }
                }
            }
        }

        private static List<Product> LoadProducts(SqlConnection connection)
        {
            var products = new List<Product>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Sku, Name, Category, ProductType, UnitPrice, Quantity, ReorderLevel, LastUpdated FROM Products";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetGuid(0),
                            Sku = reader.GetString(1),
                            Name = reader.GetString(2),
                            Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                            ProductType = (ProductType)reader.GetInt32(4),
                            UnitPrice = reader.GetDecimal(5),
                            Quantity = reader.GetInt32(6),
                            ReorderLevel = reader.GetInt32(7),
                            LastUpdated = reader.GetDateTime(8)
                        });
                    }
                }
            }

            return products;
        }

        private static List<Customer> LoadCustomers(SqlConnection connection)
        {
            var customers = new List<Customer>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Address, Phone, Email, CreatedAt FROM Customers";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Address = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return customers;
        }

        private static List<StockTransaction> LoadTransactions(SqlConnection connection)
        {
            var transactions = new List<StockTransaction>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, InvoiceNumber, IsSale, ProductId, ProductName, ProductSku, Type, Quantity,
       UnitPrice, TotalValue, Notes, CustomerId, CustomerName, Timestamp
FROM StockTransactions";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new StockTransaction
                        {
                            Id = reader.GetGuid(0),
                            InvoiceNumber = reader.IsDBNull(1) ? null : reader.GetString(1),
                            IsSale = reader.GetBoolean(2),
                            ProductId = reader.GetGuid(3),
                            ProductName = reader.GetString(4),
                            ProductSku = reader.GetString(5),
                            Type = (TransactionType)reader.GetInt32(6),
                            Quantity = reader.GetInt32(7),
                            UnitPrice = reader.GetDecimal(8),
                            TotalValue = reader.GetDecimal(9),
                            Notes = reader.IsDBNull(10) ? null : reader.GetString(10),
                            CustomerId = reader.IsDBNull(11) ? (Guid?)null : reader.GetGuid(11),
                            CustomerName = reader.IsDBNull(12) ? null : reader.GetString(12),
                            Timestamp = reader.GetDateTime(13)
                        });
                    }
                }
            }

            return transactions;
        }

        private static List<Invoice> LoadInvoices(SqlConnection connection)
        {
            var invoices = new List<Invoice>();
            var lineItems = LoadInvoiceLineItems(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, InvoiceNumber, CustomerId, CustomerName, CustomerPhone, CustomerAddress,
       TotalAmount, Notes, TransactionId, CreatedAt
FROM Invoices";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var invoiceId = reader.GetGuid(0);
                        invoices.Add(new Invoice
                        {
                            Id = invoiceId,
                            InvoiceNumber = reader.GetString(1),
                            CustomerId = reader.IsDBNull(2) ? (Guid?)null : reader.GetGuid(2),
                            CustomerName = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CustomerPhone = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CustomerAddress = reader.IsDBNull(5) ? null : reader.GetString(5),
                            TotalAmount = reader.GetDecimal(6),
                            Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                            TransactionId = reader.IsDBNull(8) ? (Guid?)null : reader.GetGuid(8),
                            CreatedAt = reader.GetDateTime(9),
                            Items = lineItems.ContainsKey(invoiceId)
                                ? lineItems[invoiceId]
                                : new List<InvoiceLineItem>()
                        });
                    }
                }
            }

            return invoices;
        }

        private static Dictionary<Guid, List<InvoiceLineItem>> LoadInvoiceLineItems(SqlConnection connection)
        {
            var items = new Dictionary<Guid, List<InvoiceLineItem>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT InvoiceId, ProductId, ProductSku, ProductName, ProductCategory, Quantity, UnitPrice, LineTotal
FROM InvoiceLineItems";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var invoiceId = reader.GetGuid(0);
                        if (!items.ContainsKey(invoiceId))
                            items[invoiceId] = new List<InvoiceLineItem>();

                        items[invoiceId].Add(new InvoiceLineItem
                        {
                            ProductId = reader.GetGuid(1),
                            ProductSku = reader.GetString(2),
                            ProductName = reader.GetString(3),
                            ProductCategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Quantity = reader.GetInt32(5),
                            UnitPrice = reader.GetDecimal(6),
                            LineTotal = reader.GetDecimal(7)
                        });
                    }
                }
            }

            return items;
        }

        private static List<ProductionRecipe> LoadRecipes(SqlConnection connection)
        {
            var recipes = new List<ProductionRecipe>();
            var materials = LoadRecipeMaterials(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, Name, OutputProductId, OutputProductSku, OutputProductName, CreatedAt
FROM ProductionRecipes";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var recipeId = reader.GetGuid(0);
                        recipes.Add(new ProductionRecipe
                        {
                            Id = recipeId,
                            Name = reader.GetString(1),
                            OutputProductId = reader.GetGuid(2),
                            OutputProductSku = reader.GetString(3),
                            OutputProductName = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            Materials = materials.ContainsKey(recipeId)
                                ? materials[recipeId]
                                : new List<ProductionMaterial>()
                        });
                    }
                }
            }

            return recipes;
        }

        private static Dictionary<Guid, List<ProductionMaterial>> LoadRecipeMaterials(SqlConnection connection)
        {
            var materials = new Dictionary<Guid, List<ProductionMaterial>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT RecipeId, ProductId, ProductSku, ProductName, QuantityPerUnit
FROM RecipeMaterials";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var recipeId = reader.GetGuid(0);
                        if (!materials.ContainsKey(recipeId))
                            materials[recipeId] = new List<ProductionMaterial>();

                        materials[recipeId].Add(new ProductionMaterial
                        {
                            ProductId = reader.GetGuid(1),
                            ProductSku = reader.GetString(2),
                            ProductName = reader.GetString(3),
                            QuantityPerUnit = reader.GetInt32(4)
                        });
                    }
                }
            }

            return materials;
        }

        private static List<ProductionOrder> LoadProductionOrders(SqlConnection connection)
        {
            var orders = new List<ProductionOrder>();
            var materials = LoadProductionOrderMaterials(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, ProductionNumber, RecipeId, RecipeName, OutputProductId, OutputProductSku,
       OutputProductName, QuantityProduced, OutputUnitPrice, TotalOutputValue, Notes, Timestamp
FROM ProductionOrders";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var orderId = reader.GetGuid(0);
                        orders.Add(new ProductionOrder
                        {
                            Id = orderId,
                            ProductionNumber = reader.GetString(1),
                            RecipeId = reader.IsDBNull(2) ? (Guid?)null : reader.GetGuid(2),
                            RecipeName = reader.IsDBNull(3) ? null : reader.GetString(3),
                            OutputProductId = reader.GetGuid(4),
                            OutputProductSku = reader.GetString(5),
                            OutputProductName = reader.GetString(6),
                            QuantityProduced = reader.GetInt32(7),
                            OutputUnitPrice = reader.GetDecimal(8),
                            TotalOutputValue = reader.GetDecimal(9),
                            Notes = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Timestamp = reader.GetDateTime(11),
                            MaterialsUsed = materials.ContainsKey(orderId)
                                ? materials[orderId]
                                : new List<ProductionMaterial>()
                        });
                    }
                }
            }

            return orders;
        }

        private static Dictionary<Guid, List<ProductionMaterial>> LoadProductionOrderMaterials(SqlConnection connection)
        {
            var materials = new Dictionary<Guid, List<ProductionMaterial>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT ProductionOrderId, ProductId, ProductSku, ProductName, QuantityPerUnit
FROM ProductionOrderMaterials";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var orderId = reader.GetGuid(0);
                        if (!materials.ContainsKey(orderId))
                            materials[orderId] = new List<ProductionMaterial>();

                        materials[orderId].Add(new ProductionMaterial
                        {
                            ProductId = reader.GetGuid(1),
                            ProductSku = reader.GetString(2),
                            ProductName = reader.GetString(3),
                            QuantityPerUnit = reader.GetInt32(4)
                        });
                    }
                }
            }

            return materials;
        }

        private static void ClearTables(SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"
DELETE FROM InvoiceLineItems;
DELETE FROM Invoices;
DELETE FROM ProductionOrderMaterials;
DELETE FROM ProductionOrders;
DELETE FROM RecipeMaterials;
DELETE FROM ProductionRecipes;
DELETE FROM StockTransactions;
DELETE FROM Products;
DELETE FROM Customers;
DELETE FROM AppSettings;";

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private static void InsertSettings(SqlConnection connection, SqlTransaction transaction, StockData data)
        {
            InsertSetting(connection, transaction, "NextInvoiceNumber", data.NextInvoiceNumber);
            InsertSetting(connection, transaction, "NextProductionNumber", data.NextProductionNumber);
        }

        private static void InsertSetting(SqlConnection connection, SqlTransaction transaction, string key, int value)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "INSERT INTO AppSettings (SettingKey, SettingValue) VALUES (@Key, @Value)";
                command.Parameters.AddWithValue("@Key", key);
                command.Parameters.AddWithValue("@Value", value);
                command.ExecuteNonQuery();
            }
        }

        private static void InsertProducts(SqlConnection connection, SqlTransaction transaction, IEnumerable<Product> products)
        {
            foreach (var product in products ?? Enumerable.Empty<Product>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO Products (Id, Sku, Name, Category, ProductType, UnitPrice, Quantity, ReorderLevel, LastUpdated)
VALUES (@Id, @Sku, @Name, @Category, @ProductType, @UnitPrice, @Quantity, @ReorderLevel, @LastUpdated)";
                    command.Parameters.AddWithValue("@Id", product.Id);
                    command.Parameters.AddWithValue("@Sku", (object)product.Sku ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Name", (object)product.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Category", (object)product.Category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ProductType", (int)product.ProductType);
                    command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    command.Parameters.AddWithValue("@ReorderLevel", product.ReorderLevel);
                    command.Parameters.AddWithValue("@LastUpdated", product.LastUpdated);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertCustomers(SqlConnection connection, SqlTransaction transaction, IEnumerable<Customer> customers)
        {
            foreach (var customer in customers ?? Enumerable.Empty<Customer>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO Customers (Id, Name, Address, Phone, Email, CreatedAt)
VALUES (@Id, @Name, @Address, @Phone, @Email, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", customer.Id);
                    command.Parameters.AddWithValue("@Name", (object)customer.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertTransactions(SqlConnection connection, SqlTransaction transaction, IEnumerable<StockTransaction> transactions)
        {
            foreach (var txn in transactions ?? Enumerable.Empty<StockTransaction>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO StockTransactions
    (Id, InvoiceNumber, IsSale, ProductId, ProductName, ProductSku, Type, Quantity,
     UnitPrice, TotalValue, Notes, CustomerId, CustomerName, Timestamp)
VALUES
    (@Id, @InvoiceNumber, @IsSale, @ProductId, @ProductName, @ProductSku, @Type, @Quantity,
     @UnitPrice, @TotalValue, @Notes, @CustomerId, @CustomerName, @Timestamp)";
                    command.Parameters.AddWithValue("@Id", txn.Id);
                    command.Parameters.AddWithValue("@InvoiceNumber", (object)txn.InvoiceNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsSale", txn.IsSale);
                    command.Parameters.AddWithValue("@ProductId", txn.ProductId);
                    command.Parameters.AddWithValue("@ProductName", (object)txn.ProductName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ProductSku", (object)txn.ProductSku ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Type", (int)txn.Type);
                    command.Parameters.AddWithValue("@Quantity", txn.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", txn.UnitPrice);
                    command.Parameters.AddWithValue("@TotalValue", txn.TotalValue);
                    command.Parameters.AddWithValue("@Notes", (object)txn.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerId", (object)txn.CustomerId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerName", (object)txn.CustomerName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Timestamp", txn.Timestamp);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertInvoices(SqlConnection connection, SqlTransaction transaction, IEnumerable<Invoice> invoices)
        {
            foreach (var invoice in invoices ?? Enumerable.Empty<Invoice>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO Invoices
    (Id, InvoiceNumber, CustomerId, CustomerName, CustomerPhone, CustomerAddress,
     TotalAmount, Notes, TransactionId, CreatedAt)
VALUES
    (@Id, @InvoiceNumber, @CustomerId, @CustomerName, @CustomerPhone, @CustomerAddress,
     @TotalAmount, @Notes, @TransactionId, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", invoice.Id);
                    command.Parameters.AddWithValue("@InvoiceNumber", (object)invoice.InvoiceNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerId", (object)invoice.CustomerId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerName", (object)invoice.CustomerName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerPhone", (object)invoice.CustomerPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerAddress", (object)invoice.CustomerAddress ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                    command.Parameters.AddWithValue("@Notes", (object)invoice.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TransactionId", (object)invoice.TransactionId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", invoice.CreatedAt);
                    command.ExecuteNonQuery();
                }

                foreach (var item in invoice.Items ?? new List<InvoiceLineItem>())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
INSERT INTO InvoiceLineItems
    (Id, InvoiceId, ProductId, ProductSku, ProductName, ProductCategory, Quantity, UnitPrice, LineTotal)
VALUES
    (@Id, @InvoiceId, @ProductId, @ProductSku, @ProductName, @ProductCategory, @Quantity, @UnitPrice, @LineTotal)";
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@InvoiceId", invoice.Id);
                        command.Parameters.AddWithValue("@ProductId", item.ProductId);
                        command.Parameters.AddWithValue("@ProductSku", (object)item.ProductSku ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductName", (object)item.ProductName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductCategory", (object)item.ProductCategory ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        command.Parameters.AddWithValue("@LineTotal", item.LineTotal);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void InsertRecipes(SqlConnection connection, SqlTransaction transaction, IEnumerable<ProductionRecipe> recipes)
        {
            foreach (var recipe in recipes ?? Enumerable.Empty<ProductionRecipe>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO ProductionRecipes
    (Id, Name, OutputProductId, OutputProductSku, OutputProductName, CreatedAt)
VALUES
    (@Id, @Name, @OutputProductId, @OutputProductSku, @OutputProductName, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", recipe.Id);
                    command.Parameters.AddWithValue("@Name", (object)recipe.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OutputProductId", recipe.OutputProductId);
                    command.Parameters.AddWithValue("@OutputProductSku", (object)recipe.OutputProductSku ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OutputProductName", (object)recipe.OutputProductName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", recipe.CreatedAt);
                    command.ExecuteNonQuery();
                }

                foreach (var material in recipe.Materials ?? new List<ProductionMaterial>())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
INSERT INTO RecipeMaterials
    (Id, RecipeId, ProductId, ProductSku, ProductName, QuantityPerUnit)
VALUES
    (@Id, @RecipeId, @ProductId, @ProductSku, @ProductName, @QuantityPerUnit)";
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@RecipeId", recipe.Id);
                        command.Parameters.AddWithValue("@ProductId", material.ProductId);
                        command.Parameters.AddWithValue("@ProductSku", (object)material.ProductSku ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductName", (object)material.ProductName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@QuantityPerUnit", material.QuantityPerUnit);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void InsertProductionOrders(SqlConnection connection, SqlTransaction transaction, IEnumerable<ProductionOrder> orders)
        {
            foreach (var order in orders ?? Enumerable.Empty<ProductionOrder>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO ProductionOrders
    (Id, ProductionNumber, RecipeId, RecipeName, OutputProductId, OutputProductSku, OutputProductName,
     QuantityProduced, OutputUnitPrice, TotalOutputValue, Notes, Timestamp)
VALUES
    (@Id, @ProductionNumber, @RecipeId, @RecipeName, @OutputProductId, @OutputProductSku, @OutputProductName,
     @QuantityProduced, @OutputUnitPrice, @TotalOutputValue, @Notes, @Timestamp)";
                    command.Parameters.AddWithValue("@Id", order.Id);
                    command.Parameters.AddWithValue("@ProductionNumber", (object)order.ProductionNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RecipeId", (object)order.RecipeId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RecipeName", (object)order.RecipeName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OutputProductId", order.OutputProductId);
                    command.Parameters.AddWithValue("@OutputProductSku", (object)order.OutputProductSku ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OutputProductName", (object)order.OutputProductName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@QuantityProduced", order.QuantityProduced);
                    command.Parameters.AddWithValue("@OutputUnitPrice", order.OutputUnitPrice);
                    command.Parameters.AddWithValue("@TotalOutputValue", order.TotalOutputValue);
                    command.Parameters.AddWithValue("@Notes", (object)order.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Timestamp", order.Timestamp);
                    command.ExecuteNonQuery();
                }

                foreach (var material in order.MaterialsUsed ?? new List<ProductionMaterial>())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
INSERT INTO ProductionOrderMaterials
    (Id, ProductionOrderId, ProductId, ProductSku, ProductName, QuantityPerUnit)
VALUES
    (@Id, @ProductionOrderId, @ProductId, @ProductSku, @ProductName, @QuantityPerUnit)";
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@ProductionOrderId", order.Id);
                        command.Parameters.AddWithValue("@ProductId", material.ProductId);
                        command.Parameters.AddWithValue("@ProductSku", (object)material.ProductSku ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductName", (object)material.ProductName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@QuantityPerUnit", material.QuantityPerUnit);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
