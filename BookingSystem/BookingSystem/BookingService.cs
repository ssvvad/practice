
using System;

namespace BookingSystem;

// возможные результаты попытки бронирования
public enum BookingStatus
{
    // бронь подтверждена: оплата успешна
    Confirmed,
    // оплата отклонена, места освобождены
    PaymentFailed
}

// неизменяемые входные данные запроса на бронирование
// параметр EventId — идентификатор мероприятия
// параметр CustomerEmail — электронная почта клиента
// параметр SeatsCount — количество запрашиваемых мест
// параметр PricePerSeat — стоимость одного места
public sealed record BookingRequest(
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal PricePerSeat);

// неизменяемая запись о результате бронирования
// параметр Id — уникальный идентификатор брони
// параметр EventId — идентификатор мероприятия
// параметр CustomerEmail — почта клиента
// параметр SeatsCount — количество мест
// параметр TotalAmount — общая стоимость
// параметр CreatedAt — время создания
// параметр Status — результат попытки оплаты
public sealed record Booking(
    Guid Id,
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    BookingStatus Status);

// контракт доступа к мероприятиям и управления свободными местами
public interface IEventRepository
{
    // проверяет существование мероприятия
    // eventId — идентификатор мероприятия
    // возвращает: true, если мероприятие существует; иначе false
    bool Exists(int eventId);

    // получает количество свободных мест
    // eventId — идентификатор мероприятия
    // возвращает: количество доступных мест
    int GetAvailableSeats(int eventId);

    // резервирует заданное количество мест
    // eventId — идентификатор мероприятия
    // seatsCount — количество резервируемых мест
    void ReserveSeats(int eventId, int seatsCount);

    // освобождает ранее зарезервированные места
    // eventId — идентификатор мероприятия
    // seatsCount — количество освобождаемых мест
    void ReleaseSeats(int eventId, int seatsCount);
}

// контракт хранения созданных записей о бронировании
public interface IBookingRepository
{
    // сохраняет запись о бронировании
    // booking — запись, которую необходимо сохранить
    void Save(Booking booking);
}

// контракт платёжного сервиса
public interface IPaymentService
{
    // пытается списать указанную сумму с клиента
    // customerEmail — электронная почта клиента
    // amount — сумма платежа
    // возвращает: true при успешной оплате; false при отказе
    bool Charge(string customerEmail, decimal amount);
}

// абстракция источника времени для независимости от системных часов
public interface IClock
{
    // текущая дата и время в UTC
    DateTimeOffset UtcNow { get; }
}

// контракт генерации идентификаторов бронирования
public interface IBookingIdGenerator
{
    // создаёт идентификатор брони
    // возвращает: идентификатор типа Guid
    Guid Create();
}

// реализует бизнес-логику бронирования: проверку запроса, резервирование, оплату и сохранение результата
public sealed class BookingService
{
    // зависимости представлены интерфейсами, чтобы их можно было заменить тестовыми реализациями
    private readonly IEventRepository eventRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly IPaymentService paymentService;
    private readonly IClock clock;
    private readonly IBookingIdGenerator idGenerator;

    // принимает необходимые зависимости через конструктор и проверяет их на null
    // eventRepository — доступ к мероприятиям и свободным местам
    // bookingRepository — хранилище бронирований
    // paymentService — сервис списания оплаты
    // clock — источник времени UTC
    // idGenerator — генератор идентификаторов брони
    // исключение ArgumentNullException — одна из зависимостей равна null
    public BookingService(
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IPaymentService paymentService,
        IClock clock,
        IBookingIdGenerator idGenerator)
    {
        // оператор ?? не позволяет создать сервис без обязательной зависимости
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

    // создаёт бронь после проверки запроса и доступности мест, обрабатывает результат оплаты
    // request — запрос с данными мероприятия, клиента, количества мест и цены
    // возвращает: сохранённая запись со статусом Confirmed или PaymentFailed
    // исключение ArgumentNullException — запрос равен null
    // исключение ArgumentOutOfRangeException — недопустимый ID, количество мест или цена
    // исключение ArgumentException — некорректная электронная почта
    // исключение InvalidOperationException — мероприятие отсутствует или мест недостаточно
    public Booking CreateBooking(BookingRequest request)
    {
        // проверяем наличие объекта запроса
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // идентификатор мероприятия должен быть положительным
        if (request.EventId <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.EventId));

        // базовая проверка email: не пустой, не только пробелы, содержит символ @
        if (string.IsNullOrWhiteSpace(request.CustomerEmail)
            || !request.CustomerEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Некорректный email.",
                nameof(request.CustomerEmail));
        }

        // за один запрос разрешено бронировать от 1 до 10 мест
        if (request.SeatsCount < 1 || request.SeatsCount > 10)
            throw new ArgumentOutOfRangeException(nameof(request.SeatsCount));

        // цена одного места должна быть больше нуля
        if (request.PricePerSeat <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.PricePerSeat));

        // проверяем существование мероприятия до обращения к остатку мест
        if (!eventRepository.Exists(request.EventId))
            throw new InvalidOperationException(
                "Мероприятие не существует.");

        // сравниваем требуемое число мест с доступным
        if (eventRepository.GetAvailableSeats(request.EventId)
            < request.SeatsCount)
        {
            throw new InvalidOperationException(
                "Недостаточно свободных мест.");
        }

        // итоговая стоимость равна количеству мест, умноженному на цену одного места
        decimal totalAmount =
            request.SeatsCount * request.PricePerSeat;

        // получаем идентификатор и время через внедрённые зависимости
        Guid id = idGenerator.Create();
        DateTimeOffset createdAt = clock.UtcNow;

        // резервируем места до попытки оплаты
        eventRepository.ReserveSeats(
            request.EventId,
            request.SeatsCount);

        // переменная хранит результат списания
        bool paymentSucceeded;

        // выполняем оплату внутри try, чтобы вернуть места при исключении
        try
        {
            paymentSucceeded = paymentService.Charge(
                request.CustomerEmail,
                totalAmount);
        }
        // при исключении платёжного сервиса отменяем резервирование
        catch
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);

            // повторно выбрасываем исходное исключение; запись о брони не сохраняется
            throw;
        }

        // если оплата вернула false, также освобождаем места, но продолжаем создание записи
        if (!paymentSucceeded)
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);
        }

        // формируем запись с соответствующим статусом оплаты
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

        // сохраняем результат, включая запись об отказе оплаты
        bookingRepository.Save(booking);

        // возвращаем созданную запись вызывающему коду
        return booking;
    }
}
