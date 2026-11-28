
using System;
using System.Collections.Generic;

/// <summary>
/// Модель одной товарной позиции заказа.
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// Создаёт позицию с названием, ценой и количеством.
    /// </summary>
    /// <param name="name">Название товара.</param>
    /// <param name="price">Цена за единицу.</param>
    /// <param name="quantity">Количество единиц товара.</param>
    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    /// <summary>
    /// Название товара.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Цена одной единицы товара.
    /// </summary>
    public decimal Price { get; }
    /// <summary>
    /// Количество единиц товара.
    /// </summary>
    public int Quantity { get; }
}

/// <summary>
/// Данные события о завершении обработки заказа.
/// </summary>
public sealed class OrderProcessedEventArgs : EventArgs
{
    /// <summary>
    /// Формирует сведения о завершённой обработке.
    /// </summary>
    /// <param name="orderId">Номер заказа.</param>
    /// <param name="total">Итоговая стоимость.</param>
    /// <param name="processedItemsCount">Количество обработанных позиций.</param>
    /// <param name="processedAt">Дата и время обработки.</param>
    public OrderProcessedEventArgs(
        int orderId,
        decimal total,
        int processedItemsCount,
        DateTimeOffset processedAt)
    {
        OrderId = orderId;
        Total = total;
        ProcessedItemsCount = processedItemsCount;
        ProcessedAt = processedAt;
    }

    /// <summary>
    /// Идентификатор заказа.
    /// </summary>
    public int OrderId { get; }
    /// <summary>
    /// Итоговая сумма заказа.
    /// </summary>
    public decimal Total { get; }
    /// <summary>
    /// Количество включённых в расчёт позиций, а не отдельных единиц товара.
    /// </summary>
    public int ProcessedItemsCount { get; }
    /// <summary>
    /// Момент завершения обработки.
    /// </summary>
    public DateTimeOffset ProcessedAt { get; }
}

/// <summary>
/// Делегат для расчёта стоимости одной позиции по преобразованной цене.
/// </summary>
/// <param name="item">Позиция заказа.</param>
/// <param name="transformedUnitPrice">Цена единицы после преобразования.</param>
/// <returns>Стоимость позиции.</returns>
public delegate decimal ItemTotalCalculator(
    OrderItem item,
    decimal transformedUnitPrice);

/// <summary>
/// Обрабатывает заказ с помощью переданных делегатов и уведомляет подписчиков о результате.
/// </summary>
public sealed class OrderProcessor
{
    /// <summary>
    /// Событие, вызываемое после завершения расчёта заказа.
    /// </summary>
    public event EventHandler<OrderProcessedEventArgs>? OrderProcessed;

    /// <summary>
    /// Проверяет позиции, фильтрует их, преобразует цены, рассчитывает итог и вызывает событие.
    /// </summary>
    /// <param name="orderId">Положительный номер заказа.</param>
    /// <param name="items">Коллекция товарных позиций.</param>
    /// <param name="shouldProcess">Предикат включения позиции в расчёт.</param>
    /// <param name="transformPrice">Функция преобразования цены единицы.</param>
    /// <param name="calculateItemTotal">Делегат расчёта стоимости позиции.</param>
    /// <param name="log">Необязательный делегат журналирования.</param>
    /// <returns>Итоговая стоимость всех включённых позиций.</returns>
    public decimal Process(
        int orderId,
        IEnumerable<OrderItem> items,
        Predicate<OrderItem> shouldProcess,
        Func<decimal, decimal> transformPrice,
        ItemTotalCalculator calculateItemTotal,
        Action<string>? log = null)
    {
        // Проверяем обязательные коллекцию и делегаты на null.
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (shouldProcess == null)
            throw new ArgumentNullException(nameof(shouldProcess));

        if (transformPrice == null)
            throw new ArgumentNullException(nameof(transformPrice));

        if (calculateItemTotal == null)
            throw new ArgumentNullException(nameof(calculateItemTotal));

        // Номер заказа должен быть положительным.
        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        // Накопитель итоговой суммы и счётчик обработанных позиций.
        decimal total = 0;
        int processedItemsCount = 0;

        // Оператор ?.Invoke вызывает журналирование только при наличии делегата.
        log?.Invoke($"Начало обработки заказа №{orderId}");

        // Последовательно обрабатываем все позиции заказа.
        foreach (OrderItem item in items)
        {
            // Проверяем, что позиция существует.
            if (item == null)
                throw new ArgumentException("Позиция заказа не может быть null.");

            // Отрицательная цена недопустима.
            if (item.Price < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Price),
                    "Цена не может быть отрицательной.");

            // Количество должно быть строго положительным.
            if (item.Quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Quantity),
                    "Количество должно быть положительным.");

            // Predicate определяет, следует ли учитывать позицию; continue пропускает исключённую позицию.
            if (!shouldProcess(item))
            {
                log?.Invoke($"Товар {item.Name} исключён из расчёта.");
                continue;
            }

            // Func преобразует исходную цену единицы товара.
            decimal transformedPrice = transformPrice(item.Price);

            log?.Invoke(
                $"{item.Name}: цена {item.Price:F2} -> {transformedPrice:F2} руб.");

            // Передаём позицию и преобразованную цену пользовательскому делегату.
            decimal itemTotal = calculateItemTotal(
                item, transformedPrice);

            // Прибавляем стоимость позиции и увеличиваем счётчик.
            total += itemTotal;
            processedItemsCount++;

