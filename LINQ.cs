
using System;
using System.Collections.Generic;
using System.Linq;

// класс для хранения данных категории товаров
public sealed class Category
{
    // идентификатор категории
    public int Id { get; set; }
    // название категории
    public string Name { get; set; } = "";
}

// класс для хранения данных поставщика товаров
public sealed class Supplier
{
    // идентификатор поставщика
    public int Id { get; set; }
    // название поставщика
    public string Name { get; set; } = "";
}

// класс для хранения данных товара с ценой, остатком и признаком активности
public sealed class Product
{
    // идентификатор товара
    public int Id { get; set; }
    // название товара
    public string Name { get; set; } = "";
    // идентификатор связанной категории
    public int CategoryId { get; set; }
    // необязательный идентификатор поставщика
    public int? SupplierId { get; set; }
    // цена единицы товара
    public decimal Price { get; set; }
    // количество единиц на складе
    public int StockQuantity { get; set; }
    // признак активности товара
    public bool IsActive { get; set; }
}

// класс для хранения данных заказа с датой создания и списком позиций
public sealed class Order
{
    // идентификатор заказа
    public int Id { get; set; }
    // дата создания заказа
    public DateTime CreatedAt { get; set; }
    // список товарных позиций заказа
    public List<OrderItem> Items { get; set; } = new();
}

// класс для хранения данных позиции заказа с количеством и фактической ценой продажи
public sealed class OrderItem
{
    // идентификатор проданного товара
    public int ProductId { get; set; }
    // количество единиц товара в позиции
    public int Quantity { get; set; }
    // цена единицы на момент заказа
    public decimal UnitPrice { get; set; }
}

// строка каталога, дополненная названиями категории и поставщика, налогом и статусом остатка
public sealed class ProductCatalogItem
{
    // идентификатор товара
    public int Id { get; set; }
    // название товара
    public string Name { get; set; } = "";
    // название категории или заменяющее значение
    public string CategoryName { get; set; } = "";
    // название поставщика или заменяющее значение
    public string SupplierName { get; set; } = "";
    // цена без добавленного налога
    public decimal Price { get; set; }
    // цена с налогом 20%, округлённая до двух знаков
    public decimal PriceWithTax { get; set; }
    // остаток товара на складе
    public int StockQuantity { get; set; }
    // текстовый статус наличия
    public string StockStatus { get; set; } = "";
}

// агрегированные показатели запасов по одной категории
public sealed class CategoryStockReport
{
    // название категории
    public string CategoryName { get; set; } = "";
    // количество товарных позиций в категории
    public int TotalProducts { get; set; }
    // количество активных товаров
    public int ActiveProducts { get; set; }
    // количество товаров с положительным остатком
    public int InStockProducts { get; set; }
    // суммарное количество единиц на складе
    public int TotalStockQuantity { get; set; }
    // суммарная стоимость запасов по ценам товаров
    public decimal TotalStockValue { get; set; }
    // средняя цена или null для пустой категории
    public decimal? AveragePrice { get; set; }
    // минимальная цена или null для пустой категории
    public decimal? MinPrice { get; set; }
    // максимальная цена или null для пустой категории
    public decimal? MaxPrice { get; set; }
}

// агрегированная статистика продаж отдельного товара
public sealed class ProductSalesReport
{
    // идентификатор товара
    public int ProductId { get; set; }
    // название товара
    public string ProductName { get; set; } = "";
    // суммарное проданное количество
    public int TotalQuantity { get; set; }
    // число различных заказов с этим товаром
    public int OrdersCount { get; set; }
    // суммарная выручка по товару
    public decimal Revenue { get; set; }
}

// сводный аналитический отчёт магазина
public sealed class StoreAnalyticsReport
{
    // строки выбранной страницы каталога
    public List<ProductCatalogItem> Catalog { get; set; } = new();
    // отчёты по всем категориям
    public List<CategoryStockReport> CategoryReports { get; set; } = new();
    // до пяти лидеров продаж по выручке
    public List<ProductSalesReport> TopSales { get; set; } = new();

    // общее количество товарных позиций
    public int TotalProducts { get; set; }
    // количество активных товаров
    public int ActiveProducts { get; set; }
    // количество товаров с нулевым остатком
    public int OutOfStockProducts { get; set; }
    // общая стоимость запасов
    public decimal TotalStockValue { get; set; }
    // средняя цена активного товара или null
    public decimal? AverageActiveProductPrice { get; set; }

