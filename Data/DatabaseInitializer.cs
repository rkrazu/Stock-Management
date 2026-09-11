using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Stock_Managemnet.Data
{
    public static class DatabaseInitializer
    {
        public const string ConnectionStringName = "StockManagement";

        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found in App.config.");

        public static string MasterConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString)
                {
                    InitialCatalog = "master"
                };
                return builder.ConnectionString;
            }
        }

        public static void EnsureCreated()
        {
            EnsureDatabaseExists();
            EnsureSchema();
        }

        private static void EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            var databaseName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("Initial Catalog is missing from the connection string.");

            var masterBuilder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = "master"
            };

            using (var connection = new SqlConnection(masterBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "IF DB_ID(@DbName) IS NULL CREATE DATABASE [" + databaseName.Replace("]", "]]") + "];";
                    command.Parameters.AddWithValue("@DbName", databaseName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureSchema()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (var batch in SchemaBatches)
                    ExecuteBatch(connection, batch);
            }
        }

        private static void ExecuteBatch(SqlConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        private static readonly string[] SchemaBatches =
        {
            @"IF OBJECT_ID(N'dbo.AppSettings', N'U') IS NULL
CREATE TABLE dbo.AppSettings (
    SettingKey NVARCHAR(50) NOT NULL PRIMARY KEY,
    SettingValue INT NOT NULL
);",

            @"IF OBJECT_ID(N'dbo.AppCredentials', N'U') IS NULL
CREATE TABLE dbo.AppCredentials (
    Id INT NOT NULL PRIMARY KEY,
    PasswordHash NVARCHAR(256) NOT NULL,
    PasswordSalt NVARCHAR(128) NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT CK_AppCredentials_SingleUser CHECK (Id = 1)
);",

            @"IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Sku NVARCHAR(50) NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Category NVARCHAR(100) NULL,
        ProductType INT NOT NULL CONSTRAINT DF_Products_ProductType DEFAULT (0),
        UnitPrice DECIMAL(18, 2) NOT NULL,
        UnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_Products_UnitCost DEFAULT (0),
        Quantity INT NOT NULL,
        ReorderLevel INT NOT NULL,
        LastUpdated DATETIME2 NOT NULL
    );
    CREATE INDEX IX_Products_Sku ON dbo.Products (Sku);
    CREATE INDEX IX_Products_Name ON dbo.Products (Name);
END;",

            @"IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Address NVARCHAR(300) NULL,
        Phone NVARCHAR(30) NULL,
        Email NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_Customers_Name ON dbo.Customers (Name);
END;",

            @"IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Suppliers (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Address NVARCHAR(300) NULL,
        Phone NVARCHAR(30) NULL,
        Email NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_Suppliers_Name ON dbo.Suppliers (Name);
END;",

            @"IF OBJECT_ID(N'dbo.StockTransactions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.StockTransactions (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        InvoiceNumber NVARCHAR(20) NULL,
        IsSale BIT NOT NULL CONSTRAINT DF_StockTransactions_IsSale DEFAULT (0),
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        ProductSku NVARCHAR(50) NOT NULL,
        Type INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        TotalValue DECIMAL(18, 2) NOT NULL,
        Notes NVARCHAR(500) NULL,
        CustomerId UNIQUEIDENTIFIER NULL,
        CustomerName NVARCHAR(200) NULL,
        Timestamp DATETIME2 NOT NULL
    );
    CREATE INDEX IX_StockTransactions_Timestamp ON dbo.StockTransactions (Timestamp DESC);
END;",

            @"IF OBJECT_ID(N'dbo.Invoices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        InvoiceNumber NVARCHAR(20) NOT NULL,
        CustomerId UNIQUEIDENTIFIER NULL,
        CustomerName NVARCHAR(200) NULL,
        CustomerPhone NVARCHAR(30) NULL,
        CustomerAddress NVARCHAR(300) NULL,
        TotalAmount DECIMAL(18, 2) NOT NULL,
        AmountPaid DECIMAL(18, 2) NOT NULL CONSTRAINT DF_Invoices_AmountPaid DEFAULT (0),
        Notes NVARCHAR(500) NULL,
        TransactionId UNIQUEIDENTIFIER NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE UNIQUE INDEX IX_Invoices_InvoiceNumber ON dbo.Invoices (InvoiceNumber);
END;",

            @"IF OBJECT_ID(N'dbo.InvoiceLineItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InvoiceLineItems (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        InvoiceId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductSku NVARCHAR(50) NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        ProductCategory NVARCHAR(100) NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        LineTotal DECIMAL(18, 2) NOT NULL,
        UnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_InvoiceLineItems_UnitCost DEFAULT (0)
    );
    CREATE INDEX IX_InvoiceLineItems_InvoiceId ON dbo.InvoiceLineItems (InvoiceId);