            log?.Invoke(
                $"{item.Name}: стоимость позиции {itemTotal:F2} руб.");
        }

        // Записываем итог обработки в журнал.
        log?.Invoke(
            $"Заказ №{orderId} обработан. Итог: {total:F2} руб.");

        // Создаём объект данных события с текущим временем.
        OrderProcessedEventArgs args = new OrderProcessedEventArgs(
            orderId,
            total,
            processedItemsCount,
            DateTimeOffset.Now);

        // Оповещаем подписчиков о завершении обработки.
        OnOrderProcessed(args);

        return total;
    }

    /// <summary>
    /// Вызывает каждого подписчика события отдельно, чтобы ошибка одного не мешала остальным.
    /// </summary>
    /// <param name="e">Данные обработанного заказа.</param>
    private void OnOrderProcessed(OrderProcessedEventArgs e)
    {
        // Сохраняем список подписчиков в локальную переменную.
        EventHandler<OrderProcessedEventArgs>? handlers = OrderProcessed;

        // Если подписчиков нет, завершать вызов нечего.
        if (handlers == null)
            return;

        // GetInvocationList возвращает отдельные обработчики многоадресного делегата.
        foreach (EventHandler<OrderProcessedEventArgs> handler
                 in handlers.GetInvocationList())
        {
            // Изолируем выполнение каждого обработчика.
            try
            {
                handler(this, e);
            }
            // Регистрируем ошибку и продолжаем вызов оставшихся подписчиков.
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ошибка обработчика события: {ex.Message}");
            }
        }
    }
}

/// <summary>
/// Демонстрирует делегаты, лямбда-выражения и события на примере заказа.
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа: настраивает правила обработки заказа, подписчиков и журналирование.
    /// </summary>
    static void Main()
    {
        // Тестовые позиции с разными ценами и количеством.
        OrderItem[] items =
        {
            new OrderItem("Ноутбук", 50000m, 1),
            new OrderItem("Мышь", 1500m, 2),
            new OrderItem("Клавиатура", 3000m, 1),
            new OrderItem("Кабель", 300m, 3)
        };

        OrderProcessor processor = new OrderProcessor();

        // Скидка 10% и минимальная цена позиции для включения в расчёт.
        decimal discount = 0.10m;
        decimal minPrice = 500m;

        // Предикат захватывает minPrice из окружающей области видимости.
        Predicate<OrderItem> shouldProcess =
            item => item.Price >= minPrice;

        // Лямбда применяет скидку к цене единицы.
        Func<decimal, decimal> applyDiscount =
            price => price * (1 - discount);

        // Статическая лямбда округляет цену и не захватывает внешние переменные.
        Func<decimal, decimal> roundPrice =
            static price => Math.Round(price, 2);

        // Композиция функций: сначала скидка, затем округление.
        Func<decimal, decimal> transformPrice =
            price => roundPrice(applyDiscount(price));

        // Рассчитываем стоимость позиции с учётом количества.
        ItemTotalCalculator calculateItemTotal =
            static (item, price) => price * item.Quantity;

        // Список для сохранения сообщений журнала.
        List<string> journal = new List<string>();

        // Первый обработчик Action выводит сообщение в консоль.
        Action<string> log =
            static message => Console.WriteLine($"[Журнал] {message}");

        // Добавляем второй обработчик: он сохраняет сообщение в список.
        log += message => journal.Add(message);

        // Подписчик имитирует уведомление покупателя.
        EventHandler<OrderProcessedEventArgs> customerNotification =
            static (sender, e) =>
            {
                Console.WriteLine(
                    $"Покупатель уведомлён о заказе №{e.OrderId}.");
            };

        // Подписчик намеренно выбрасывает исключение для проверки изоляции ошибок.
        EventHandler<OrderProcessedEventArgs> errorHandler =
            static (sender, e) =>
            {
                throw new InvalidOperationException(
                    "Не удалось отправить уведомление.");
            };

        // Подписчик выводит статистику обработанного заказа.
        EventHandler<OrderProcessedEventArgs> statisticsHandler =
            static (sender, e) =>
            {
                Console.WriteLine(
                    $"Обработано позиций: {e.ProcessedItemsCount}");

                Console.WriteLine(
                    $"Итоговая сумма: {e.Total:F2} руб.");

                Console.WriteLine(
                    $"Дата обработки: {e.ProcessedAt}");
            };

        // Подписываем три обработчика на событие.
        processor.OrderProcessed += customerNotification;
        processor.OrderProcessed += errorHandler;
        processor.OrderProcessed += statisticsHandler;

        try
        {
            // Обрабатываем заказ со всеми настроенными делегатами.
            decimal total = processor.Process(
                1,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            Console.WriteLine($"К оплате: {total:F2} руб.");

            // Удаляем два обработчика, оставляя только статистику.
            processor.OrderProcessed -= errorHandler;
            processor.OrderProcessed -= customerNotification;

            Console.WriteLine(
                "\nПовторная обработка после отписки:");

            // Повторяем расчёт после отписки от события.
            processor.Process(
                2,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            // Выводим количество сообщений, накопленных за оба вызова.
            Console.WriteLine(
                $"\nКоличество записей в журнале: {journal.Count}");
        }
        // Обрабатываем исключения самого процесса расчёта.
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки заказа: {ex.Message}");
        }

        // Отписываем последний обработчик события.
        processor.OrderProcessed -= statisticsHandler;
    }
}
