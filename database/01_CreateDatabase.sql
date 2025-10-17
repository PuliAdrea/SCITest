-- =============================================
-- SCI Technical Test
-- Script: Database Creation and Initial Setup
-- Author: Yesica Andrea Pulido Escobar
-- Date: 2025-10-17
-- =============================================

USE master;
GO

-- Drop database if exists (for clean testing)
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'SCIProductsDB')
BEGIN
    ALTER DATABASE SCIProductsDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SCIProductsDB;
    PRINT 'Previous database dropped successfully';
END
GO

-- Create new database
CREATE DATABASE SCIProductsDB;
GO

PRINT 'Database SCIProductsDB created successfully';
GO

USE SCIProductsDB;
GO

-- =============================================
-- Table: Products
-- =============================================
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Products_Name UNIQUE (Name)
);
GO

PRINT 'Table Products created successfully';
GO


-- =============================================
-- Seed Data (Initial Products)
-- =============================================
INSERT INTO Products (Name, Description, Price, CreatedDate, IsActive)
VALUES 
    ('Diesel Fuel Sensor', 
     'High-precision fuel level sensor for diesel tanks with digital output', 
     299.99, 
     GETDATE(), 
     1),
    
    ('Fleet Tracking Device', 
     'GPS tracking device with real-time monitoring and geofencing capabilities', 
     449.50, 
     GETDATE(), 
     1),
    
    ('Fuel Management Software License', 
     'Annual license for comprehensive fuel management software platform', 
     1200.00, 
     GETDATE(), 
     1),
    
    ('Tank Level Monitor', 
     'Wireless tank level monitoring system with mobile alerts', 
     675.00, 
     GETDATE(), 
     1),
    
    ('Fuel Pump Controller', 
     'Electronic fuel pump controller with automatic shut-off', 
     325.75, 
     GETDATE(), 
     1);
GO

PRINT 'Seed data inserted successfully';
GO
