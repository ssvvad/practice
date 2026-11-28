
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Модель категории товаров.
/// </summary>
public sealed class Category
{
    /// <summary>
    /// Идентификатор категории.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Название категории.
    /// </summary>
    public string Name { get; set; } = "";
}

/// <summary>
/// Модель поставщика товаров.
/// </summary>
public sealed class Supplier
{
    /// <summary>
    /// Идентификатор поставщика.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Название поставщика.
    /// </summary>
    public string Name { get; set; } = "";
}

/// <summary>
/// Модель товара с ценой, остатком и признаком активности.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Название товара.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Идентификатор связанной категории.
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Необязательный идентификатор поставщика.
    /// </summary>
    public int? SupplierId { get; set; }
    /// <summary>
    /// Цена единицы товара.
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Количество единиц на складе.
    /// </summary>
    public int StockQuantity { get; set; }
    /// <summary>
    /// Признак активности товара.
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Модель заказа с датой создания и списком позиций.
/// </summary>
public sealed class Order
{
    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Дата создания заказа.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Список товарных позиций заказа.
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();
}

/// <summary>
/// Модель позиции заказа с количеством и фактической ценой продажи.
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// Идентификатор проданного товара.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Количество единиц товара в позиции.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Цена единицы на момент заказа.
    /// </summary>
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Строка каталога, дополненная названиями категории и поставщика, налогом и статусом остатка.
/// </summary>
public sealed class ProductCatalogItem
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Название товара.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Название категории или заменяющее значение.
    /// </summary>
    public string CategoryName { get; set; } = "";
    /// <summary>
    /// Название поставщика или заменяющее значение.
    /// </summary>
    public string SupplierName { get; set; } = "";
    /// <summary>
    /// Цена без добавленного налога.
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// Цена с налогом 20%, округлённая до двух знаков.
    /// </summary>
    public decimal PriceWithTax { get; set; }
    /// <summary>
    /// Остаток товара на складе.
    /// </summary>
    public int StockQuantity { get; set; }
    /// <summary>
    /// Текстовый статус наличия.
    /// </summary>
    public string StockStatus { get; set; } = "";
}

/// <summary>
/// Агрегированные показатели запасов по одной категории.
/// </summary>
public sealed class CategoryStockReport
{
    /// <summary>
    /// Название категории.
    /// </summary>
    public string CategoryName { get; set; } = "";
    /// <summary>
    /// Количество товарных позиций в категории.
    /// </summary>
    public int TotalProducts { get; set; }
    /// <summary>
    /// Количество активных товаров.
    /// </summary>
    public int ActiveProducts { get; set; }
    /// <summary>
    /// Количество товаров с положительным остатком.
    /// </summary>
    public int InStockProducts { get; set; }
    /// <summary>
    /// Суммарное количество единиц на складе.
    /// </summary>
    public int TotalStockQuantity { get; set; }
    /// <summary>
    /// Суммарная стоимость запасов по ценам товаров.
    /// </summary>
    public decimal TotalStockValue { get; set; }
    /// <summary>
    /// Средняя цена или null для пустой категории.
    /// </summary>
    public decimal? AveragePrice { get; set; }
    /// <summary>
    /// Минимальная цена или null для пустой категории.
    /// </summary>
    public decimal? MinPrice { get; set; }
    /// <summary>
    /// Максимальная цена или null для пустой категории.
    /// </summary>
    public decimal? MaxPrice { get; set; }
}

/// <summary>
/// Агрегированная статистика продаж отдельного товара.
/// </summary>
public sealed class ProductSalesReport
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Название товара.
    /// </summary>
    public string ProductName { get; set; } = "";
    /// <summary>
    /// Суммарное проданное количество.
    /// </summary>
    public int TotalQuantity { get; set; }
    /// <summary>
    /// Число различных заказов с этим товаром.
    /// </summary>
    public int OrdersCount { get; set; }
    /// <summary>
    /// Суммарная выручка по товару.
    /// </summary>
    public decimal Revenue { get; set; }
}

