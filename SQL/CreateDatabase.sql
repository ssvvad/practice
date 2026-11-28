-- создание базы данных интернет-магазина
CREATE DATABASE ComputerStore;
GO
USE ComputerStore;
GO

-- создание таблицы категорий
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

-- создание таблицы товаров и связи с категориями
CREATE TABLE Products (
    ProductId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryId INT NOT NULL REFERENCES Categories(CategoryId),
    Name NVARCHAR(150) NOT NULL,
    Price DECIMAL(12,2) NOT NULL CHECK (Price >= 0),
    StockQuantity INT NOT NULL CHECK (StockQuantity >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- создание таблицы покупателей
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    RegisteredAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- создание таблицы заказов и связи с покупателями
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL REFERENCES Customers(CustomerId),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsPaid BIT NOT NULL DEFAULT 0
);

-- создание промежуточной таблицы для связи многие ко многим
CREATE TABLE OrderItems (
    OrderId INT NOT NULL REFERENCES Orders(OrderId),
    ProductId UNIQUEIDENTIFIER NOT NULL REFERENCES Products(ProductId),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(12,2) NOT NULL CHECK (UnitPrice >= 0),
    PRIMARY KEY (OrderId, ProductId)
);

-- создание уникального индекса для электронной почты покупателей
CREATE UNIQUE NONCLUSTERED INDEX UX_Customers_Email ON Customers(Email);
GO
