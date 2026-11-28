
using System;
using System.Collections.Generic;

// класс для хранения данных одной товарной позиции заказа
public sealed class OrderItem
{
    // создаёт позицию с названием, ценой и количеством
    // name — название товара
    // price — цена за единицу
    // quantity — количество единиц товара
    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    // название товара
    public string Name { get; }
    // цена одной единицы товара
    public decimal Price { get; }
    // количество единиц товара
    public int Quantity { get; }
}

// данные события о завершении обработки заказа
public sealed class OrderProcessedEventArgs : EventArgs
{
    // формирует сведения о завершённой обработке
    // orderId — номер заказа
    // total — итоговая стоимость
    // processedItemsCount — количество обработанных позиций
    // processedAt — дата и время обработки
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

    // идентификатор заказа
    public int OrderId { get; }
    // итоговая сумма заказа
    public decimal Total { get; }
    // количество включённых в расчёт позиций, а не отдельных единиц товара
    public int ProcessedItemsCount { get; }
    // момент завершения обработки
    public DateTimeOffset ProcessedAt { get; }
}

// делегат для расчёта стоимости одной позиции по преобразованной цене
// item — позиция заказа
// transformedUnitPrice — цена единицы после преобразования
// возвращает: стоимость позиции
public delegate decimal ItemTotalCalculator(
    OrderItem item,
    decimal transformedUnitPrice);

// обрабатывает заказ с помощью переданных делегатов и уведомляет подписчиков о результате
public sealed class OrderProcessor
{
    // событие, вызываемое после завершения расчёта заказа
    public event EventHandler<OrderProcessedEventArgs>? OrderProcessed;

    // проверяет позиции, фильтрует их, преобразует цены, рассчитывает итог и вызывает событие
    // orderId — положительный номер заказа
    // items — коллекция товарных позиций
    // shouldProcess — предикат включения позиции в расчёт
    // transformPrice — функция преобразования цены единицы
    // calculateItemTotal — делегат расчёта стоимости позиции
    // log — необязательный делегат журналирования
    // возвращает: итоговая стоимость всех включённых позиций
    public decimal Process(
        int orderId,
        IEnumerable<OrderItem> items,
        Predicate<OrderItem> shouldProcess,
        Func<decimal, decimal> transformPrice,
        ItemTotalCalculator calculateItemTotal,
        Action<string>? log = null)
    {
        // проверяем обязательные коллекцию и делегаты на null
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (shouldProcess == null)
            throw new ArgumentNullException(nameof(shouldProcess));

        if (transformPrice == null)
            throw new ArgumentNullException(nameof(transformPrice));

        if (calculateItemTotal == null)
            throw new ArgumentNullException(nameof(calculateItemTotal));

        // номер заказа должен быть положительным
        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        // накопитель итоговой суммы и счётчик обработанных позиций
        decimal total = 0;
        int processedItemsCount = 0;

        // оператор ?.Invoke вызывает журналирование только при наличии делегата
        log?.Invoke($"Начало обработки заказа №{orderId}");

        // последовательно обрабатываем все позиции заказа
        foreach (OrderItem item in items)
        {
            // проверяем, что позиция существует
            if (item == null)
                throw new ArgumentException("Позиция заказа не может быть null.");

            // отрицательная цена недопустима
            if (item.Price < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Price),
                    "Цена не может быть отрицательной.");

            // количество должно быть строго положительным
            if (item.Quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Quantity),
                    "Количество должно быть положительным.");

            // делегат Predicate определяет, следует ли учитывать позицию; continue пропускает исключённую позицию
            if (!shouldProcess(item))
            {
                log?.Invoke($"Товар {item.Name} исключён из расчёта.");
                continue;
            }

            // делегат Func преобразует исходную цену единицы товара
            decimal transformedPrice = transformPrice(item.Price);

            log?.Invoke(
                $"{item.Name}: цена {item.Price:F2} -> {transformedPrice:F2} руб.");

            // передаём позицию и преобразованную цену пользовательскому делегату
            decimal itemTotal = calculateItemTotal(
                item, transformedPrice);

            // прибавляем стоимость позиции и увеличиваем счётчик
            total += itemTotal;
            processedItemsCount++;