END;",

            @"IF OBJECT_ID(N'dbo.Accounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounts (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Code NVARCHAR(20) NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Type INT NOT NULL,
        IsSystem BIT NOT NULL CONSTRAINT DF_Accounts_IsSystem DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_Accounts_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE UNIQUE INDEX IX_Accounts_Code ON dbo.Accounts (Code);
END;",

            @"IF OBJECT_ID(N'dbo.JournalEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JournalEntries (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        EntryDate DATETIME2 NOT NULL,
        ReferenceType INT NOT NULL,
        ReferenceId UNIQUEIDENTIFIER NULL,
        ReferenceNumber NVARCHAR(50) NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_JournalEntries_EntryDate ON dbo.JournalEntries (EntryDate DESC);
    CREATE INDEX IX_JournalEntries_Reference ON dbo.JournalEntries (ReferenceType, ReferenceId);
END;",

            @"IF OBJECT_ID(N'dbo.JournalLines', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JournalLines (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        JournalEntryId UNIQUEIDENTIFIER NOT NULL,
        AccountId UNIQUEIDENTIFIER NOT NULL,
        AccountCode NVARCHAR(20) NOT NULL,
        AccountName NVARCHAR(200) NOT NULL,
        CustomerId UNIQUEIDENTIFIER NULL,
        CustomerName NVARCHAR(200) NULL,
        Debit DECIMAL(18, 2) NOT NULL,
        Credit DECIMAL(18, 2) NOT NULL
    );
    CREATE INDEX IX_JournalLines_JournalEntryId ON dbo.JournalLines (JournalEntryId);
    CREATE INDEX IX_JournalLines_AccountId ON dbo.JournalLines (AccountId);
    CREATE INDEX IX_JournalLines_CustomerId ON dbo.JournalLines (CustomerId);
END;",

            @"IF OBJECT_ID(N'dbo.CustomerPayments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CustomerPayments (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        CustomerId UNIQUEIDENTIFIER NOT NULL,
        CustomerName NVARCHAR(200) NOT NULL,
        InvoiceId UNIQUEIDENTIFIER NULL,
        InvoiceNumber NVARCHAR(20) NULL,
        CashAccountId UNIQUEIDENTIFIER NOT NULL,
        CashAccountName NVARCHAR(200) NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        PaymentMethod NVARCHAR(50) NULL,
        Reference NVARCHAR(100) NULL,
        Notes NVARCHAR(500) NULL,
        PaidAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_CustomerPayments_CustomerId ON dbo.CustomerPayments (CustomerId);
    CREATE INDEX IX_CustomerPayments_PaidAt ON dbo.CustomerPayments (PaidAt DESC);
END;",

            @"IF OBJECT_ID(N'dbo.BusinessExpenses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BusinessExpenses (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ExpenseAccountId UNIQUEIDENTIFIER NOT NULL,
        ExpenseAccountName NVARCHAR(200) NOT NULL,
        CashAccountId UNIQUEIDENTIFIER NOT NULL,
        CashAccountName NVARCHAR(200) NOT NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        Reference NVARCHAR(100) NULL,
        Notes NVARCHAR(500) NULL,
        PaidAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_BusinessExpenses_PaidAt ON dbo.BusinessExpenses (PaidAt DESC);
    CREATE INDEX IX_BusinessExpenses_ExpenseAccountId ON dbo.BusinessExpenses (ExpenseAccountId);
END;",

            @"IF OBJECT_ID(N'dbo.ProductionRecipes', N'U') IS NULL
CREATE TABLE dbo.ProductionRecipes (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    OutputProductId UNIQUEIDENTIFIER NOT NULL,
    OutputProductSku NVARCHAR(50) NOT NULL,
    OutputProductName NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);",

            @"IF OBJECT_ID(N'dbo.RecipeMaterials', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RecipeMaterials (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        RecipeId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductSku NVARCHAR(50) NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        QuantityPerUnit INT NOT NULL
    );
    CREATE INDEX IX_RecipeMaterials_RecipeId ON dbo.RecipeMaterials (RecipeId);
