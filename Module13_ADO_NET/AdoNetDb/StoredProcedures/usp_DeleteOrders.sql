CREATE PROCEDURE [dbo].[usp_DeleteOrders]
    @month INT = NULL,
    @year INT = NULL,
    @status NVARCHAR(20) = NULL,
    @productId INT = NULL
AS
BEGIN
    DELETE FROM [dbo].[Orders]
    WHERE (@month IS NULL OR MONTH([CreatedDate]) = @month)
      AND (@year IS NULL OR YEAR([CreatedDate]) = @year)
      AND (@status IS NULL OR [Status] = @status)
      AND (@productId IS NULL OR [ProductId] = @productId);
END;