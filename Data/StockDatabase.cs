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
                data.Suppliers = LoadSuppliers(connection);
                data.Transactions = LoadTransactions(connection);
                data.Invoices = LoadInvoices(connection);
                data.Accounts = LoadAccounts(connection);
                data.BankAccounts = LoadBankAccounts(connection);
                data.JournalEntries = LoadJournalEntries(connection);
                data.CustomerPayments = LoadCustomerPayments(connection);
                data.SupplierPayments = LoadSupplierPayments(connection);
                data.BusinessExpenses = LoadBusinessExpenses(connection);
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
                        InsertSuppliers(connection, transaction, data.Suppliers);
                        InsertTransactions(connection, transaction, data.Transactions);
                        InsertInvoices(connection, transaction, data.Invoices);
                        InsertAccounts(connection, transaction, data.Accounts);
                        InsertBankAccounts(connection, transaction, data.BankAccounts);
                        InsertJournalEntries(connection, transaction, data.JournalEntries);
                        InsertCustomerPayments(connection, transaction, data.CustomerPayments);
                        InsertSupplierPayments(connection, transaction, data.SupplierPayments);
                        InsertBusinessExpenses(connection, transaction, data.BusinessExpenses);
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
            data.NextStockInNumber = 1;

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
                        if (key == "NextStockInNumber") data.NextStockInNumber = value;
                    }
                }
            }
        }

        private static List<Product> LoadProducts(SqlConnection connection)
        {
            var products = new List<Product>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Sku, Name, Category, ProductType, UnitPrice, UnitCost, Quantity, ReorderLevel, LastUpdated FROM Products";
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
                            UnitCost = reader.GetDecimal(6),
                            Quantity = reader.GetInt32(7),
                            ReorderLevel = reader.GetInt32(8),
                            LastUpdated = reader.GetDateTime(9)
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

        private static List<Supplier> LoadSuppliers(SqlConnection connection)
        {
            var suppliers = new List<Supplier>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Address, Phone, Email, CreatedAt FROM Suppliers";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        suppliers.Add(new Supplier
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

            return suppliers;
        }

        private static List<StockTransaction> LoadTransactions(SqlConnection connection)
        {
            var transactions = new List<StockTransaction>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, InvoiceNumber, IsSale, ProductId, ProductName, ProductSku, Type, Quantity,
       UnitPrice, TotalValue, Notes, CustomerId, CustomerName, Timestamp, SupplierId, SupplierName,
       StockInBatchId, StockInNumber, Status, VoidedAt, VoidReason
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
                            Timestamp = reader.GetDateTime(13),
                            SupplierId = reader.FieldCount > 14 && !reader.IsDBNull(14) ? (Guid?)reader.GetGuid(14) : null,
                            SupplierName = reader.FieldCount > 15 && !reader.IsDBNull(15) ? reader.GetString(15) : null,
                            StockInBatchId = reader.FieldCount > 16 && !reader.IsDBNull(16) ? (Guid?)reader.GetGuid(16) : null,
                            StockInNumber = reader.FieldCount > 17 && !reader.IsDBNull(17) ? reader.GetString(17) : null,
                            Status = reader.FieldCount > 18 && !reader.IsDBNull(18)
                                ? (OperationalStatus)reader.GetInt32(18)
                                : OperationalStatus.Active,
                            VoidedAt = reader.FieldCount > 19 && !reader.IsDBNull(19) ? (DateTime?)reader.GetDateTime(19) : null,
                            VoidReason = reader.FieldCount > 20 && !reader.IsDBNull(20) ? reader.GetString(20) : null
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
       TotalAmount, AmountPaid, Notes, TransactionId, CreatedAt, Status, VoidedAt, VoidReason,
       DiscountAmount
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
                            AmountPaid = reader.GetDecimal(7),
                            Notes = reader.IsDBNull(8) ? null : reader.GetString(8),
                            TransactionId = reader.IsDBNull(9) ? (Guid?)null : reader.GetGuid(9),
                            CreatedAt = reader.GetDateTime(10),
                            Status = reader.FieldCount > 11 && !reader.IsDBNull(11)
                                ? (OperationalStatus)reader.GetInt32(11)
                                : OperationalStatus.Active,
                            VoidedAt = reader.FieldCount > 12 && !reader.IsDBNull(12) ? (DateTime?)reader.GetDateTime(12) : null,
                            VoidReason = reader.FieldCount > 13 && !reader.IsDBNull(13) ? reader.GetString(13) : null,
                            DiscountAmount = reader.FieldCount > 14 && !reader.IsDBNull(14) ? reader.GetDecimal(14) : 0m,
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
SELECT InvoiceId, ProductId, ProductSku, ProductName, ProductCategory, Quantity, UnitPrice, LineTotal, UnitCost
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
                            LineTotal = reader.GetDecimal(7),
                            UnitCost = reader.GetDecimal(8)
                        });
                    }
                }
            }

            return items;
        }

        private static List<Account> LoadAccounts(SqlConnection connection)
        {
            var accounts = new List<Account>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, Code, Name, Type, IsSystem, IsActive, CreatedAt
FROM Accounts
ORDER BY Code";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(new Account
                        {
                            Id = reader.GetGuid(0),
                            Code = reader.GetString(1),
                            Name = reader.GetString(2),
                            Type = (AccountType)reader.GetInt32(3),
                            IsSystem = reader.GetBoolean(4),
                            IsActive = reader.GetBoolean(5),
                            CreatedAt = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return accounts;
        }

        private static List<BankAccount> LoadBankAccounts(SqlConnection connection)
        {
            var banks = new List<BankAccount>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, Name, AccountNumber, Branch, Notes, GlAccountId, IsActive, CreatedAt
FROM BankAccounts
ORDER BY Name";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        banks.Add(new BankAccount
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            AccountNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Branch = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                            GlAccountId = reader.GetGuid(5),
                            IsActive = reader.GetBoolean(6),
                            CreatedAt = reader.GetDateTime(7)
                        });
                    }
                }
            }

            return banks;
        }

        private static List<JournalEntry> LoadJournalEntries(SqlConnection connection)
        {
            var entries = new List<JournalEntry>();
            var lines = LoadJournalLines(connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, EntryDate, ReferenceType, ReferenceId, ReferenceNumber, Description, CreatedAt
FROM JournalEntries
ORDER BY EntryDate DESC, CreatedAt DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entryId = reader.GetGuid(0);
                        entries.Add(new JournalEntry
                        {
                            Id = entryId,
                            EntryDate = reader.GetDateTime(1),
                            ReferenceType = (JournalReferenceType)reader.GetInt32(2),
                            ReferenceId = reader.IsDBNull(3) ? (Guid?)null : reader.GetGuid(3),
                            ReferenceNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            Lines = lines.ContainsKey(entryId) ? lines[entryId] : new List<JournalLine>()
                        });
                    }
                }
            }

            return entries;
        }

        private static Dictionary<Guid, List<JournalLine>> LoadJournalLines(SqlConnection connection)
        {
            var lines = new Dictionary<Guid, List<JournalLine>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, JournalEntryId, AccountId, AccountCode, AccountName, CustomerId, CustomerName, Debit, Credit
FROM JournalLines";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entryId = reader.GetGuid(1);
                        if (!lines.ContainsKey(entryId))
                            lines[entryId] = new List<JournalLine>();

                        lines[entryId].Add(new JournalLine
                        {
                            Id = reader.GetGuid(0),
                            JournalEntryId = entryId,
                            AccountId = reader.GetGuid(2),
                            AccountCode = reader.GetString(3),
                            AccountName = reader.GetString(4),
                            CustomerId = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5),
                            CustomerName = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Debit = reader.GetDecimal(7),
                            Credit = reader.GetDecimal(8)
                        });
                    }
                }
            }

            return lines;
        }

        private static List<CustomerPayment> LoadCustomerPayments(SqlConnection connection)
        {
            var payments = new List<CustomerPayment>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, CustomerId, CustomerName, InvoiceId, InvoiceNumber, CashAccountId, CashAccountName,
       Amount, PaymentMethod, Reference, Notes, PaidAt, IsVoided, VoidedAt,
       ReceiptRelativePath, ReceiptOriginalName, ReceiptContentType, ReceiptSizeBytes, ReceiptSha256,
       IsAdvance
FROM CustomerPayments
ORDER BY PaidAt DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new CustomerPayment
                        {
                            Id = reader.GetGuid(0),
                            CustomerId = reader.GetGuid(1),
                            CustomerName = reader.GetString(2),
                            InvoiceId = reader.IsDBNull(3) ? (Guid?)null : reader.GetGuid(3),
                            InvoiceNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CashAccountId = reader.GetGuid(5),
                            CashAccountName = reader.GetString(6),
                            Amount = reader.GetDecimal(7),
                            PaymentMethod = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Reference = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Notes = reader.IsDBNull(10) ? null : reader.GetString(10),
                            PaidAt = reader.GetDateTime(11),
                            IsVoided = reader.FieldCount > 12 && !reader.IsDBNull(12) && reader.GetBoolean(12),
                            VoidedAt = reader.FieldCount > 13 && !reader.IsDBNull(13) ? (DateTime?)reader.GetDateTime(13) : null,
                            ReceiptRelativePath = reader.FieldCount > 14 && !reader.IsDBNull(14) ? reader.GetString(14) : null,
                            ReceiptOriginalName = reader.FieldCount > 15 && !reader.IsDBNull(15) ? reader.GetString(15) : null,
                            ReceiptContentType = reader.FieldCount > 16 && !reader.IsDBNull(16) ? reader.GetString(16) : null,
                            ReceiptSizeBytes = reader.FieldCount > 17 && !reader.IsDBNull(17) ? reader.GetInt64(17) : 0L,
                            ReceiptSha256 = reader.FieldCount > 18 && !reader.IsDBNull(18) ? reader.GetString(18) : null,
                            IsAdvance = reader.FieldCount > 19 && !reader.IsDBNull(19) && reader.GetBoolean(19)
                        });
                    }
                }
            }

            return payments;
        }

        private static List<SupplierPayment> LoadSupplierPayments(SqlConnection connection)
        {
            var payments = new List<SupplierPayment>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, SupplierId, SupplierName, Amount, PaymentMethod, Reference, Notes, PaidAt, IsVoided, VoidedAt,
       CashAccountId, CashAccountName,
       ReceiptRelativePath, ReceiptOriginalName, ReceiptContentType, ReceiptSizeBytes, ReceiptSha256,
       IsAdvance
FROM SupplierPayments
ORDER BY PaidAt DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new SupplierPayment
                        {
                            Id = reader.GetGuid(0),
                            SupplierId = reader.GetGuid(1),
                            SupplierName = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Amount = reader.GetDecimal(3),
                            PaymentMethod = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Reference = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Notes = reader.IsDBNull(6) ? null : reader.GetString(6),
                            PaidAt = reader.GetDateTime(7),
                            IsVoided = reader.FieldCount > 8 && !reader.IsDBNull(8) && reader.GetBoolean(8),
                            VoidedAt = reader.FieldCount > 9 && !reader.IsDBNull(9) ? (DateTime?)reader.GetDateTime(9) : null,
                            CashAccountId = reader.FieldCount > 10 && !reader.IsDBNull(10) ? reader.GetGuid(10) : Guid.Empty,
                            CashAccountName = reader.FieldCount > 11 && !reader.IsDBNull(11) ? reader.GetString(11) : null,
                            ReceiptRelativePath = reader.FieldCount > 12 && !reader.IsDBNull(12) ? reader.GetString(12) : null,
                            ReceiptOriginalName = reader.FieldCount > 13 && !reader.IsDBNull(13) ? reader.GetString(13) : null,
                            ReceiptContentType = reader.FieldCount > 14 && !reader.IsDBNull(14) ? reader.GetString(14) : null,
                            ReceiptSizeBytes = reader.FieldCount > 15 && !reader.IsDBNull(15) ? reader.GetInt64(15) : 0L,
                            ReceiptSha256 = reader.FieldCount > 16 && !reader.IsDBNull(16) ? reader.GetString(16) : null,
                            IsAdvance = reader.FieldCount > 17 && !reader.IsDBNull(17) && reader.GetBoolean(17)
                        });
                    }
                }
            }

            return payments;
        }

        private static List<BusinessExpense> LoadBusinessExpenses(SqlConnection connection)
        {
            var expenses = new List<BusinessExpense>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT Id, ExpenseAccountId, ExpenseAccountName, CashAccountId, CashAccountName,
       Amount, Reference, Notes, PaidAt
FROM BusinessExpenses
ORDER BY PaidAt DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expenses.Add(new BusinessExpense
                        {
                            Id = reader.GetGuid(0),
                            ExpenseAccountId = reader.GetGuid(1),
                            ExpenseAccountName = reader.GetString(2),
                            CashAccountId = reader.GetGuid(3),
                            CashAccountName = reader.GetString(4),
                            Amount = reader.GetDecimal(5),
                            Reference = reader.IsDBNull(6) ? null : reader.GetString(6),
                            Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                            PaidAt = reader.GetDateTime(8)
                        });
                    }
                }
            }

            return expenses;
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
       OutputProductName, QuantityProduced, OutputUnitPrice, TotalOutputValue, Notes, Timestamp,
       Status, VoidedAt, VoidReason, PriorOutputQuantity, PriorOutputUnitCost, BatchUnitCost
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
                            Status = reader.FieldCount > 12 && !reader.IsDBNull(12)
                                ? (OperationalStatus)reader.GetInt32(12)
                                : OperationalStatus.Active,
                            VoidedAt = reader.FieldCount > 13 && !reader.IsDBNull(13) ? (DateTime?)reader.GetDateTime(13) : null,
                            VoidReason = reader.FieldCount > 14 && !reader.IsDBNull(14) ? reader.GetString(14) : null,
                            PriorOutputQuantity = reader.FieldCount > 15 && !reader.IsDBNull(15) ? reader.GetInt32(15) : 0,
                            PriorOutputUnitCost = reader.FieldCount > 16 && !reader.IsDBNull(16) ? reader.GetDecimal(16) : 0,
                            BatchUnitCost = reader.FieldCount > 17 && !reader.IsDBNull(17) ? reader.GetDecimal(17) : 0,
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
DELETE FROM JournalLines;
DELETE FROM JournalEntries;
DELETE FROM CustomerPayments;
DELETE FROM SupplierPayments;
DELETE FROM BusinessExpenses;
DELETE FROM BankAccounts;
DELETE FROM Accounts;
DELETE FROM InvoiceLineItems;
DELETE FROM Invoices;
DELETE FROM ProductionOrderMaterials;
DELETE FROM ProductionOrders;
DELETE FROM RecipeMaterials;
DELETE FROM ProductionRecipes;
DELETE FROM StockTransactions;
DELETE FROM Products;
DELETE FROM Customers;
DELETE FROM Suppliers;
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
            InsertSetting(connection, transaction, "NextStockInNumber", data.NextStockInNumber);
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
INSERT INTO Products (Id, Sku, Name, Category, ProductType, UnitPrice, UnitCost, Quantity, ReorderLevel, LastUpdated)
VALUES (@Id, @Sku, @Name, @Category, @ProductType, @UnitPrice, @UnitCost, @Quantity, @ReorderLevel, @LastUpdated)";
                    command.Parameters.AddWithValue("@Id", product.Id);
                    command.Parameters.AddWithValue("@Sku", (object)product.Sku ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Name", (object)product.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Category", (object)product.Category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ProductType", (int)product.ProductType);
                    command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                    command.Parameters.AddWithValue("@UnitCost", product.UnitCost);
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

        private static void InsertSuppliers(SqlConnection connection, SqlTransaction transaction, IEnumerable<Supplier> suppliers)
        {
            foreach (var supplier in suppliers ?? Enumerable.Empty<Supplier>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO Suppliers (Id, Name, Address, Phone, Email, CreatedAt)
VALUES (@Id, @Name, @Address, @Phone, @Email, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", supplier.Id);
                    command.Parameters.AddWithValue("@Name", (object)supplier.Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)supplier.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)supplier.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)supplier.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", supplier.CreatedAt);
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
     UnitPrice, TotalValue, Notes, CustomerId, CustomerName, Timestamp, SupplierId, SupplierName,
     StockInBatchId, StockInNumber, Status, VoidedAt, VoidReason)
VALUES
    (@Id, @InvoiceNumber, @IsSale, @ProductId, @ProductName, @ProductSku, @Type, @Quantity,
     @UnitPrice, @TotalValue, @Notes, @CustomerId, @CustomerName, @Timestamp, @SupplierId, @SupplierName,
     @StockInBatchId, @StockInNumber, @Status, @VoidedAt, @VoidReason)";
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
                    command.Parameters.AddWithValue("@SupplierId", (object)txn.SupplierId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SupplierName", (object)txn.SupplierName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StockInBatchId", (object)txn.StockInBatchId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StockInNumber", (object)txn.StockInNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", (int)txn.Status);
                    command.Parameters.AddWithValue("@VoidedAt", (object)txn.VoidedAt ?? DBNull.Value);
                    command.Parameters.AddWithValue("@VoidReason", (object)txn.VoidReason ?? DBNull.Value);
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
     TotalAmount, AmountPaid, Notes, TransactionId, CreatedAt, Status, VoidedAt, VoidReason, DiscountAmount)