END;",

            @"IF OBJECT_ID(N'dbo.ProductionOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductionOrders (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ProductionNumber NVARCHAR(20) NOT NULL,
        RecipeId UNIQUEIDENTIFIER NULL,
        RecipeName NVARCHAR(200) NULL,
        OutputProductId UNIQUEIDENTIFIER NOT NULL,
        OutputProductSku NVARCHAR(50) NOT NULL,
        OutputProductName NVARCHAR(200) NOT NULL,
        QuantityProduced INT NOT NULL,
        OutputUnitPrice DECIMAL(18, 2) NOT NULL,
        TotalOutputValue DECIMAL(18, 2) NOT NULL,
        Notes NVARCHAR(500) NULL,
        Timestamp DATETIME2 NOT NULL
    );
    CREATE INDEX IX_ProductionOrders_Timestamp ON dbo.ProductionOrders (Timestamp DESC);
END;",

            @"IF OBJECT_ID(N'dbo.ProductionOrderMaterials', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductionOrderMaterials (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ProductionOrderId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        ProductSku NVARCHAR(50) NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        QuantityPerUnit INT NOT NULL
    );
    CREATE INDEX IX_ProductionOrderMaterials_OrderId ON dbo.ProductionOrderMaterials (ProductionOrderId);
END;",

            @"IF COL_LENGTH('dbo.Products', 'ProductType') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD ProductType INT NOT NULL CONSTRAINT DF_Products_ProductType DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.InvoiceLineItems', 'ProductCategory') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceLineItems ADD ProductCategory NVARCHAR(100) NULL;
END;",

            @"IF COL_LENGTH('dbo.Invoices', 'AmountPaid') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD AmountPaid DECIMAL(18, 2) NOT NULL CONSTRAINT DF_Invoices_AmountPaid_Mig DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.Products', 'UnitCost') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD UnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_Products_UnitCost_Mig DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.InvoiceLineItems', 'UnitCost') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceLineItems ADD UnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_InvoiceLineItems_UnitCost_Mig DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.Invoices', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD Status INT NOT NULL CONSTRAINT DF_Invoices_Status DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.Invoices', 'VoidedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD VoidedAt DATETIME2 NULL;
END;",

            @"IF COL_LENGTH('dbo.Invoices', 'VoidReason') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD VoidReason NVARCHAR(500) NULL;
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD Status INT NOT NULL CONSTRAINT DF_ProductionOrders_Status DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'VoidedAt') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD VoidedAt DATETIME2 NULL;
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'VoidReason') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD VoidReason NVARCHAR(500) NULL;
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'PriorOutputQuantity') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD PriorOutputQuantity INT NOT NULL CONSTRAINT DF_ProductionOrders_PriorQty DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'PriorOutputUnitCost') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD PriorOutputUnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_ProductionOrders_PriorCost DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.ProductionOrders', 'BatchUnitCost') IS NULL
BEGIN
    ALTER TABLE dbo.ProductionOrders ADD BatchUnitCost DECIMAL(18, 2) NOT NULL CONSTRAINT DF_ProductionOrders_BatchCost DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.CustomerPayments', 'IsVoided') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD IsVoided BIT NOT NULL CONSTRAINT DF_CustomerPayments_IsVoided DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.CustomerPayments', 'VoidedAt') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD VoidedAt DATETIME2 NULL;
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'SupplierId') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD SupplierId UNIQUEIDENTIFIER NULL;
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'SupplierName') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD SupplierName NVARCHAR(200) NULL;
END;",

            @"IF OBJECT_ID(N'dbo.SupplierPayments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupplierPayments (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        SupplierId UNIQUEIDENTIFIER NOT NULL,
        SupplierName NVARCHAR(200) NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        PaymentMethod NVARCHAR(50) NULL,
        Reference NVARCHAR(100) NULL,
        Notes NVARCHAR(500) NULL,
        PaidAt DATETIME2 NOT NULL,
        IsVoided BIT NOT NULL CONSTRAINT DF_SupplierPayments_IsVoided DEFAULT (0),
        VoidedAt DATETIME2 NULL
    );
    CREATE INDEX IX_SupplierPayments_SupplierId ON dbo.SupplierPayments (SupplierId);
    CREATE INDEX IX_SupplierPayments_PaidAt ON dbo.SupplierPayments (PaidAt DESC);
