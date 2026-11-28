
using System;

namespace BookingSystem;

/// <summary>
/// Возможные результаты попытки бронирования.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Бронь подтверждена: оплата успешна.
    /// </summary>
    Confirmed,
    /// <summary>
    /// Оплата отклонена, места освобождены.
    /// </summary>
    PaymentFailed
}

/// <summary>
/// Неизменяемые входные данные запроса на бронирование.
/// </summary>
/// <param name="EventId">Идентификатор мероприятия.</param>
/// <param name="CustomerEmail">Электронная почта клиента.</param>
/// <param name="SeatsCount">Количество запрашиваемых мест.</param>
/// <param name="PricePerSeat">Стоимость одного места.</param>
public sealed record BookingRequest(
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal PricePerSeat);

/// <summary>
/// Неизменяемая запись о результате бронирования.
/// </summary>
/// <param name="Id">Уникальный идентификатор брони.</param>
/// <param name="EventId">Идентификатор мероприятия.</param>
/// <param name="CustomerEmail">Почта клиента.</param>
/// <param name="SeatsCount">Количество мест.</param>
/// <param name="TotalAmount">Общая стоимость.</param>
/// <param name="CreatedAt">Время создания.</param>
/// <param name="Status">Результат попытки оплаты.</param>
public sealed record Booking(
    Guid Id,
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    BookingStatus Status);

/// <summary>
/// Контракт доступа к мероприятиям и управления свободными местами.
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Проверяет существование мероприятия.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    /// <returns>true, если мероприятие существует; иначе false.</returns>
    bool Exists(int eventId);

    /// <summary>
    /// Получает количество свободных мест.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    /// <returns>Количество доступных мест.</returns>
    int GetAvailableSeats(int eventId);

    /// <summary>
    /// Резервирует заданное количество мест.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    /// <param name="seatsCount">Количество резервируемых мест.</param>
    void ReserveSeats(int eventId, int seatsCount);

    /// <summary>
    /// Освобождает ранее зарезервированные места.
    /// </summary>
    /// <param name="eventId">Идентификатор мероприятия.</param>
    /// <param name="seatsCount">Количество освобождаемых мест.</param>
    void ReleaseSeats(int eventId, int seatsCount);
}

/// <summary>
/// Контракт хранения созданных записей о бронировании.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Сохраняет запись о бронировании.
    /// </summary>
    /// <param name="booking">Запись, которую необходимо сохранить.</param>
    void Save(Booking booking);
}

/// <summary>
/// Контракт платёжного сервиса.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Пытается списать указанную сумму с клиента.
    /// </summary>
    /// <param name="customerEmail">Электронная почта клиента.</param>
    /// <param name="amount">Сумма платежа.</param>
    /// <returns>true при успешной оплате; false при отказе.</returns>
    bool Charge(string customerEmail, decimal amount);
}

/// <summary>
/// Абстракция источника времени для независимости от системных часов.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Текущая дата и время в UTC.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}

/// <summary>
/// Контракт генерации идентификаторов бронирования.
/// </summary>
public interface IBookingIdGenerator
{
    /// <summary>
    /// Создаёт идентификатор брони.
    /// </summary>
    /// <returns>Идентификатор типа Guid.</returns>
    Guid Create();
}

/// <summary>
/// Реализует бизнес-логику бронирования: проверку запроса, резервирование, оплату и сохранение результата.
/// </summary>
public sealed class BookingService
{
    // Зависимости представлены интерфейсами, чтобы их можно было заменить тестовыми реализациями.
    private readonly IEventRepository eventRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly IPaymentService paymentService;
    private readonly IClock clock;
    private readonly IBookingIdGenerator idGenerator;

