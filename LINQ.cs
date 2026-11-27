
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public sealed class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}

public sealed class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public sealed class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public sealed class ProductCatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public string SupplierName { get; set; } = "";
    public decimal Price { get; set; }
    public decimal PriceWithTax { get; set; }
    public int StockQuantity { get; set; }
    public string StockStatus { get; set; } = "";
}

public sealed class CategoryStockReport
{
    public string CategoryName { get; set; } = "";
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int InStockProducts { get; set; }
    public int TotalStockQuantity { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal? AveragePrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public sealed class ProductSalesReport
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int TotalQuantity { get; set; }
    public int OrdersCount { get; set; }
    public decimal Revenue { get; set; }
}

public sealed class StoreAnalyticsReport
{
    public List<ProductCatalogItem> Catalog { get; set; } = new();
    public List<CategoryStockReport> CategoryReports { get; set; } = new();
    public List<ProductSalesReport> TopSales { get; set; } = new();

    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public decimal? AverageActiveProductPrice { get; set; }

    public bool HasOutOfStockProducts { get; set; }
    public bool AllPricesNonNegative { get; set; }

    public Product? ExpensiveProduct { get; set; }
    public Category? SelectedCategory { get; set; }
}

public sealed class StoreData
{
    public List<Category> Categories { get; set; } = new();
    public List<Supplier> Suppliers { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}

public static class TestDataFactory
{
    public static StoreData Create()
    {
        return new StoreData
        {
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Компьютеры" },
                new Category { Id = 2, Name = "Аксессуары" },
                new Category { Id = 3, Name = "Мониторы" },
                new Category { Id = 4, Name = "Принтеры" }
            },

            Suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "TechStore" },
                new Supplier { Id = 2, Name = "DigitalMarket" }
            },

            Products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Ноутбук",
                    CategoryId = 1,
                    SupplierId = 1,
                    Price = 120000m,
                    StockQuantity = 5,
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Компьютер",
                    CategoryId = 1,
                    SupplierId = 2,
                    Price = 80000m,
                    StockQuantity = 3,
                    IsActive = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Мышь",
                    CategoryId = 2,
                    SupplierId = 1,
                    Price = 1500m,
                    StockQuantity = 20,
                    IsActive = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Клавиатура",
                    CategoryId = 2,
                    SupplierId = null,
                    Price = 3500m,
                    StockQuantity = 10,
                    IsActive = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Монитор",
                    CategoryId = 3,
                    SupplierId = 2,
                    Price = 25000m,
                    StockQuantity = 0,
                    IsActive = true
                },
                new Product
                {
                    Id = 6,
                    Name = "Кабель",
                    CategoryId = 2,
                    SupplierId = 1,
                    Price = 300m,
                    StockQuantity = 15,
                    IsActive = false
                },
                new Product
                {
                    Id = 7,
                    Name = "Переходник",
                    CategoryId = 99,
                    SupplierId = null,
                    Price = 750m,
                    StockQuantity = 8,
                    IsActive = true
                },
                new Product
                {
                    Id = 8,
                    Name = "Мышь",
                    CategoryId = 2,
                    SupplierId = 2,
                    Price = 1500m,
                    StockQuantity = 4,
                    IsActive = true
                }
            },

            Orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    CreatedAt = new DateTime(2026, 11, 20),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 120000m
                        },
                        new OrderItem
                        {
                            ProductId = 3,
                            Quantity = 2,
                            UnitPrice = 1500m
                        },
                        new OrderItem
                        {
                            ProductId = 3,
                            Quantity = 1,
                            UnitPrice = 1500m
                        }
                    }
                },
                new Order
                {
                    Id = 2,
                    CreatedAt = new DateTime(2026, 11, 21),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = 2,
                            Quantity = 1,
                            UnitPrice = 80000m
                        },
                        new OrderItem
                        {
                            ProductId = 3,
                            Quantity = 3,
                            UnitPrice = 1500m
                        }
                    }
                },
                new Order
                {
                    Id = 3,
                    CreatedAt = new DateTime(2026, 11, 22),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = 4,
                            Quantity = 2,
                            UnitPrice = 3500m
                        }
                    }
                }
            }
        };
    }
}