END;",

            @"IF OBJECT_ID(N'dbo.BankAccounts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BankAccounts (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        AccountNumber NVARCHAR(100) NULL,
        Branch NVARCHAR(200) NULL,
        Notes NVARCHAR(500) NULL,
        GlAccountId UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_BankAccounts_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL
    );
    CREATE INDEX IX_BankAccounts_Name ON dbo.BankAccounts (Name);
END;",

            @"IF COL_LENGTH('dbo.SupplierPayments', 'CashAccountId') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD CashAccountId UNIQUEIDENTIFIER NULL;
END;",

            @"IF COL_LENGTH('dbo.SupplierPayments', 'CashAccountName') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD CashAccountName NVARCHAR(200) NULL;
END;",

            @"IF COL_LENGTH('dbo.CustomerPayments', 'PaymentMethod') IS NOT NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ALTER COLUMN PaymentMethod NVARCHAR(200) NULL;
END;",

            @"IF COL_LENGTH('dbo.SupplierPayments', 'PaymentMethod') IS NOT NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ALTER COLUMN PaymentMethod NVARCHAR(200) NULL;
END;",

            @"IF COL_LENGTH('dbo.CustomerPayments', 'ReceiptRelativePath') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD ReceiptRelativePath NVARCHAR(500) NULL;
END;",
            @"IF COL_LENGTH('dbo.CustomerPayments', 'ReceiptOriginalName') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD ReceiptOriginalName NVARCHAR(260) NULL;
END;",
            @"IF COL_LENGTH('dbo.CustomerPayments', 'ReceiptContentType') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD ReceiptContentType NVARCHAR(100) NULL;
END;",
            @"IF COL_LENGTH('dbo.CustomerPayments', 'ReceiptSizeBytes') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD ReceiptSizeBytes BIGINT NOT NULL CONSTRAINT DF_CustomerPayments_ReceiptSize DEFAULT (0);
END;",
            @"IF COL_LENGTH('dbo.CustomerPayments', 'ReceiptSha256') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD ReceiptSha256 NVARCHAR(64) NULL;
END;",

            @"IF COL_LENGTH('dbo.SupplierPayments', 'ReceiptRelativePath') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD ReceiptRelativePath NVARCHAR(500) NULL;
END;",
            @"IF COL_LENGTH('dbo.SupplierPayments', 'ReceiptOriginalName') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD ReceiptOriginalName NVARCHAR(260) NULL;
END;",
            @"IF COL_LENGTH('dbo.SupplierPayments', 'ReceiptContentType') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD ReceiptContentType NVARCHAR(100) NULL;
END;",
            @"IF COL_LENGTH('dbo.SupplierPayments', 'ReceiptSizeBytes') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD ReceiptSizeBytes BIGINT NOT NULL CONSTRAINT DF_SupplierPayments_ReceiptSize DEFAULT (0);
END;",
            @"IF COL_LENGTH('dbo.SupplierPayments', 'ReceiptSha256') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD ReceiptSha256 NVARCHAR(64) NULL;
END;",

            @"IF COL_LENGTH('dbo.CustomerPayments', 'IsAdvance') IS NULL
BEGIN
    ALTER TABLE dbo.CustomerPayments ADD IsAdvance BIT NOT NULL CONSTRAINT DF_CustomerPayments_IsAdvance DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.SupplierPayments', 'IsAdvance') IS NULL
BEGIN
    ALTER TABLE dbo.SupplierPayments ADD IsAdvance BIT NOT NULL CONSTRAINT DF_SupplierPayments_IsAdvance DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.Invoices', 'DiscountAmount') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices ADD DiscountAmount DECIMAL(18, 2) NOT NULL CONSTRAINT DF_Invoices_DiscountAmount DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'StockInBatchId') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD StockInBatchId UNIQUEIDENTIFIER NULL;
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'StockInNumber') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD StockInNumber NVARCHAR(20) NULL;
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD Status INT NOT NULL CONSTRAINT DF_StockTransactions_Status DEFAULT (0);
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'VoidedAt') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD VoidedAt DATETIME2 NULL;
END;",

            @"IF COL_LENGTH('dbo.StockTransactions', 'VoidReason') IS NULL
BEGIN
    ALTER TABLE dbo.StockTransactions ADD VoidReason NVARCHAR(500) NULL;
END;"
        };
    }
}
