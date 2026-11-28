-- выбор базы данных импортированной из sql-дампа
USE ComputerStore_Imported;
GO

-- проверка содержимого восстановленных таблиц
SELECT * FROM Categories;
SELECT * FROM Products;
SELECT * FROM Customers;
SELECT * FROM Orders;
SELECT * FROM OrderItems;

-- проверка наличия уникального индекса
SELECT name, is_unique
FROM sys.indexes
WHERE object_id = OBJECT_ID('dbo.Customers');