            log?.Invoke(
                $"{item.Name}: стоимость позиции {itemTotal:F2} руб.");
        }

        // записываем итог обработки в журнал
        log?.Invoke(
            $"Заказ №{orderId} обработан. Итог: {total:F2} руб.");

        // создаём объект данных события с текущим временем
        OrderProcessedEventArgs args = new OrderProcessedEventArgs(
            orderId,
            total,
            processedItemsCount,
            DateTimeOffset.Now);

        // оповещаем подписчиков о завершении обработки
        OnOrderProcessed(args);

        return total;
    }

    // вызывает каждого подписчика события отдельно, чтобы ошибка одного не мешала остальным
    // e — данные обработанного заказа
    private void OnOrderProcessed(OrderProcessedEventArgs e)
    {
        // сохраняем список подписчиков в локальную переменную
        EventHandler<OrderProcessedEventArgs>? handlers = OrderProcessed;

        // если подписчиков нет, завершать вызов нечего
        if (handlers == null)
            return;

        // метод GetInvocationList возвращает отдельные обработчики многоадресного делегата
        foreach (EventHandler<OrderProcessedEventArgs> handler
                 in handlers.GetInvocationList())
        {
            // изолируем выполнение каждого обработчика
            try
            {
                handler(this, e);
            }
            // регистрируем ошибку и продолжаем вызов оставшихся подписчиков
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ошибка обработчика события: {ex.Message}");
            }
        }
    }
}

// демонстрирует делегаты, лямбда-выражения и события на примере заказа
class Program
{
    // метод Main: настраивает правила обработки заказа, подписчиков и журналирование
    static void Main()
    {
        // тестовые позиции с разными ценами и количеством
        OrderItem[] items =
        {
            new OrderItem("Ноутбук", 50000m, 1),
            new OrderItem("Мышь", 1500m, 2),
            new OrderItem("Клавиатура", 3000m, 1),
            new OrderItem("Кабель", 300m, 3)
        };

        OrderProcessor processor = new OrderProcessor();

        // скидка 10% и минимальная цена позиции для включения в расчёт
        decimal discount = 0.10m;
        decimal minPrice = 500m;

        // предикат захватывает minPrice из окружающей области видимости
        Predicate<OrderItem> shouldProcess =
            item => item.Price >= minPrice;

        // лямбда применяет скидку к цене единицы
        Func<decimal, decimal> applyDiscount =
            price => price * (1 - discount);

        // статическая лямбда округляет цену и не захватывает внешние переменные
        Func<decimal, decimal> roundPrice =
            static price => Math.Round(price, 2);

        // композиция функций: сначала скидка, затем округление
        Func<decimal, decimal> transformPrice =
            price => roundPrice(applyDiscount(price));

        // рассчитываем стоимость позиции с учётом количества
        ItemTotalCalculator calculateItemTotal =
            static (item, price) => price * item.Quantity;

        // список для сохранения сообщений журнала
        List<string> journal = new List<string>();

        // первый обработчик Action выводит сообщение в консоль
        Action<string> log =
            static message => Console.WriteLine($"[Журнал] {message}");

        // добавляем второй обработчик: он сохраняет сообщение в список
        log += message => journal.Add(message);

        // подписчик имитирует уведомление покупателя
        EventHandler<OrderProcessedEventArgs> customerNotification =
            static (sender, e) =>
            {
                Console.WriteLine(
                    $"Покупатель уведомлён о заказе №{e.OrderId}.");
            };

        // подписчик намеренно выбрасывает исключение для проверки изоляции ошибок
        EventHandler<OrderProcessedEventArgs> errorHandler =
            static (sender, e) =>
            {
                throw new InvalidOperationException(
                    "Не удалось отправить уведомление.");
            };

        // подписчик выводит статистику обработанного заказа
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

        // подписываем три обработчика на событие
        processor.OrderProcessed += customerNotification;
        processor.OrderProcessed += errorHandler;
        processor.OrderProcessed += statisticsHandler;

        try
        {
            // обрабатываем заказ со всеми настроенными делегатами
            decimal total = processor.Process(
                1,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            Console.WriteLine($"К оплате: {total:F2} руб.");

            // удаляем два обработчика, оставляя только статистику
            processor.OrderProcessed -= errorHandler;
            processor.OrderProcessed -= customerNotification;

            Console.WriteLine(
                "\nПовторная обработка после отписки:");

            // повторяем расчёт после отписки от события
            processor.Process(
                2,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            // выводим количество сообщений, накопленных за оба вызова
            Console.WriteLine(
                $"\nКоличество записей в журнале: {journal.Count}");
        }
        // обрабатываем исключения самого процесса расчёта
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки заказа: {ex.Message}");
        }

        // отписываем последний обработчик события
        processor.OrderProcessed -= statisticsHandler;
    }
}
