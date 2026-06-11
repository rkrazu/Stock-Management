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
        UnitPrice DECIMAL(18, 2) NOT NULL,
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
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        LineTotal DECIMAL(18, 2) NOT NULL
    );
    CREATE INDEX IX_InvoiceLineItems_InvoiceId ON dbo.InvoiceLineItems (InvoiceId);
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
END;"
        };
    }
}
