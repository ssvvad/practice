-- выбор базы данных интернет-магазина
USE ComputerStore;
GO

-- добавление категорий товаров
INSERT INTO Categories (Name)
VALUES
    (N'Компьютеры'),
    (N'Периферия'),
    (N'Мониторы'),
    (N'Принтеры');

-- добавление товаров с указанием категории, цены и остатка
INSERT INTO Products
    (CategoryId, Name, Price, StockQuantity, IsActive)
VALUES
    (1, N'Ноутбук ASUS', 85000, 10, 1),
    (1, N'Ноутбук Lenovo', 65000, 5, 1),
    (2, N'Мышь Logitech', 3500, 25, 1),
    (2, N'Клавиатура HyperX', 7000, 15, 1),
    (3, N'Монитор Samsung', 25000, 8, 1),
    (3, N'Монитор LG', 22000, 0, 0);

-- добавление покупателей
INSERT INTO Customers (FullName, Email)
VALUES
    (N'Иван Иванов', 'ivan@mail.ru'),
    (N'Анна Петрова', 'anna@mail.ru'),
    (N'Мария Смирнова', 'maria@mail.ru'),
    (N'Алексей Соколов', 'alex@mail.ru');

-- создание оплаченных и неоплаченных заказов
INSERT INTO Orders (CustomerId, IsPaid)
VALUES
    (1, 1),
    (2, 1),
    (1, 0),
    (3, 1);

-- добавление ноутбука в первый заказ
INSERT INTO OrderItems
    (OrderId, ProductId, Quantity, UnitPrice)
SELECT 1, ProductId, 1, Price
FROM Products
WHERE Name = N'Ноутбук ASUS';

-- добавление двух мышей в первый заказ
INSERT INTO OrderItems
    (OrderId, ProductId, Quantity, UnitPrice)
SELECT 1, ProductId, 2, Price
FROM Products
WHERE Name = N'Мышь Logitech';

-- добавление монитора во второй заказ
INSERT INTO OrderItems
    (OrderId, ProductId, Quantity, UnitPrice)
SELECT 2, ProductId, 1, Price
FROM Products
WHERE Name = N'Монитор Samsung';

-- добавление клавиатуры в третий заказ
INSERT INTO OrderItems
    (OrderId, ProductId, Quantity, UnitPrice)
SELECT 3, ProductId, 1, Price
FROM Products
WHERE Name = N'Клавиатура HyperX';

-- добавление трех мышей в четвертый заказ
INSERT INTO OrderItems
    (OrderId, ProductId, Quantity, UnitPrice)
SELECT 4, ProductId, 3, Price
FROM Products
WHERE Name = N'Мышь Logitech';

-- проверка содержимого всех таблиц
SELECT * FROM Categories;
SELECT * FROM Products;
SELECT * FROM Customers;
SELECT * FROM Orders;
SELECT * FROM OrderItems;