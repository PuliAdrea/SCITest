-- =============================================
-- SCI Technical Test
-- Script: Database Creation and Initial Setup
-- Author: Yesica Andrea Pulido Escobar
-- Date: 2025-10-17
-- =============================================

USE SCIProductsDB;
GO

CREATE PROCEDURE sp_AddProduct
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Products (Name, Description, Price)
    VALUES (@Name, @Description, @Price);

    SELECT * 
    FROM Products 
    WHERE Id = SCOPE_IDENTITY();
END;
GO


CREATE OR ALTER PROCEDURE sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        Price,
        CreatedDate,
        IsActive
    FROM Products
    WHERE IsActive = 1
    ORDER BY CreatedDate DESC;
END
GO


CREATE OR ALTER PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        Price,
        CreatedDate,
        IsActive
    FROM Products
    WHERE Id = @ProductId;
END
GO


CREATE OR ALTER PROCEDURE sp_UpdateProduct
    @ProductId INT,
    @Name NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Products
    SET 
        Name = @Name,
        Description = @Description,
        Price = @Price
    WHERE Id = @ProductId;
    
    SELECT 
        Id,
        Name,
        Description,
        Price,
        CreatedDate,
        IsActive
    FROM Products
    WHERE Id = @ProductId;
END
GO


CREATE OR ALTER PROCEDURE sp_DeleteProduct
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Products
    SET IsActive = 0
    WHERE Id = @ProductId;
    
    SELECT 
        Id,
        Name,
        IsActive
    FROM Products
    WHERE Id = @ProductId;
END
GO