VALUES
    (@Id, @InvoiceNumber, @CustomerId, @CustomerName, @CustomerPhone, @CustomerAddress,
     @TotalAmount, @AmountPaid, @Notes, @TransactionId, @CreatedAt, @Status, @VoidedAt, @VoidReason, @DiscountAmount)";
                    command.Parameters.AddWithValue("@Id", invoice.Id);
                    command.Parameters.AddWithValue("@InvoiceNumber", (object)invoice.InvoiceNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerId", (object)invoice.CustomerId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerName", (object)invoice.CustomerName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerPhone", (object)invoice.CustomerPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerAddress", (object)invoice.CustomerAddress ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                    command.Parameters.AddWithValue("@AmountPaid", invoice.AmountPaid);
                    command.Parameters.AddWithValue("@Notes", (object)invoice.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TransactionId", (object)invoice.TransactionId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", invoice.CreatedAt);
                    command.Parameters.AddWithValue("@Status", (int)invoice.Status);
                    command.Parameters.AddWithValue("@VoidedAt", (object)invoice.VoidedAt ?? DBNull.Value);
                    command.Parameters.AddWithValue("@VoidReason", (object)invoice.VoidReason ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DiscountAmount", invoice.DiscountAmount);
                    command.ExecuteNonQuery();
                }

                foreach (var item in invoice.Items ?? new List<InvoiceLineItem>())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
INSERT INTO InvoiceLineItems
    (Id, InvoiceId, ProductId, ProductSku, ProductName, ProductCategory, Quantity, UnitPrice, LineTotal, UnitCost)
VALUES
    (@Id, @InvoiceId, @ProductId, @ProductSku, @ProductName, @ProductCategory, @Quantity, @UnitPrice, @LineTotal, @UnitCost)";
                        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        command.Parameters.AddWithValue("@InvoiceId", invoice.Id);
                        command.Parameters.AddWithValue("@ProductId", item.ProductId);
                        command.Parameters.AddWithValue("@ProductSku", (object)item.ProductSku ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductName", (object)item.ProductName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductCategory", (object)item.ProductCategory ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Quantity", item.Quantity);
                        command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                        command.Parameters.AddWithValue("@LineTotal", item.LineTotal);
                        command.Parameters.AddWithValue("@UnitCost", item.UnitCost);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void InsertAccounts(SqlConnection connection, SqlTransaction transaction, IEnumerable<Account> accounts)
        {
            foreach (var account in accounts ?? Enumerable.Empty<Account>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO Accounts (Id, Code, Name, Type, IsSystem, IsActive, CreatedAt)
VALUES (@Id, @Code, @Name, @Type, @IsSystem, @IsActive, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", account.Id);
                    command.Parameters.AddWithValue("@Code", account.Code);
                    command.Parameters.AddWithValue("@Name", account.Name);
                    command.Parameters.AddWithValue("@Type", (int)account.Type);
                    command.Parameters.AddWithValue("@IsSystem", account.IsSystem);
                    command.Parameters.AddWithValue("@IsActive", account.IsActive);
                    command.Parameters.AddWithValue("@CreatedAt", account.CreatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertBankAccounts(SqlConnection connection, SqlTransaction transaction, IEnumerable<BankAccount> banks)
        {
            foreach (var bank in banks ?? Enumerable.Empty<BankAccount>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO BankAccounts
    (Id, Name, AccountNumber, Branch, Notes, GlAccountId, IsActive, CreatedAt)
VALUES
    (@Id, @Name, @AccountNumber, @Branch, @Notes, @GlAccountId, @IsActive, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", bank.Id);
                    command.Parameters.AddWithValue("@Name", bank.Name ?? string.Empty);
                    command.Parameters.AddWithValue("@AccountNumber", (object)bank.AccountNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Branch", (object)bank.Branch ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)bank.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@GlAccountId", bank.GlAccountId);
                    command.Parameters.AddWithValue("@IsActive", bank.IsActive);
                    command.Parameters.AddWithValue("@CreatedAt", bank.CreatedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertJournalEntries(SqlConnection connection, SqlTransaction transaction, IEnumerable<JournalEntry> entries)
        {
            foreach (var entry in entries ?? Enumerable.Empty<JournalEntry>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO JournalEntries
    (Id, EntryDate, ReferenceType, ReferenceId, ReferenceNumber, Description, CreatedAt)
VALUES
    (@Id, @EntryDate, @ReferenceType, @ReferenceId, @ReferenceNumber, @Description, @CreatedAt)";
                    command.Parameters.AddWithValue("@Id", entry.Id);
                    command.Parameters.AddWithValue("@EntryDate", entry.EntryDate);
                    command.Parameters.AddWithValue("@ReferenceType", (int)entry.ReferenceType);
                    command.Parameters.AddWithValue("@ReferenceId", (object)entry.ReferenceId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReferenceNumber", (object)entry.ReferenceNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Description", (object)entry.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedAt", entry.CreatedAt);
                    command.ExecuteNonQuery();
                }

                foreach (var line in entry.Lines ?? new List<JournalLine>())
                {
                    line.JournalEntryId = entry.Id;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
INSERT INTO JournalLines
    (Id, JournalEntryId, AccountId, AccountCode, AccountName, CustomerId, CustomerName, Debit, Credit)
VALUES
    (@Id, @JournalEntryId, @AccountId, @AccountCode, @AccountName, @CustomerId, @CustomerName, @Debit, @Credit)";
                        command.Parameters.AddWithValue("@Id", line.Id);
                        command.Parameters.AddWithValue("@JournalEntryId", line.JournalEntryId);
                        command.Parameters.AddWithValue("@AccountId", line.AccountId);
                        command.Parameters.AddWithValue("@AccountCode", line.AccountCode);
                        command.Parameters.AddWithValue("@AccountName", line.AccountName);
                        command.Parameters.AddWithValue("@CustomerId", (object)line.CustomerId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerName", (object)line.CustomerName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Debit", line.Debit);
                        command.Parameters.AddWithValue("@Credit", line.Credit);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void InsertCustomerPayments(SqlConnection connection, SqlTransaction transaction, IEnumerable<CustomerPayment> payments)
        {
            foreach (var payment in payments ?? Enumerable.Empty<CustomerPayment>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO CustomerPayments
    (Id, CustomerId, CustomerName, InvoiceId, InvoiceNumber, CashAccountId, CashAccountName,
     Amount, PaymentMethod, Reference, Notes, PaidAt, IsVoided, VoidedAt,
     ReceiptRelativePath, ReceiptOriginalName, ReceiptContentType, ReceiptSizeBytes, ReceiptSha256,
     IsAdvance)
VALUES
    (@Id, @CustomerId, @CustomerName, @InvoiceId, @InvoiceNumber, @CashAccountId, @CashAccountName,
     @Amount, @PaymentMethod, @Reference, @Notes, @PaidAt, @IsVoided, @VoidedAt,
     @ReceiptRelativePath, @ReceiptOriginalName, @ReceiptContentType, @ReceiptSizeBytes, @ReceiptSha256,
     @IsAdvance)";
                    command.Parameters.AddWithValue("@Id", payment.Id);
                    command.Parameters.AddWithValue("@CustomerId", payment.CustomerId);
                    command.Parameters.AddWithValue("@CustomerName", payment.CustomerName);
                    command.Parameters.AddWithValue("@InvoiceId", (object)payment.InvoiceId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@InvoiceNumber", (object)payment.InvoiceNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CashAccountId", payment.CashAccountId);
                    command.Parameters.AddWithValue("@CashAccountName", payment.CashAccountName);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);
                    command.Parameters.AddWithValue("@PaymentMethod", (object)payment.PaymentMethod ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Reference", (object)payment.Reference ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)payment.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PaidAt", payment.PaidAt);
                    command.Parameters.AddWithValue("@IsVoided", payment.IsVoided);
                    command.Parameters.AddWithValue("@VoidedAt", (object)payment.VoidedAt ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptRelativePath", (object)payment.ReceiptRelativePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptOriginalName", (object)payment.ReceiptOriginalName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptContentType", (object)payment.ReceiptContentType ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptSizeBytes", payment.ReceiptSizeBytes);
                    command.Parameters.AddWithValue("@ReceiptSha256", (object)payment.ReceiptSha256 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsAdvance", payment.IsAdvance);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertSupplierPayments(SqlConnection connection, SqlTransaction transaction, IEnumerable<SupplierPayment> payments)
        {
            foreach (var payment in payments ?? Enumerable.Empty<SupplierPayment>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO SupplierPayments
    (Id, SupplierId, SupplierName, Amount, PaymentMethod, Reference, Notes, PaidAt, IsVoided, VoidedAt,
     CashAccountId, CashAccountName,
     ReceiptRelativePath, ReceiptOriginalName, ReceiptContentType, ReceiptSizeBytes, ReceiptSha256,
     IsAdvance)
VALUES
    (@Id, @SupplierId, @SupplierName, @Amount, @PaymentMethod, @Reference, @Notes, @PaidAt, @IsVoided, @VoidedAt,
     @CashAccountId, @CashAccountName,
     @ReceiptRelativePath, @ReceiptOriginalName, @ReceiptContentType, @ReceiptSizeBytes, @ReceiptSha256,
     @IsAdvance)";
                    command.Parameters.AddWithValue("@Id", payment.Id);
                    command.Parameters.AddWithValue("@SupplierId", payment.SupplierId);
                    command.Parameters.AddWithValue("@SupplierName", (object)payment.SupplierName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);
                    command.Parameters.AddWithValue("@PaymentMethod", (object)payment.PaymentMethod ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Reference", (object)payment.Reference ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)payment.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PaidAt", payment.PaidAt);
                    command.Parameters.AddWithValue("@IsVoided", payment.IsVoided);
                    command.Parameters.AddWithValue("@VoidedAt", (object)payment.VoidedAt ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CashAccountId",
                        payment.CashAccountId == Guid.Empty ? (object)DBNull.Value : payment.CashAccountId);
                    command.Parameters.AddWithValue("@CashAccountName", (object)payment.CashAccountName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptRelativePath", (object)payment.ReceiptRelativePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptOriginalName", (object)payment.ReceiptOriginalName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptContentType", (object)payment.ReceiptContentType ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReceiptSizeBytes", payment.ReceiptSizeBytes);
                    command.Parameters.AddWithValue("@ReceiptSha256", (object)payment.ReceiptSha256 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsAdvance", payment.IsAdvance);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertBusinessExpenses(SqlConnection connection, SqlTransaction transaction, IEnumerable<BusinessExpense> expenses)
        {
            foreach (var expense in expenses ?? Enumerable.Empty<BusinessExpense>())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
INSERT INTO BusinessExpenses
    (Id, ExpenseAccountId, ExpenseAccountName, CashAccountId, CashAccountName,
     Amount, Reference, Notes, PaidAt)
VALUES
    (@Id, @ExpenseAccountId, @ExpenseAccountName, @CashAccountId, @CashAccountName,
     @Amount, @Reference, @Notes, @PaidAt)";
                    command.Parameters.AddWithValue("@Id", expense.Id);
                    command.Parameters.AddWithValue("@ExpenseAccountId", expense.ExpenseAccountId);
                    command.Parameters.AddWithValue("@ExpenseAccountName", expense.ExpenseAccountName);
                    command.Parameters.AddWithValue("@CashAccountId", expense.CashAccountId);
                    command.Parameters.AddWithValue("@CashAccountName", expense.CashAccountName);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Reference", (object)expense.Reference ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Notes", (object)expense.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PaidAt", expense.PaidAt);
                    command.ExecuteNonQuery();
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
     QuantityProduced, OutputUnitPrice, TotalOutputValue, Notes, Timestamp,
     Status, VoidedAt, VoidReason, PriorOutputQuantity, PriorOutputUnitCost, BatchUnitCost)
VALUES
    (@Id, @ProductionNumber, @RecipeId, @RecipeName, @OutputProductId, @OutputProductSku, @OutputProductName,
     @QuantityProduced, @OutputUnitPrice, @TotalOutputValue, @Notes, @Timestamp,
     @Status, @VoidedAt, @VoidReason, @PriorOutputQuantity, @PriorOutputUnitCost, @BatchUnitCost)";
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
                    command.Parameters.AddWithValue("@Status", (int)order.Status);
                    command.Parameters.AddWithValue("@VoidedAt", (object)order.VoidedAt ?? DBNull.Value);
                    command.Parameters.AddWithValue("@VoidReason", (object)order.VoidReason ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PriorOutputQuantity", order.PriorOutputQuantity);
                    command.Parameters.AddWithValue("@PriorOutputUnitCost", order.PriorOutputUnitCost);
                    command.Parameters.AddWithValue("@BatchUnitCost", order.BatchUnitCost);
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
