using Microsoft.Data.SqlClient;

namespace AdoNetLibrary.Tests;

internal static class TestDatabaseHelper
{
    public static string? GetConnectionString() => Environment.GetEnvironmentVariable("ADO_NET_TEST_CONNECTION_STRING");

    public static void EnsureDatabaseObjects(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        string[] commands =
        [
            @"IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;",
            @"IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;",
            @"
            CREATE TABLE dbo.Products
            (
                Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0)
            );",
            @"
            CREATE TABLE dbo.Orders
            (
                Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                ProductId INT NOT NULL,
                Quantity INT NOT NULL CHECK (Quantity > 0),
                OrderDate DATETIME2 NOT NULL,
                Status NVARCHAR(20) NOT NULL,
                CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_CreatedDate DEFAULT SYSUTCDATETIME(),
                UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_UpdatedDate DEFAULT SYSUTCDATETIME(),
                CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products (Id),
                CONSTRAINT CK_Orders_Status CHECK (Status IN ('NotStarted', 'Loading', 'InProgress', 'Arrived', 'Unloading', 'Cancelled', 'Done'))
            );",
            @"IF OBJECT_ID('dbo.usp_GetOrders', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetOrders;",
            @"
            EXEC('CREATE PROCEDURE dbo.usp_GetOrders
                @month INT = NULL,
                @year INT = NULL,
                @status NVARCHAR(20) = NULL,
                @productId INT = NULL
            AS
            BEGIN
                SET NOCOUNT ON;
                                SELECT Id, ProductId, Quantity, OrderDate, Status, CreatedDate, UpdatedDate
                FROM dbo.Orders
                WHERE (@month IS NULL OR MONTH(OrderDate) = @month)
                  AND (@year IS NULL OR YEAR(OrderDate) = @year)
                  AND (@status IS NULL OR Status = @status)
                  AND (@productId IS NULL OR ProductId = @productId)
                ORDER BY Id;
            END;');",
            @"IF OBJECT_ID('dbo.usp_DeleteOrders', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_DeleteOrders;",
            @"
            EXEC('CREATE PROCEDURE dbo.usp_DeleteOrders
                @month INT = NULL,
                @year INT = NULL,
                @status NVARCHAR(20) = NULL,
                @productId INT = NULL
            AS
            BEGIN
                DELETE FROM dbo.Orders
                WHERE (@month IS NULL OR MONTH(OrderDate) = @month)
                  AND (@year IS NULL OR YEAR(OrderDate) = @year)
                  AND (@status IS NULL OR Status = @status)
                  AND (@productId IS NULL OR ProductId = @productId);
            END;');",
        ];

        foreach (string sql in commands)
        {
            using var command = new SqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
