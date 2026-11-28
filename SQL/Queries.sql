-- выбор базы данных
USE ComputerStore;
GO

-- выборка активных товаров в наличии
-- фильтрация по цене и сортировка по возрастанию
SELECT Name, Price, StockQuantity
FROM Products
WHERE IsActive = 1
    AND StockQuantity > 0
    AND Price BETWEEN 3000 AND 90000
ORDER BY Price ASC, Name ASC, ProductId ASC;


-- изменение цены ноутбука на 10 процентов
UPDATE Products
SET Price = Price * 1.10
WHERE Name = N'Ноутбук ASUS';

-- проверка изменения цены
SELECT Name, Price
FROM Products
WHERE Name = N'Ноутбук ASUS';


-- группировка товаров по категориям
-- подсчет количества товаров, средней цены и стоимости запасов
SELECT
    c.Name AS CategoryName,
    COUNT(p.ProductId) AS ProductCount,
    AVG(p.Price) AS AveragePrice,
    COALESCE(SUM(p.Price * p.StockQuantity), 0)
        AS TotalStockValue
FROM Categories c
LEFT JOIN Products p
    ON c.CategoryId = p.CategoryId
GROUP BY c.CategoryId, c.Name
ORDER BY ProductCount DESC, CategoryName;


-- выборка категорий с количеством товаров не менее двух
SELECT
    c.Name AS CategoryName,
    COUNT(p.ProductId) AS ProductCount
FROM Categories c
INNER JOIN Products p
    ON c.CategoryId = p.CategoryId
GROUP BY c.CategoryId, c.Name
HAVING COUNT(p.ProductId) >= 2;


-- левое соединение категорий и товаров
-- выводятся все категории даже при отсутствии товаров
SELECT
    c.Name AS CategoryName,
    p.Name AS ProductName,
    p.Price
FROM Categories c
LEFT JOIN Products p
    ON c.CategoryId = p.CategoryId;


-- правое соединение заказов и покупателей
-- выводятся все покупатели даже при отсутствии заказов
SELECT
    c.FullName,
    c.Email,
    o.OrderId,
    o.CreatedAt
FROM Orders o
RIGHT JOIN Customers c
    ON o.CustomerId = c.CustomerId;


-- внутреннее соединение связанных таблиц
-- получение информации о заказах, покупателях и товарах
SELECT
    o.OrderId,
    c.FullName,
    p.Name AS ProductName,
    oi.Quantity,
    oi.UnitPrice,
    oi.Quantity * oi.UnitPrice AS TotalAmount,
    o.IsPaid
FROM Orders o
INNER JOIN Customers c
    ON o.CustomerId = c.CustomerId
INNER JOIN OrderItems oi
    ON o.OrderId = oi.OrderId
INNER JOIN Products p
    ON oi.ProductId = p.ProductId
ORDER BY o.OrderId, p.Name;


-- пересечение двух выборок
-- вывод активных товаров которые присутствуют в заказах
SELECT ProductId, Name
FROM Products
WHERE IsActive = 1

INTERSECT

SELECT p.ProductId, p.Name
FROM Products p
INNER JOIN OrderItems oi
    ON p.ProductId = oi.ProductId;


-- выборка через связь многие ко многим
-- подсчет количества продаж и выручки по оплаченным заказам
SELECT
    p.Name AS ProductName,
    SUM(oi.Quantity) AS SoldQuantity,
    COUNT(DISTINCT oi.OrderId) AS OrdersCount,
    SUM(oi.Quantity * oi.UnitPrice) AS Revenue
FROM Products p
INNER JOIN OrderItems oi
    ON p.ProductId = oi.ProductId
INNER JOIN Orders o
    ON oi.OrderId = o.OrderId
WHERE o.IsPaid = 1
GROUP BY p.ProductId, p.Name
ORDER BY Revenue DESC, ProductName;


-- удаление покупателя у которого нет заказов
-- проверка отсутствия заказов нужна для сохранения связей
DELETE FROM Customers
WHERE Email = 'alex@mail.ru'
    AND NOT EXISTS (
        SELECT 1
        FROM Orders
        WHERE Orders.CustomerId = Customers.CustomerId
    );

-- проверка удаления покупателя
SELECT * FROM Customers;


-- получение информации об индексах таблицы покупателей
-- is_unique показывает является ли индекс уникальным
SELECT name, is_unique, type_desc
FROM sys.indexes
WHERE object_id = OBJECT_ID('dbo.Customers');