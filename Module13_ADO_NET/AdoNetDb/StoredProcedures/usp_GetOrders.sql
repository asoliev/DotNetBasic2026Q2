CREATE PROCEDURE [dbo].[usp_GetOrders]
    @month INT = NULL,
    @year INT = NULL,
    @status NVARCHAR(20) = NULL,
    @productId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ProductId], [Status], [CreatedDate], [UpdatedDate]
    FROM [dbo].[Orders]
    WHERE (@month IS NULL OR MONTH([CreatedDate]) = @month)
      AND (@year IS NULL OR YEAR([CreatedDate]) = @year)
      AND (@status IS NULL OR [Status] = @status)
      AND (@productId IS NULL OR [ProductId] = @productId)
    ORDER BY [Id];
END;