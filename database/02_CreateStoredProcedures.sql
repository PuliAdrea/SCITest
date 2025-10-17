-- =============================================
-- SCI Technical Test
-- Script: Database Creation and Initial Setup
-- Author: Yesica Andrea Pulido Escobar
-- Date: 2025-10-17
-- =============================================

USE SCIProductsDB;
GO

-- =============================================
-- SP: CREATE - Insert New Product
-- =============================================
CREATE OR ALTER PROCEDURE sp_CreateProduct
    @Name NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Price DECIMAL(18,2),
    @ProductId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Products (Name, Description, Price, CreatedDate, IsActive)
    VALUES (@Name, @Description, @Price, GETDATE(), 1);
    
    SET @ProductId = SCOPE_IDENTITY();
    
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


-- =============================================
-- SP: READ ALL - Get All Active Products
-- =============================================
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

-- =============================================
-- SP: READ BY ID - Get Product By ID
-- =============================================
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

-- =============================================
-- SP: UPDATE - Update Existing Product
-- =============================================
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

-- =============================================
-- SP: DELETE - Soft Delete Product
-- =============================================
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

-- =============================================
-- SP: SEARCH - Search Products by Name/Description
-- =============================================
CREATE OR ALTER PROCEDURE sp_SearchProducts
    @SearchTerm NVARCHAR(200)
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
        AND (
            Name LIKE '%' + @SearchTerm + '%' 
            OR Description LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY CreatedDate DESC;
END
GO
