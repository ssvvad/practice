
using System;
using System.Collections.Generic;

public sealed class OrderItem
{
    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }
}

public sealed class OrderProcessedEventArgs : EventArgs
{
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

    public int OrderId { get; }
    public decimal Total { get; }
    public int ProcessedItemsCount { get; }
    public DateTimeOffset ProcessedAt { get; }
}

public delegate decimal ItemTotalCalculator(
    OrderItem item,
    decimal transformedUnitPrice);

public sealed class OrderProcessor
{
    public event EventHandler<OrderProcessedEventArgs>? OrderProcessed;

    public decimal Process(
        int orderId,
        IEnumerable<OrderItem> items,
        Predicate<OrderItem> shouldProcess,
        Func<decimal, decimal> transformPrice,
        ItemTotalCalculator calculateItemTotal,
        Action<string>? log = null)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (shouldProcess == null)
            throw new ArgumentNullException(nameof(shouldProcess));

        if (transformPrice == null)
            throw new ArgumentNullException(nameof(transformPrice));

        if (calculateItemTotal == null)
            throw new ArgumentNullException(nameof(calculateItemTotal));

        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        decimal total = 0;
        int processedItemsCount = 0;

        log?.Invoke($"Начало обработки заказа №{orderId}");

        foreach (OrderItem item in items)
        {
            if (item == null)
                throw new ArgumentException("Позиция заказа не может быть null.");

            if (item.Price < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Price),
                    "Цена не может быть отрицательной.");

            if (item.Quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(item.Quantity),
                    "Количество должно быть положительным.");

            if (!shouldProcess(item))
            {
                log?.Invoke($"Товар {item.Name} исключён из расчёта.");
                continue;
            }

            decimal transformedPrice = transformPrice(item.Price);

            log?.Invoke(
                $"{item.Name}: цена {item.Price:F2} -> {transformedPrice:F2} руб.");

            decimal itemTotal = calculateItemTotal(
                item, transformedPrice);

            total += itemTotal;
            processedItemsCount++;

            log?.Invoke(
                $"{item.Name}: стоимость позиции {itemTotal:F2} руб.");
        }

        log?.Invoke(
            $"Заказ №{orderId} обработан. Итог: {total:F2} руб.");

        OrderProcessedEventArgs args = new OrderProcessedEventArgs(
            orderId,
            total,
            processedItemsCount,
            DateTimeOffset.Now);

        OnOrderProcessed(args);

        return total;
    }

    private void OnOrderProcessed(OrderProcessedEventArgs e)
    {
        EventHandler<OrderProcessedEventArgs>? handlers = OrderProcessed;

        if (handlers == null)
            return;

        foreach (EventHandler<OrderProcessedEventArgs> handler
                 in handlers.GetInvocationList())
        {
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ошибка обработчика события: {ex.Message}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        OrderItem[] items =
        {
            new OrderItem("Ноутбук", 50000m, 1),
            new OrderItem("Мышь", 1500m, 2),
            new OrderItem("Клавиатура", 3000m, 1),
            new OrderItem("Кабель", 300m, 3)
        };

        OrderProcessor processor = new OrderProcessor();

        decimal discount = 0.10m;
        decimal minPrice = 500m;

        Predicate<OrderItem> shouldProcess =
            item => item.Price >= minPrice;

        Func<decimal, decimal> applyDiscount =
            price => price * (1 - discount);

        Func<decimal, decimal> roundPrice =
            static price => Math.Round(price, 2);

        Func<decimal, decimal> transformPrice =
            price => roundPrice(applyDiscount(price));

        ItemTotalCalculator calculateItemTotal =
            static (item, price) => price * item.Quantity;

        List<string> journal = new List<string>();

        Action<string> log =
            static message => Console.WriteLine($"[Журнал] {message}");

        log += message => journal.Add(message);

        EventHandler<OrderProcessedEventArgs> customerNotification =
            static (sender, e) =>
            {
                Console.WriteLine(
                    $"Покупатель уведомлён о заказе №{e.OrderId}.");
            };

        EventHandler<OrderProcessedEventArgs> errorHandler =
            static (sender, e) =>
            {
                throw new InvalidOperationException(
                    "Не удалось отправить уведомление.");
            };

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

        processor.OrderProcessed += customerNotification;
        processor.OrderProcessed += errorHandler;
        processor.OrderProcessed += statisticsHandler;

        try
        {
            decimal total = processor.Process(
                1,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            Console.WriteLine($"К оплате: {total:F2} руб.");

            processor.OrderProcessed -= errorHandler;
            processor.OrderProcessed -= customerNotification;

            Console.WriteLine(
                "\nПовторная обработка после отписки:");

            processor.Process(
                2,
                items,
                shouldProcess,
                transformPrice,
                calculateItemTotal,
                log);

            Console.WriteLine(
                $"\nКоличество записей в журнале: {journal.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки заказа: {ex.Message}");
        }

        processor.OrderProcessed -= statisticsHandler;
    }
}