/// <summary>
/// Сводный аналитический отчёт магазина.
/// </summary>
public sealed class StoreAnalyticsReport
{
    /// <summary>
    /// Строки выбранной страницы каталога.
    /// </summary>
    public List<ProductCatalogItem> Catalog { get; set; } = new();
    /// <summary>
    /// Отчёты по всем категориям.
    /// </summary>
    public List<CategoryStockReport> CategoryReports { get; set; } = new();
    /// <summary>
    /// До пяти лидеров продаж по выручке.
    /// </summary>
    public List<ProductSalesReport> TopSales { get; set; } = new();

    /// <summary>
    /// Общее количество товарных позиций.
    /// </summary>
    public int TotalProducts { get; set; }
    /// <summary>
    /// Количество активных товаров.
    /// </summary>
    public int ActiveProducts { get; set; }
    /// <summary>
    /// Количество товаров с нулевым остатком.
    /// </summary>
    public int OutOfStockProducts { get; set; }
    /// <summary>
    /// Общая стоимость запасов.
    /// </summary>
    public decimal TotalStockValue { get; set; }
    /// <summary>
    /// Средняя цена активного товара или null.
    /// </summary>
    public decimal? AverageActiveProductPrice { get; set; }

    /// <summary>
    /// Признак наличия хотя бы одного товара с нулевым остатком.
    /// </summary>
    public bool HasOutOfStockProducts { get; set; }
    /// <summary>
    /// Признак того, что все цены неотрицательны.
    /// </summary>
    public bool AllPricesNonNegative { get; set; }

    /// <summary>
    /// Первый товар дороже 100000 рублей или null.
    /// </summary>
    public Product? ExpensiveProduct { get; set; }
    /// <summary>
    /// Категория с идентификатором 1, найденная через Single.
    /// </summary>
    public Category? SelectedCategory { get; set; }
}

/// <summary>
/// Контейнер исходных коллекций для построения отчёта.
/// </summary>
public sealed class StoreData
{
    /// <summary>
    /// Справочник категорий.
    /// </summary>
    public List<Category> Categories { get; set; } = new();
    /// <summary>
    /// Справочник поставщиков.
    /// </summary>
    public List<Supplier> Suppliers { get; set; } = new();
    /// <summary>
    /// Коллекция товаров.
    /// </summary>
    public List<Product> Products { get; set; } = new();
    /// <summary>
    /// Коллекция заказов.
    /// </summary>
    public List<Order> Orders { get; set; } = new();
}

/// <summary>
/// Фабрика демонстрационного набора данных.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Создаёт категории, поставщиков, товары и заказы для проверки LINQ-запросов.
    /// </summary>
    /// <returns>Заполненный объект StoreData.</returns>
    public static StoreData Create()
    {
        return new StoreData
        {
            // Категория «Принтеры» не содержит товаров: проверка пустой группы.
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Компьютеры" },
                new Category { Id = 2, Name = "Аксессуары" },
                new Category { Id = 3, Name = "Мониторы" },
                new Category { Id = 4, Name = "Принтеры" }
            },

            // Справочник поставщиков для последующего объединения.
            Suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "TechStore" },
                new Supplier { Id = 2, Name = "DigitalMarket" }
            },

            // Набор включает активные и неактивные товары, нулевой остаток, отсутствующего поставщика и неизвестную категорию.
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

            // Заказы содержат в том числе повторяющиеся позиции одного товара для проверки подсчёта уникальных заказов.
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