public sealed class StoreAnalyticsService
{
    public StoreAnalyticsReport BuildReport(
        IEnumerable<Product> products,
        IEnumerable<Category> categories,
        IEnumerable<Supplier> suppliers,
        IEnumerable<Order> orders,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? categoryId = null,
        string? searchText = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(suppliers);
        ArgumentNullException.ThrowIfNull(orders);

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (minPrice.HasValue && maxPrice.HasValue &&
            minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException(
                "Минимальная цена больше максимальной.");
        }

        return new StoreAnalyticsReport
        {
            Catalog = BuildCatalog(
                products, categories, suppliers,
                minPrice, maxPrice, categoryId,
                searchText, pageNumber, pageSize),

            CategoryReports = BuildCategoryReport(
                products, categories),

            TopSales = BuildSalesReport(products, orders),

            TotalProducts = products.Count(),

            ActiveProducts = products.Count(
                product => product.IsActive),

            OutOfStockProducts = products.Count(
                product => product.StockQuantity == 0),

            TotalStockValue = products.Sum(
                product => product.Price * product.StockQuantity),

            AverageActiveProductPrice = products
                .Where(product => product.IsActive)
                .Average(product => (decimal?)product.Price),

            HasOutOfStockProducts = products.Any(
                product => product.StockQuantity == 0),

            AllPricesNonNegative = products.All(
                product => product.Price >= 0),

            ExpensiveProduct = products.FirstOrDefault(
                product => product.Price > 100_000m),

            SelectedCategory = categories.Single(
                category => category.Id == 1)
        };
    }

