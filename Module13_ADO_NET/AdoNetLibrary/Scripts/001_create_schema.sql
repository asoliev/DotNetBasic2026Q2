IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Orders;
END;

IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Products;
END;

CREATE TABLE dbo.Products
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Weight DECIMAL(18,2) NOT NULL CHECK (Weight >= 0),
    Height DECIMAL(18,2) NOT NULL CHECK (Height >= 0),
    Width DECIMAL(18,2) NOT NULL CHECK (Width >= 0),
    Length DECIMAL(18,2) NOT NULL CHECK (Length >= 0)
);

CREATE TABLE dbo.Orders
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_CreatedDate DEFAULT SYSUTCDATETIME(),
    UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_UpdatedDate DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products (Id),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('NotStarted', 'Loading', 'InProgress', 'Arrived', 'Unloading', 'Cancelled', 'Done'))
);