/// <summary>
/// Сервис аналитики магазина: фильтрация, объединение, группировка и агрегация коллекций с помощью LINQ.
/// </summary>
public sealed class StoreAnalyticsService
{
    /// <summary>
    /// Проверяет входные данные и формирует сводный отчёт, каталог, статистику запасов и продаж.
    /// </summary>
    /// <param name="products">Исходная последовательность товаров.</param>
    /// <param name="categories">Справочник категорий.</param>
    /// <param name="suppliers">Справочник поставщиков.</param>
    /// <param name="orders">Последовательность заказов.</param>
    /// <param name="minPrice">Необязательная нижняя граница цены включительно.</param>
    /// <param name="maxPrice">Необязательная верхняя граница цены включительно.</param>
    /// <param name="categoryId">Необязательный идентификатор категории.</param>
    /// <param name="searchText">Необязательная подстрока для поиска по названию.</param>
    /// <param name="pageNumber">Номер страницы каталога начиная с 1.</param>
    /// <param name="pageSize">Максимальное количество элементов страницы.</param>
    /// <returns>Сводный аналитический отчёт.</returns>
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
        // Проверяем обязательные последовательности на null.
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(suppliers);
        ArgumentNullException.ThrowIfNull(orders);

        // Номер страницы должен быть положительным.
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        // Размер страницы должен быть положительным.
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        // Проверяем согласованность границ цены, если заданы обе.
        if (minPrice.HasValue && maxPrice.HasValue &&
            minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException(
                "Минимальная цена больше максимальной.");
        }