    // признак наличия хотя бы одного товара с нулевым остатком
    public bool HasOutOfStockProducts { get; set; }
    // признак того, что все цены неотрицательны
    public bool AllPricesNonNegative { get; set; }

    // первый товар дороже 100000 рублей или null
    public Product? ExpensiveProduct { get; set; }
    // категория с идентификатором 1, найденная через Single
    public Category? SelectedCategory { get; set; }
}

// контейнер исходных коллекций для построения отчёта
public sealed class StoreData
{
    // справочник категорий
    public List<Category> Categories { get; set; } = new();
    // справочник поставщиков
    public List<Supplier> Suppliers { get; set; } = new();
    // коллекция товаров
    public List<Product> Products { get; set; } = new();
    // коллекция заказов
    public List<Order> Orders { get; set; } = new();
}

// фабрика демонстрационного набора данных
public static class TestDataFactory
{
    // создаёт категории, поставщиков, товары и заказы для проверки LINQ-запросов
    // возвращает: заполненный объект StoreData
    public static StoreData Create()
    {
        return new StoreData
        {
            // категория «Принтеры» не содержит товаров: проверка пустой группы
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Компьютеры" },
                new Category { Id = 2, Name = "Аксессуары" },
                new Category { Id = 3, Name = "Мониторы" },
                new Category { Id = 4, Name = "Принтеры" }
            },

            // справочник поставщиков для последующего объединения
            Suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "TechStore" },
                new Supplier { Id = 2, Name = "DigitalMarket" }
            },

            // набор включает активные и неактивные товары, нулевой остаток, отсутствующего поставщика и неизвестную категорию
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

            // заказы содержат в том числе повторяющиеся позиции одного товара для проверки подсчёта уникальных заказов
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

// сервис аналитики магазина: фильтрация, объединение, группировка и агрегация коллекций с помощью LINQ
public sealed class StoreAnalyticsService
{
    // проверяет входные данные и формирует сводный отчёт, каталог, статистику запасов и продаж
    // products — исходная последовательность товаров
    // categories — справочник категорий
    // suppliers — справочник поставщиков
    // orders — последовательность заказов
    // minPrice — необязательная нижняя граница цены включительно
    // maxPrice — необязательная верхняя граница цены включительно
    // categoryId — необязательный идентификатор категории
    // searchText — необязательная подстрока для поиска по названию
    // pageNumber — номер страницы каталога начиная с 1
    // pageSize — максимальное количество элементов страницы
    // возвращает: сводный аналитический отчёт
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
        // проверяем обязательные последовательности на null
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(suppliers);
        ArgumentNullException.ThrowIfNull(orders);

        // номер страницы должен быть положительным
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        // размер страницы должен быть положительным
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        // проверяем согласованность границ цены, если заданы обе
        if (minPrice.HasValue && maxPrice.HasValue &&
            minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException(
                "Минимальная цена больше максимальной.");
        }