    private List<ProductCatalogItem> BuildCatalog(
        IEnumerable<Product> products,
        IEnumerable<Category> categories,
        IEnumerable<Supplier> suppliers,
        decimal? minPrice,
        decimal? maxPrice,
        int? categoryId,
        string? searchText,
        int pageNumber,
        int pageSize)
    {


        IQueryable<Product> query = products.AsQueryable();

        query = query.Where(product => product.IsActive);

        if (minPrice.HasValue)
            query = query.Where(
                product => product.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(
                product => product.Price <= maxPrice.Value);

        if (categoryId.HasValue)
            query = query.Where(
                product => product.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(
                product => product.Name.Contains(
                    searchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        query = query.Where(
            product => product.StockQuantity > 0);

        // AsEnumerable переключает дальнейшие операции
        // на LINQ to Objects без материализации запроса.

        var catalog =
            from product in query.AsEnumerable()

            join category in categories
                on product.CategoryId equals category.Id
                into categoryGroup

            from category in categoryGroup.DefaultIfEmpty()

            join supplier in suppliers
                on product.SupplierId equals (int?)supplier.Id
                into supplierGroup

            from supplier in supplierGroup.DefaultIfEmpty()

            select new ProductCatalogItem
            {
                Id = product.Id,
                Name = product.Name,

                CategoryName = category?.Name
                    ?? "Неизвестная категория",

                SupplierName = supplier?.Name
                    ?? "Не указан",

                Price = product.Price,

                PriceWithTax = decimal.Round(
                    product.Price * 1.2m,
                    2,
                    MidpointRounding.AwayFromZero),

                StockQuantity = product.StockQuantity,

                StockStatus = product.StockQuantity == 0
                    ? "Нет в наличии"
                    : product.StockQuantity <= 5
                        ? "Мало осталось"
                        : "В наличии"
            };


        return catalog
            .OrderBy(item => item.Price)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private List<CategoryStockReport> BuildCategoryReport(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        return categories
            .GroupJoin(
                products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) =>
                    new CategoryStockReport
                    {
                        CategoryName = category.Name,

                        TotalProducts = categoryProducts.Count(),

                        ActiveProducts = categoryProducts.Count(
                            product => product.IsActive),

                        InStockProducts = categoryProducts.Count(
                            product => product.StockQuantity > 0),

                        TotalStockQuantity = categoryProducts.Sum(
                            product => product.StockQuantity),

                        TotalStockValue = categoryProducts.Sum(
                            product => product.Price *
                                       product.StockQuantity),

                        AveragePrice = categoryProducts
                            .Select(product => (decimal?)product.Price)
                            .Average(),

                        MinPrice = categoryProducts
                            .Select(product => (decimal?)product.Price)
                            .Min(),

                        MaxPrice = categoryProducts
                            .Select(product => (decimal?)product.Price)
                            .Max()
                    })
            .OrderBy(report => report.CategoryName)
            .ToList();
    }

    private List<ProductSalesReport> BuildSalesReport(
        IEnumerable<Product> products,
        IEnumerable<Order> orders)
    {


        var orderLines = orders.SelectMany(
            order => order.Items,
            (order, item) => new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                ProductId = item.ProductId,
                item.Quantity,
                item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            });


        return orderLines
            .Join(
                products,
                line => line.ProductId,
                product => product.Id,
                (line, product) => new
                {
                    line.OrderId,
                    line.OrderDate,
                    line.ProductId,
                    ProductName = product.Name,
                    line.Quantity,
                    line.UnitPrice,
                    line.LineTotal
                })
            .GroupBy(item => new
            {
                item.ProductId,
                item.ProductName
            })
            .Select(group => new ProductSalesReport
            {
                ProductId = group.Key.ProductId,
                ProductName = group.Key.ProductName,

                TotalQuantity = group.Sum(
                    item => item.Quantity),


                OrdersCount = group
                    .Select(item => item.OrderId)
                    .Distinct()
                    .Count(),

                Revenue = group.Sum(item => item.LineTotal)
            })
            .OrderByDescending(item => item.Revenue)
            .ThenByDescending(item => item.TotalQuantity)
            .ThenBy(item => item.ProductName)
            .ThenBy(item => item.ProductId)
            .Take(5)
            .ToList();
    }

    public void PrintPriceGroups(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        var groups = products
            .GroupBy(product => new
            {
                product.CategoryId,

                LowerBound = decimal.Floor(
                    product.Price / 500m) * 500m
            })
            .OrderBy(group => group.Key.CategoryId)
            .ThenBy(group => group.Key.LowerBound);

        Console.WriteLine(
            "\n=== ГРУППИРОВКА ПО КАТЕГОРИИ И ЦЕНЕ ===");

        foreach (var group in groups)
        {
            string categoryName = categories
                .FirstOrDefault(
                    category => category.Id == group.Key.CategoryId)
                ?.Name ?? "Неизвестная категория";

            decimal upperBound = group.Key.LowerBound + 500m;

            Console.WriteLine(
                $"{categoryName} | " +
                $"[{group.Key.LowerBound}; {upperBound}) руб. | " +
                $"Товаров: {group.Count()}");
        }
    }
}

public static class Program
{
    public static void Main()
    {
        StoreData data = TestDataFactory.Create();

        StoreAnalyticsService service = new();

        StoreAnalyticsReport report = service.BuildReport(
            data.Products,
            data.Categories,
            data.Suppliers,
            data.Orders,
            minPrice: null,
            maxPrice: null,
            categoryId: null,
            searchText: null,
            pageNumber: 1,
            pageSize: 10);

        PrintReport(report);

        service.PrintPriceGroups(
            data.Products, data.Categories);

        DemonstrateDeferredExecution(data.Products);
    }

    private static void PrintReport(StoreAnalyticsReport report)
    {
        Console.WriteLine("=== КАТАЛОГ ТОВАРОВ ===");

        foreach (var item in report.Catalog)
        {
            Console.WriteLine($"ID: {item.Id} | {item.Name}");
            Console.WriteLine($"Категория: {item.CategoryName}");
            Console.WriteLine($"Поставщик: {item.SupplierName}");
            Console.WriteLine($"Цена: {item.Price:F2} руб.");
            Console.WriteLine($"Цена с НДС: {item.PriceWithTax:F2} руб.");
            Console.WriteLine($"Остаток: {item.StockQuantity}");
            Console.WriteLine($"Статус: {item.StockStatus}\n");
        }

        Console.WriteLine("\n=== ОТЧЁТ ПО КАТЕГОРИЯМ ===");

        foreach (var item in report.CategoryReports)
        {
            Console.WriteLine($"\n{item.CategoryName}");
            Console.WriteLine($"Всего товаров: {item.TotalProducts}");
            Console.WriteLine($"Активных товаров: {item.ActiveProducts}");
            Console.WriteLine($"Товаров в наличии: {item.InStockProducts}");
            Console.WriteLine($"Суммарный остаток: {item.TotalStockQuantity}");
            Console.WriteLine($"Стоимость запасов: {item.TotalStockValue:F2}");
            Console.WriteLine($"Средняя цена: {FormatPrice(item.AveragePrice)}");
            Console.WriteLine($"Минимальная цена: {FormatPrice(item.MinPrice)}");
            Console.WriteLine($"Максимальная цена: {FormatPrice(item.MaxPrice)}");
        }

        Console.WriteLine("\n=== ТОП-5 ПРОДАЖ ===");

        foreach (var item in report.TopSales)
        {
            Console.WriteLine(
                $"{item.ProductName} | " +
                $"Продано: {item.TotalQuantity} | " +
                $"Заказов: {item.OrdersCount} | " +
                $"Выручка: {item.Revenue:F2} руб.");
        }

        Console.WriteLine("\n=== ОБЩИЕ ПОКАЗАТЕЛИ ===");

        Console.WriteLine($"Всего товаров: {report.TotalProducts}");
        Console.WriteLine($"Активных товаров: {report.ActiveProducts}");
        Console.WriteLine($"Товаров без остатка: {report.OutOfStockProducts}");
        Console.WriteLine($"Стоимость запасов: {report.TotalStockValue:F2}");

        Console.WriteLine(
            $"Средняя цена активных товаров: " +
            $"{FormatPrice(report.AverageActiveProductPrice)}");

        Console.WriteLine("\n=== ПРОВЕРКИ LINQ ===");

        Console.WriteLine(
            $"Any: есть товары без остатка — " +
            $"{report.HasOutOfStockProducts}");

        Console.WriteLine(
            $"All: все цены неотрицательны — " +
            $"{report.AllPricesNonNegative}");

        Console.WriteLine(
            $"FirstOrDefault: товар дороже 100000 руб. — " +
            $"{report.ExpensiveProduct?.Name ?? "Не найден"}");

        Console.WriteLine(
            $"Single: категория с ID = 1 — " +
            $"{report.SelectedCategory?.Name}");
    }

    private static string FormatPrice(decimal? price)
    {
        return price.HasValue
            ? $"{price.Value:F2} руб."
            : "Нет данных";
    }

    private static void DemonstrateDeferredExecution(
        List<Product> products)
    {
        Console.WriteLine("\n=== ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ ===");

        var deferredQuery = products
            .Where(product => product.IsActive);


        var snapshot = deferredQuery.ToList();

        Console.WriteLine(
            $"До добавления товара: {deferredQuery.Count()}");

        products.Add(new Product
        {
            Id = 9,
            Name = "Веб-камера",
            CategoryId = 2,
            SupplierId = 1,
            Price = 5000m,
            StockQuantity = 7,
            IsActive = true
        });

        Console.WriteLine(
            $"Отложенный запрос: {deferredQuery.Count()}");

        Console.WriteLine(
            $"Материализованный снимок: {snapshot.Count}");
    }
}