    /// <summary>
    /// Принимает необходимые зависимости через конструктор и проверяет их на null.
    /// </summary>
    /// <param name="eventRepository">Доступ к мероприятиям и свободным местам.</param>
    /// <param name="bookingRepository">Хранилище бронирований.</param>
    /// <param name="paymentService">Сервис списания оплаты.</param>
    /// <param name="clock">Источник времени UTC.</param>
    /// <param name="idGenerator">Генератор идентификаторов брони.</param>
    /// <exception cref="ArgumentNullException">Одна из зависимостей равна null.</exception>
    public BookingService(
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IPaymentService paymentService,
        IClock clock,
        IBookingIdGenerator idGenerator)
    {
        // Оператор ?? не позволяет создать сервис без обязательной зависимости.
        this.eventRepository = eventRepository
            ?? throw new ArgumentNullException(nameof(eventRepository));

        this.bookingRepository = bookingRepository
            ?? throw new ArgumentNullException(nameof(bookingRepository));

        this.paymentService = paymentService
            ?? throw new ArgumentNullException(nameof(paymentService));

        this.clock = clock
            ?? throw new ArgumentNullException(nameof(clock));

        this.idGenerator = idGenerator
            ?? throw new ArgumentNullException(nameof(idGenerator));
    }

    /// <summary>
    /// Создаёт бронь после проверки запроса и доступности мест, обрабатывает результат оплаты.
    /// </summary>
    /// <param name="request">Запрос с данными мероприятия, клиента, количества мест и цены.</param>
    /// <returns>Сохранённая запись со статусом Confirmed или PaymentFailed.</returns>
    /// <exception cref="ArgumentNullException">Запрос равен null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Недопустимый ID, количество мест или цена.</exception>
    /// <exception cref="ArgumentException">Некорректная электронная почта.</exception>
    /// <exception cref="InvalidOperationException">Мероприятие отсутствует или мест недостаточно.</exception>
    public Booking CreateBooking(BookingRequest request)
    {
        // Проверяем наличие объекта запроса.
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Идентификатор мероприятия должен быть положительным.
        if (request.EventId <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.EventId));

        // Базовая проверка email: не пустой, не только пробелы, содержит символ @.
        if (string.IsNullOrWhiteSpace(request.CustomerEmail)
            || !request.CustomerEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Некорректный email.",
                nameof(request.CustomerEmail));
        }

        // За один запрос разрешено бронировать от 1 до 10 мест.
        if (request.SeatsCount < 1 || request.SeatsCount > 10)
            throw new ArgumentOutOfRangeException(nameof(request.SeatsCount));

        // Цена одного места должна быть больше нуля.
        if (request.PricePerSeat <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.PricePerSeat));

        // Проверяем существование мероприятия до обращения к остатку мест.
        if (!eventRepository.Exists(request.EventId))
            throw new InvalidOperationException(
                "Мероприятие не существует.");

        // Сравниваем требуемое число мест с доступным.
        if (eventRepository.GetAvailableSeats(request.EventId)
            < request.SeatsCount)
        {
            throw new InvalidOperationException(
                "Недостаточно свободных мест.");
        }

        // Итоговая стоимость равна количеству мест, умноженному на цену одного места.
        decimal totalAmount =
            request.SeatsCount * request.PricePerSeat;

        // Получаем идентификатор и время через внедрённые зависимости.
        Guid id = idGenerator.Create();
        DateTimeOffset createdAt = clock.UtcNow;

        // Резервируем места до попытки оплаты.
        eventRepository.ReserveSeats(
            request.EventId,
            request.SeatsCount);

        // Переменная хранит результат списания.
        bool paymentSucceeded;

        // Выполняем оплату внутри try, чтобы вернуть места при исключении.
        try
        {
            paymentSucceeded = paymentService.Charge(
                request.CustomerEmail,
                totalAmount);
        }
        // При исключении платёжного сервиса отменяем резервирование.
        catch
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);

            // Повторно выбрасываем исходное исключение; запись о брони не сохраняется.
            throw;
        }

        // Если оплата вернула false, также освобождаем места, но продолжаем создание записи.
        if (!paymentSucceeded)
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);
        }

        // Формируем запись с соответствующим статусом оплаты.
        Booking booking = new Booking(
            id,
            request.EventId,
            request.CustomerEmail,
            request.SeatsCount,
            totalAmount,
            createdAt,
            paymentSucceeded
                ? BookingStatus.Confirmed
                : BookingStatus.PaymentFailed);

        // Сохраняем результат, включая запись об отказе оплаты.
        bookingRepository.Save(booking);

        // Возвращаем созданную запись вызывающему коду.
        return booking;
    }
}