        return new StoreAnalyticsReport
        {
            // фильтры и пагинация применяются к каталогу, но не к общим показателям отчёта
            Catalog = BuildCatalog(
                products, categories, suppliers,
                minPrice, maxPrice, categoryId,
                searchText, pageNumber, pageSize),

            // формируем статистику всех категорий
            CategoryReports = BuildCategoryReport(
                products, categories),

            // строим рейтинг по фактическим продажам
            TopSales = BuildSalesReport(products, orders),

            // здесь Count без предиката считает все элементы
            TotalProducts = products.Count(),

            // здесь Count с предикатом считает только активные товары
            ActiveProducts = products.Count(
                product => product.IsActive),

            // считаем товары с нулевым остатком
            OutOfStockProducts = products.Count(
                product => product.StockQuantity == 0),

            // здесь Sum суммирует стоимость всех складских остатков
            TotalStockValue = products.Sum(
                product => product.Price * product.StockQuantity),

            // здесь Where фильтрует активные товары; nullable decimal позволяет Average вернуть null для пустой выборки
            AverageActiveProductPrice = products
                .Where(product => product.IsActive)
                .Average(product => (decimal?)product.Price),

            // здесь Any проверяет существование хотя бы одного подходящего элемента
            HasOutOfStockProducts = products.Any(
                product => product.StockQuantity == 0),

            // здесь All проверяет выполнение условия для каждого элемента (для пустой последовательности возвращает true)
            AllPricesNonNegative = products.All(
                product => product.Price >= 0),

            // здесь FirstOrDefault возвращает первый подходящий товар или null
            ExpensiveProduct = products.FirstOrDefault(
                product => product.Price > 100_000m),

            // здесь Single требует ровно одну категорию с ID 1; отсутствие или дубликаты вызовут исключение
            SelectedCategory = categories.Single(
                category => category.Id == 1)
        };
    }

    // формирует страницу каталога активных товаров в наличии с фильтрами и данными справочников
    // products — коллекция товаров
    // categories — коллекция категорий
    // suppliers — коллекция поставщиков
    // minPrice — нижняя граница цены или null
    // maxPrice — верхняя граница цены или null
    // categoryId — идентификатор категории или null
    // searchText — подстрока названия товара или null
    // pageNumber — номер страницы начиная с 1
    // pageSize — размер страницы
    // возвращает: отсортированный список строк выбранной страницы каталога
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


        // здесь AsQueryable позволяет постепенно формировать запрос без немедленного получения результата
        IQueryable<Product> query = products.AsQueryable();

        // в каталог включаем только активные товары
        query = query.Where(product => product.IsActive);

        // добавляем нижнюю границу, если она указана
        if (minPrice.HasValue)
            query = query.Where(
                product => product.Price >= minPrice.Value);

        // добавляем верхнюю границу, если она указана
        if (maxPrice.HasValue)
            query = query.Where(
                product => product.Price <= maxPrice.Value);

        // при заданной категории ограничиваем выборку её идентификатором
        if (categoryId.HasValue)
            query = query.Where(
                product => product.CategoryId == categoryId.Value);

        // ищем подстроку названия без учёта регистра
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(
                product => product.Name.Contains(
                    searchText,
                    StringComparison.OrdinalIgnoreCase));
        }

        // исключаем товары без положительного остатка
        query = query.Where(
            product => product.StockQuantity > 0);

        // метод AsEnumerable переключает дальнейшие операции
        // на LINQ to Objects без материализации запроса

        // запрос в синтаксисе LINQ query syntax объединяет товары со справочниками
        var catalog =
            from product in query.AsEnumerable()

            // групповое соединение по идентификатору категории
            join category in categories
                on product.CategoryId equals category.Id
                into categoryGroup

            // здесь DefaultIfEmpty превращает соединение в левое: товар остаётся, даже если категории нет
            from category in categoryGroup.DefaultIfEmpty()

            // соединяем с поставщиками; приводим ID к nullable int для совпадения типов ключей
            join supplier in suppliers
                on product.SupplierId equals (int?)supplier.Id
                into supplierGroup

            // левое соединение сохраняет товары без поставщика
            from supplier in supplierGroup.DefaultIfEmpty()

            // проекция select создаёт объект строки каталога из объединённых данных
            select new ProductCatalogItem
            {
                Id = product.Id,
                Name = product.Name,

                // оператор ?? подставляет название при отсутствии связанной записи
                CategoryName = category?.Name
                    ?? "Неизвестная категория",

                SupplierName = supplier?.Name
                    ?? "Не указан",

                Price = product.Price,

                // добавляем 20% налога и округляем до двух знаков, середины — от нуля
                PriceWithTax = decimal.Round(
                    product.Price * 1.2m,
                    2,
                    MidpointRounding.AwayFromZero),

                StockQuantity = product.StockQuantity,

                // вложенный тернарный оператор задаёт статус остатка; нулевые остатки отсеяны фильтром выше
                StockStatus = product.StockQuantity == 0
                    ? "Нет в наличии"
                    : product.StockQuantity <= 5
                        ? "Мало осталось"
                        : "В наличии"
            };


        // сортируем по цене, затем названию и ID для стабильного порядка
        return catalog
            .OrderBy(item => item.Price)
            .ThenBy(item => item.Name)
            .ThenBy(item => item.Id)
            // здесь Skip пропускает предыдущие страницы, Take ограничивает размер, ToList выполняет запрос
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    // строит статистику запасов для каждой категории, включая категории без товаров
    // products — коллекция товаров
    // categories — коллекция категорий
    // возвращает: список отчётов, отсортированный по названию категории
    private List<CategoryStockReport> BuildCategoryReport(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        // здесь GroupJoin сохраняет каждую категорию, даже если соответствующих товаров нет
        return categories
            .GroupJoin(
                products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) =>
                    new CategoryStockReport
                    {
                        CategoryName = category.Name,

                        // здесь Count рассчитывает общее число товаров
                        TotalProducts = categoryProducts.Count(),

                        // отдельно считаем активные товары и товары в наличии
                        ActiveProducts = categoryProducts.Count(
                            product => product.IsActive),

                        InStockProducts = categoryProducts.Count(
                            product => product.StockQuantity > 0),

                        // суммируем физические остатки
                        TotalStockQuantity = categoryProducts.Sum(
                            product => product.StockQuantity),

                        // стоимость запасов равна сумме произведений цены на остаток
                        TotalStockValue = categoryProducts.Sum(
                            product => product.Price *
                                       product.StockQuantity),

                        // здесь Nullable decimal позволяет агрегатам вернуть null при отсутствии товаров
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
            // сортируем отчёт по названию и материализуем список
            .OrderBy(report => report.CategoryName)
            .ToList();
    }

    // рассчитывает продажи по товарам и выбирает пять лидеров по выручке
    // products — справочник товаров
    // orders — коллекция заказов с позициями
    // возвращает: до пяти строк отчёта о продажах
    private List<ProductSalesReport> BuildSalesReport(
        IEnumerable<Product> products,
        IEnumerable<Order> orders)
    {


        // здесь SelectMany разворачивает вложенные списки позиций заказов в плоскую последовательность
        var orderLines = orders.SelectMany(
            order => order.Items,
            (order, item) => new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                ProductId = item.ProductId,
                item.Quantity,
                item.UnitPrice,
                // вычисляем выручку строки заказа по фактической цене продажи
                LineTotal = item.Quantity * item.UnitPrice
            });


        // здесь Join связывает строки заказов с товарами; позиции без соответствующего товара исключаются
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
            // группируем по ID и названию, чтобы одноимённые товары с разными ID не смешивались
            .GroupBy(item => new
            {
                item.ProductId,
                item.ProductName
            })
            // преобразуем каждую группу в строку отчёта о продажах
            .Select(group => new ProductSalesReport
            {
                ProductId = group.Key.ProductId,
                ProductName = group.Key.ProductName,

                // суммируем проданные единицы
                TotalQuantity = group.Sum(
                    item => item.Quantity),


                // здесь Distinct удаляет повторяющиеся ID заказов перед подсчётом
                OrdersCount = group
                    .Select(item => item.OrderId)
                    .Distinct()
                    .Count(),

                // суммируем выручку всех позиций данного товара
                Revenue = group.Sum(item => item.LineTotal)
            })
            // сортируем по убыванию выручки и количества; при равенстве — по названию и ID
            .OrderByDescending(item => item.Revenue)
            .ThenByDescending(item => item.TotalQuantity)
            .ThenBy(item => item.ProductName)
            .ThenBy(item => item.ProductId)
            // выбираем пять лидеров и выполняем запрос
            .Take(5)
            .ToList();
    }

    // выводит группы товаров по категории и ценовым интервалам шириной 500 рублей
    // products — коллекция товаров для группировки
    // categories — справочник названий категорий
    public void PrintPriceGroups(
        IEnumerable<Product> products,
        IEnumerable<Category> categories)
    {
        // группируем по составному ключу: категория и нижняя граница интервала
        var groups = products
            .GroupBy(product => new
            {
                product.CategoryId,

                // здесь Floor округляет цену вниз до ближайшего кратного 500
                LowerBound = decimal.Floor(
                    product.Price / 500m) * 500m
            })
            // сортируем группы по категории и нижней границе цены
            .OrderBy(group => group.Key.CategoryId)
            .ThenBy(group => group.Key.LowerBound);

        Console.WriteLine(
            "\n=== ГРУППИРОВКА ПО КАТЕГОРИИ И ЦЕНЕ ===");

        // перебираем полученные группы
        foreach (var group in groups)
        {
            // ищем название категории; если она отсутствует, используем текст по умолчанию
            string categoryName = categories
                .FirstOrDefault(
                    category => category.Id == group.Key.CategoryId)
                ?.Name ?? "Неизвестная категория";

            // верхняя граница интервала не включается в него
            decimal upperBound = group.Key.LowerBound + 500m;

            Console.WriteLine(
                $"{categoryName} | " +
                $"[{group.Key.LowerBound}; {upperBound}) руб. | " +
                $"Товаров: {group.Count()}");
        }
    }
}

