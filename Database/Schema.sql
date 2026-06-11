-- Stock Management database schema (SQL Server)
-- Default connection uses LocalDB database name: StockManagement

IF DB_ID(N'StockManagement') IS NULL
BEGIN
    CREATE DATABASE StockManagement;
END
GO

USE StockManagement;
GO

-- See DatabaseInitializer.cs for runtime schema creation used by the app.