        return new StoreAnalyticsReport
        {
            // Фильтры и пагинация применяются к каталогу, но не к общим показателям отчёта.
            Catalog = BuildCatalog(
                products, categories, suppliers,
                minPrice, maxPrice, categoryId,
                searchText, pageNumber, pageSize),

            // Формируем статистику всех категорий.
            CategoryReports = BuildCategoryReport(
                products, categories),

            // Строим рейтинг по фактическим продажам.
            TopSales = BuildSalesReport(products, orders),

            // Count без предиката считает все элементы.
            TotalProducts = products.Count(),

            // Count с предикатом считает только активные товары.
            ActiveProducts = products.Count(
                product => product.IsActive),

            // Считаем товары с нулевым остатком.
            OutOfStockProducts = products.Count(
                product => product.StockQuantity == 0),

            // Sum суммирует стоимость всех складских остатков.
            TotalStockValue = products.Sum(
                product => product.Price * product.StockQuantity),

            // Where фильтрует активные товары; nullable decimal позволяет Average вернуть null для пустой выборки.
            AverageActiveProductPrice = products
                .Where(product => product.IsActive)
                .Average(product => (decimal?)product.Price),

            // Any проверяет существование хотя бы одного подходящего элемента.
            HasOutOfStockProducts = products.Any(
                product => product.StockQuantity == 0),

            // All проверяет выполнение условия для каждого элемента (для пустой последовательности возвращает true).
            AllPricesNonNegative = products.All(
                product => product.Price >= 0),

            // FirstOrDefault возвращает первый подходящий товар или null.
            ExpensiveProduct = products.FirstOrDefault(
                product => product.Price > 100_000m),

            // Single требует ровно одну категорию с ID 1; отсутствие или дубликаты вызовут исключение.
            SelectedCategory = categories.Single(
                category => category.Id == 1)
        };
    }

    /// <summary>
    /// Формирует страницу каталога активных товаров в наличии с фильтрами и данными справочников.
    /// </summary>
    /// <param name="products">Коллекция товаров.</param>
    /// <param name="categories">Коллекция категорий.</param>
    /// <param name="suppliers">Коллекция поставщиков.</param>
    /// <param name="minPrice">Нижняя граница цены или null.</param>
    /// <param name="maxPrice">Верхняя граница цены или null.</param>
    /// <param name="categoryId">Идентификатор категории или null.</param>
    /// <param name="searchText">Подстрока названия товара или null.</param>
    /// <param name="pageNumber">Номер страницы начиная с 1.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Отсортированный список строк выбранной страницы каталога.</returns>
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


        // AsQueryable позволяет постепенно формировать запрос без немедленного получения результата.
        IQueryable<Product> query = products.AsQueryable();

        // В каталог включаем только активные товары.
        query = query.Where(product => product.IsActive);

        // Добавляем нижнюю границу, если она указана.
        if (minPrice.HasValue)
            query = query.Where(
                product => product.Price >= minPrice.Value);

        // Добавляем верхнюю границу, если она указана.
        if (maxPrice.HasValue)
            query = query.Where(
                product => product.Price <= maxPrice.Value);

        // При заданной категории ограничиваем выборку её идентификатором.
        if (categoryId.HasValue)
            query = query.Where(
                product => product.CategoryId == categoryId.Value);

        // Ищем подстроку названия без учёта регистра.
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(
                product => product.Name.Contains(
                    searchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Исключаем товары без положительного остатка.
        query = query.Where(
            product => product.StockQuantity > 0);

        // AsEnumerable переключает дальнейшие операции
        // на LINQ to Objects без материализации запроса.

        // Запрос в синтаксисе LINQ query syntax объединяет товары со справочниками.
        var catalog =
            from product in query.AsEnumerable()

            // Групповое соединение по идентификатору категории.
            join category in categories
                on product.CategoryId equals category.Id
                into categoryGroup

            // DefaultIfEmpty превращает соединение в левое: товар остаётся, даже если категории нет.
            from category in categoryGroup.DefaultIfEmpty()

            // Соединяем с поставщиками; приводим ID к nullable int для совпадения типов ключей.
            join supplier in suppliers
                on product.SupplierId equals (int?)supplier.Id
                into supplierGroup

            // Левое соединение сохраняет товары без поставщика.
            from supplier in supplierGroup.DefaultIfEmpty()

            // Проекция select создаёт объект строки каталога из объединённых данных.
            select new ProductCatalogItem
            {
                Id = product.Id,
                Name = product.Name,

                // Оператор ?? подставляет название при отсутствии связанной записи.
                CategoryName = category?.Name
                    ?? "Неизвестная категория",

                SupplierName = supplier?.Name
                    ?? "Не указан",

                Price = product.Price,

                // Добавляем 20% налога и округляем до двух знаков, середины — от нуля.
                PriceWithTax = decimal.Round(
                    product.Price * 1.2m,
                    2,
                    MidpointRounding.AwayFromZero),

                StockQuantity = product.StockQuantity,

                // Вложенный тернарный оператор задаёт статус остатка. Нулевые остатки отсеяны фильтром выше.
                StockStatus = product.StockQuantity == 0
                    ? "Нет в наличии"
                    : product.StockQuantity <= 5
                        ? "Мало осталось"
                        : "В наличии"
            };


        // Сортируем по цене, затем названию и ID для стабильного порядка.
        return catalog
            .OrderBy(item => item.Price)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            // Skip пропускает предыдущие страницы, Take ограничивает размер, ToList выполняет запрос.
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Строит статистику запасов для каждой категории, включая категории без товаров.
    /// </summary>
    /// <param name="products">Коллекция товаров.</param>
    /// <param name="categories">Коллекция категорий.</param>
    /// <returns>Список отчётов, отсортированный по названию категории.</returns>
    private List<CategoryStockReport> BuildCategoryReport(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        // GroupJoin сохраняет каждую категорию, даже если соответствующих товаров нет.
        return categories
            .GroupJoin(
                products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) =>
                    new CategoryStockReport
                    {
                        CategoryName = category.Name,

                        // Count рассчитывает общее число товаров.
                        TotalProducts = categoryProducts.Count(),

                        // Отдельно считаем активные товары и товары в наличии.
                        ActiveProducts = categoryProducts.Count(
                            product => product.IsActive),

                        InStockProducts = categoryProducts.Count(
                            product => product.StockQuantity > 0),

                        // Суммируем физические остатки.
                        TotalStockQuantity = categoryProducts.Sum(
                            product => product.StockQuantity),

                        // Стоимость запасов равна сумме произведений цены на остаток.
                        TotalStockValue = categoryProducts.Sum(
                            product => product.Price *
                                       product.StockQuantity),

                        // Nullable decimal позволяет агрегатам вернуть null при отсутствии товаров.
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
            // Сортируем отчёт по названию и материализуем список.
            .OrderBy(report => report.CategoryName)
            .ToList();
    }

    /// <summary>
    /// Рассчитывает продажи по товарам и выбирает пять лидеров по выручке.
    /// </summary>
    /// <param name="products">Справочник товаров.</param>
    /// <param name="orders">Коллекция заказов с позициями.</param>
    /// <returns>До пяти строк отчёта о продажах.</returns>
    private List<ProductSalesReport> BuildSalesReport(
        IEnumerable<Product> products,
        IEnumerable<Order> orders)
    {


        // SelectMany разворачивает вложенные списки позиций заказов в плоскую последовательность.
        var orderLines = orders.SelectMany(
            order => order.Items,
            (order, item) => new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                ProductId = item.ProductId,
                item.Quantity,
                item.UnitPrice,
                // Вычисляем выручку строки заказа по фактической цене продажи.
                LineTotal = item.Quantity * item.UnitPrice
            });


        // Join связывает строки заказов с товарами; позиции без соответствующего товара исключаются.
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
            // Группируем по ID и названию, чтобы одноимённые товары с разными ID не смешивались.
            .GroupBy(item => new
            {
                item.ProductId,
                item.ProductName
            })
            // Преобразуем каждую группу в строку отчёта о продажах.
            .Select(group => new ProductSalesReport
            {
                ProductId = group.Key.ProductId,
                ProductName = group.Key.ProductName,

                // Суммируем проданные единицы.
                TotalQuantity = group.Sum(
                    item => item.Quantity),


                // Distinct удаляет повторяющиеся ID заказов перед подсчётом.
                OrdersCount = group
                    .Select(item => item.OrderId)
                    .Distinct()
                    .Count(),

                // Суммируем выручку всех позиций данного товара.
                Revenue = group.Sum(item => item.LineTotal)
            })
            // Сортируем по убыванию выручки и количества; при равенстве — по названию и ID.
            .OrderByDescending(item => item.Revenue)
            .ThenByDescending(item => item.TotalQuantity)
            .ThenBy(item => item.ProductName)
            .ThenBy(item => item.ProductId)
            // Выбираем пять лидеров и выполняем запрос.
            .Take(5)
            .ToList();
    }

    /// <summary>
    /// Выводит группы товаров по категории и ценовым интервалам шириной 500 рублей.
    /// </summary>
    /// <param name="products">Коллекция товаров для группировки.</param>
    /// <param name="categories">Справочник названий категорий.</param>
    public void PrintPriceGroups(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        // Группируем по составному ключу: категория и нижняя граница интервала.
        var groups = products
            .GroupBy(product => new
            {
                product.CategoryId,

                // Floor округляет цену вниз до ближайшего кратного 500.
                LowerBound = decimal.Floor(
                    product.Price / 500m) * 500m
            })
            // Сортируем группы по категории и нижней границе цены.
            .OrderBy(group => group.Key.CategoryId)
            .ThenBy(group => group.Key.LowerBound);

        Console.WriteLine(
            "\n=== ГРУППИРОВКА ПО КАТЕГОРИИ И ЦЕНЕ ===");

        // Перебираем полученные группы.
        foreach (var group in groups)
        {
            // Ищем название категории; если она отсутствует, используем текст по умолчанию.
            string categoryName = categories
                .FirstOrDefault(
                    category => category.Id == group.Key.CategoryId)
                ?.Name ?? "Неизвестная категория";

            // Верхняя граница интервала не включается в него.
            decimal upperBound = group.Key.LowerBound + 500m;

            Console.WriteLine(
                $"{categoryName} | " +
                $"[{group.Key.LowerBound}; {upperBound}) руб. | " +
                $"Товаров: {group.Count()}");
        }
    }
}

/// <summary>
/// Консольная демонстрация построения отчётов и отложенного выполнения LINQ.
/// </summary>
public static class Program
{
    /// <summary>
    /// Точка входа: создаёт данные, формирует и печатает отчёт, демонстрирует группировку и отложенные запросы.
    /// </summary>
    public static void Main()
    {
        // Создаём демонстрационные данные.
        StoreData data = TestDataFactory.Create();

        StoreAnalyticsService service = new();

        // Строим отчёт без дополнительных фильтров, первая страница содержит до 10 строк.
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

        // Выводим сводный отчёт.
        PrintReport(report);

        // Дополнительно печатаем группировку по ценовым интервалам.
        service.PrintPriceGroups(
            data.Products, data.Categories);

        // Показываем поведение отложенного запроса после изменения исходного списка.
        DemonstrateDeferredExecution(data.Products);
    }

    /// <summary>
    /// Печатает разделы аналитического отчёта в консоль.
    /// </summary>
    /// <param name="report">Готовый сводный отчёт магазина.</param>
    private static void PrintReport(StoreAnalyticsReport report)
    {
        Console.WriteLine("=== КАТАЛОГ ТОВАРОВ ===");

        // Выводим каждую строку каталога.
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

        // Выводим агрегаты по каждой категории.
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

        // Выводим рейтинг продаж.
        foreach (var item in report.TopSales)
        {
            Console.WriteLine(
                $"{item.ProductName} | " +
                $"Продано: {item.TotalQuantity} | " +
                $"Заказов: {item.OrdersCount} | " +
                $"Выручка: {item.Revenue:F2} руб.");
        }

        // Печатаем показатели по всем товарам.
        Console.WriteLine("\n=== ОБЩИЕ ПОКАЗАТЕЛИ ===");

        Console.WriteLine($"Всего товаров: {report.TotalProducts}");
        Console.WriteLine($"Активных товаров: {report.ActiveProducts}");
        Console.WriteLine($"Товаров без остатка: {report.OutOfStockProducts}");
        Console.WriteLine($"Стоимость запасов: {report.TotalStockValue:F2}");

        Console.WriteLine(
            $"Средняя цена активных товаров: " +
            $"{FormatPrice(report.AverageActiveProductPrice)}");

        // Демонстрируем результаты операторов Any, All, FirstOrDefault и Single.
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

    /// <summary>
    /// Форматирует необязательную цену для вывода.
    /// </summary>
    /// <param name="price">Цена или null при отсутствии данных.</param>
    /// <returns>Цена с двумя десятичными знаками либо строка об отсутствии данных.</returns>
    private static string FormatPrice(decimal? price)
    {
        // Для null используем текст «Нет данных», иначе форматируем цену.
        return price.HasValue
            ? $"{price.Value:F2} руб."
            : "Нет данных";
    }

    /// <summary>
    /// Показывает отличие отложенного запроса Where от списка, материализованного через ToList.
    /// </summary>
    /// <param name="products">Изменяемый список товаров, в который добавляется демонстрационный товар.</param>
    private static void DemonstrateDeferredExecution(
        List<Product> products)
    {
        Console.WriteLine("\n=== ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ ===");

        // Where создаёт отложенный запрос, который будет выполнен при перечислении.
        var deferredQuery = products
            .Where(product => product.IsActive);


        // ToList выполняет запрос сейчас и сохраняет состав результата в отдельном списке.
        var snapshot = deferredQuery.ToList();

        // Первый Count выполняет отложенный запрос до изменения данных.
        Console.WriteLine(
            $"До добавления товара: {deferredQuery.Count()}");

        // Добавляем активный товар в исходную коллекцию.
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

        // Повторное выполнение запроса учитывает новый товар.
        Console.WriteLine(
            $"Отложенный запрос: {deferredQuery.Count()}");

        // Ранее созданный список сохраняет прежний состав элементов.
        Console.WriteLine(
            $"Материализованный снимок: {snapshot.Count}");
    }
}
