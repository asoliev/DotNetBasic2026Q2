CREATE TABLE [dbo].[Products]
(
    [Id] INT IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_Products] PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Weight] DECIMAL(18, 2) NOT NULL CONSTRAINT [CK_Products_Weight] CHECK ([Weight] >= 0),
    [Height] DECIMAL(18, 2) NOT NULL CONSTRAINT [CK_Products_Height] CHECK ([Height] >= 0),
    [Width] DECIMAL(18, 2) NOT NULL CONSTRAINT [CK_Products_Width] CHECK ([Width] >= 0),
    [Length] DECIMAL(18, 2) NOT NULL CONSTRAINT [CK_Products_Length] CHECK ([Length] >= 0)
);