// консольная демонстрация построения отчётов и отложенного выполнения LINQ
public static class Program
{
    // метод Main: создаёт данные, формирует и печатает отчёт, демонстрирует группировку и отложенные запросы
    public static void Main()
    {
        // создаём демонстрационные данные
        StoreData data = TestDataFactory.Create();

        StoreAnalyticsService service = new();

        // строим отчёт без дополнительных фильтров, первая страница содержит до 10 строк
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

        // выводим сводный отчёт
        PrintReport(report);

        // дополнительно печатаем группировку по ценовым интервалам
        service.PrintPriceGroups(
            data.Products, data.Categories);

        // показываем поведение отложенного запроса после изменения исходного списка
        DemonstrateDeferredExecution(data.Products);
    }

    // печатает разделы аналитического отчёта в консоль
    // report — готовый сводный отчёт магазина
    private static void PrintReport(StoreAnalyticsReport report)
    {
        Console.WriteLine("=== КАТАЛОГ ТОВАРОВ ===");

        // выводим каждую строку каталога
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

        // выводим агрегаты по каждой категории
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

        // выводим рейтинг продаж
        foreach (var item in report.TopSales)
        {
            Console.WriteLine(
                $"{item.ProductName} | " +
                $"Продано: {item.TotalQuantity} | " +
                $"Заказов: {item.OrdersCount} | " +
                $"Выручка: {item.Revenue:F2} руб.");
        }

        // печатаем показатели по всем товарам
        Console.WriteLine("\n=== ОБЩИЕ ПОКАЗАТЕЛИ ===");

        Console.WriteLine($"Всего товаров: {report.TotalProducts}");
        Console.WriteLine($"Активных товаров: {report.ActiveProducts}");
        Console.WriteLine($"Товаров без остатка: {report.OutOfStockProducts}");
        Console.WriteLine($"Стоимость запасов: {report.TotalStockValue:F2}");

        Console.WriteLine(
            $"Средняя цена активных товаров: " +
            $"{FormatPrice(report.AverageActiveProductPrice)}");

        // демонстрируем результаты операторов Any, All, FirstOrDefault и Single
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

    // форматирует необязательную цену для вывода
    // price — цена или null при отсутствии данных
    // возвращает: цена с двумя десятичными знаками либо строка об отсутствии данных
    private static string FormatPrice(decimal? price)
    {
        // для null используем текст «Нет данных», иначе форматируем цену
        return price.HasValue
            ? $"{price.Value:F2} руб."
            : "Нет данных";
    }

    // показывает отличие отложенного запроса Where от списка, материализованного через ToList
    // products — изменяемый список товаров, в который добавляется демонстрационный товар
    private static void DemonstrateDeferredExecution(
        List<Product> products)
    {
        Console.WriteLine("\n=== ОТЛОЖЕННОЕ ВЫПОЛНЕНИЕ ===");

        // здесь Where создаёт отложенный запрос, который будет выполнен при перечислении
        var deferredQuery = products
            .Where(product => product.IsActive);


        // здесь ToList выполняет запрос сейчас и сохраняет состав результата в отдельном списке
        var snapshot = deferredQuery.ToList();

        // первый Count выполняет отложенный запрос до изменения данных
        Console.WriteLine(
            $"До добавления товара: {deferredQuery.Count()}");

        // добавляем активный товар в исходную коллекцию
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

        // повторное выполнение запроса учитывает новый товар
        Console.WriteLine(
            $"Отложенный запрос: {deferredQuery.Count()}");

        // ранее созданный список сохраняет прежний состав элементов
        Console.WriteLine(
            $"Материализованный снимок: {snapshot.Count}");
    }
}
