IF OBJECT_ID('dbo.usp_GetOrders', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_GetOrders;
END;

EXEC('CREATE PROCEDURE dbo.usp_GetOrders
    @month INT = NULL,
    @year INT = NULL,
    @status NVARCHAR(20) = NULL,
    @productId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductId, Quantity, OrderDate, Status
    FROM dbo.Orders
    WHERE (@month IS NULL OR MONTH(OrderDate) = @month)
      AND (@year IS NULL OR YEAR(OrderDate) = @year)
      AND (@status IS NULL OR Status = @status)
      AND (@productId IS NULL OR ProductId = @productId)
    ORDER BY Id;
END;');




IF OBJECT_ID('dbo.usp_DeleteOrders', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_DeleteOrders;
END;

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
END;');
