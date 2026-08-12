CREATE TABLE [dbo].[Orders]
(
    [Id] INT IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_Orders] PRIMARY KEY,
    [ProductId] INT NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Orders_CreatedDate] DEFAULT SYSUTCDATETIME(),
    [UpdatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Orders_UpdatedDate] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [FK_Orders_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT [CK_Orders_Status] CHECK ([Status] IN ('NotStarted', 'Loading', 'InProgress', 'Arrived', 'Unloading', 'Cancelled', 'Done'))
);