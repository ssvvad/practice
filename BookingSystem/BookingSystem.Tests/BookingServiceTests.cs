
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookingSystem;

namespace BookingSystem.Tests;

// тестовая реализация репозитория мероприятий: имитирует наличие мест и фиксирует обращения
public sealed class FakeEventRepository : IEventRepository
{
    // настраиваемый признак существования мероприятия
    public bool EventExists { get; set; } = true;
    // текущее количество свободных мест в имитации репозитория
    public int AvailableSeats { get; set; } = 20;

    // число вызовов Exists
    public int ExistsCalls { get; private set; }
    // число вызовов GetAvailableSeats
    public int GetAvailableSeatsCalls { get; private set; }
    // число вызовов ReserveSeats
    public int ReserveSeatsCalls { get; private set; }
    // число вызовов ReleaseSeats
    public int ReleaseSeatsCalls { get; private set; }

    // суммарное количество зарезервированных мест
    public int ReservedSeats { get; private set; }
    // суммарное количество освобождённых мест
    public int ReleasedSeats { get; private set; }

    // журнал порядка операций резервирования и освобождения
    public List<string> Operations { get; } = new();

    // фиксирует проверку существования и возвращает настроенное значение
    // eventId — id мероприятия; в тестовой реализации не влияет на результат
    // возвращает: настроенный признак EventExists
    public bool Exists(int eventId)
    // счётчик позволяет проверить количество обращений сервиса
    {
        ExistsCalls++;
        return EventExists;
    }

    // фиксирует запрос остатка мест
    // eventId — id мероприятия; в тестовой реализации не влияет на результат
    // возвращает: текущий остаток свободных мест
    public int GetAvailableSeats(int eventId)
    {
        GetAvailableSeatsCalls++;
        return AvailableSeats;
    }

    // имитирует резервирование, уменьшает остаток и записывает операцию
    // eventId — id мероприятия; в имитации не используется
    // seatsCount — количество резервируемых мест
    public void ReserveSeats(int eventId, int seatsCount)
    // обновляем счётчики и уменьшаем доступный остаток
    {
        ReserveSeatsCalls++;
        ReservedSeats += seatsCount;
        AvailableSeats -= seatsCount;

        Operations.Add("Reserve");
    }

    // имитирует освобождение, увеличивает остаток и записывает операцию
    // eventId — id мероприятия; в имитации не используется
    // seatsCount — количество освобождаемых мест
    public void ReleaseSeats(int eventId, int seatsCount)
    // обновляем счётчики и восстанавливаем доступный остаток
    {
        ReleaseSeatsCalls++;
        ReleasedSeats += seatsCount;
        AvailableSeats += seatsCount;

        Operations.Add("Release");
    }
}

// тестовый репозиторий бронирований, хранящий записи в памяти
public sealed class FakeBookingRepository : IBookingRepository
{
    // список записей, переданных на сохранение
    public List<Booking> SavedBookings { get; } = new();

    // число сохранённых записей (число вызовов Save)
    public int SaveCalls => SavedBookings.Count;

    // добавляет бронирование в тестовый список
    // booking — запись для сохранения
    public void Save(Booking booking)
    {
        SavedBookings.Add(booking);
    }
}

// тестовая реализация оплаты с управляемым результатом или исключением
public sealed class FakePaymentService : IPaymentService
{
    // настраиваемый результат оплаты при отсутствии исключения
    public bool Result { get; set; } = true;

    // исключение, которое следует выбросить при оплате; null означает обычную обработку
    public Exception? ExceptionToThrow { get; set; }

    // число попыток оплаты
    public int ChargeCalls { get; private set; }

    // сумма последней попытки оплаты
    public decimal ChargedAmount { get; private set; }

    // почта клиента при последней попытке оплаты
    public string? ChargedEmail { get; private set; }

    // запоминает параметры платежа и имитирует заданный результат или ошибку
    // customerEmail — почта клиента
    // amount — сумма списания
    // возвращает: настроенный результат оплаты, если исключение не задано
    public bool Charge(string customerEmail, decimal amount)
    {
        // запоминаем фактические аргументы, даже если оплата выбросит исключение
        ChargeCalls++;
        ChargedEmail = customerEmail;
        ChargedAmount = amount;

        // имитируем исключение платёжного сервиса по настройке теста
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;

        return Result;
    }
}

// тестовый источник времени, возвращающий предсказуемое значение
public sealed class FakeClock : IClock
{
    // фиксированная дата и время UTC для воспроизводимости тестов
    public DateTimeOffset FixedTime { get; set; } =
        new DateTimeOffset(
            2026, 11, 27, 10, 0, 0, TimeSpan.Zero);

    // число обращений к свойству UtcNow
    public int Calls { get; private set; }

    // возвращает фиксированное время и увеличивает счётчик обращений
    public DateTimeOffset UtcNow
    {
        get
        {
            // считаем обращения и возвращаем заранее известное время
            Calls++;
            return FixedTime;
        }
    }
}

// тестовый генератор идентификаторов, возвращающий фиксированный Guid
public sealed class FakeBookingIdGenerator : IBookingIdGenerator
{
    // предсказуемый идентификатор, используемый в тестах
    public Guid FixedId { get; set; } =
        Guid.Parse("12345678-1234-1234-1234-123456789012");

    // число вызовов генератора
    public int Calls { get; private set; }

    // фиксирует вызов и возвращает заданный идентификатор
    // возвращает: значение FixedId
    public Guid Create()
    {
        // считаем вызовы генератора
        Calls++;
        return FixedId;
    }
}

// набор модульных тестов BookingService на MSTest с подменой внешних зависимостей
[TestClass]
public class BookingServiceTests
{
    // тестовые зависимости и проверяемый сервис; null! подавляет предупреждение до Initialize
    private FakeEventRepository eventRepository = null!;
    private FakeBookingRepository bookingRepository = null!;
    private FakePaymentService paymentService = null!;
    private FakeClock clock = null!;
    private FakeBookingIdGenerator idGenerator = null!;
    private BookingService service = null!;

    // перед каждым тестом создаёт новые тестовые зависимости и экземпляр сервиса, чтобы сценарии не влияли друг на друга
    [TestInitialize]
    public void Initialize()
    {
        // создаём чистые имитации для независимости каждого теста
        eventRepository = new FakeEventRepository();
        bookingRepository = new FakeBookingRepository();
        paymentService = new FakePaymentService();
        clock = new FakeClock();
        idGenerator = new FakeBookingIdGenerator();

        // внедряем имитации вместо настоящих хранилищ, часов и платёжного сервиса
        service = new BookingService(
            eventRepository,
            bookingRepository,
            paymentService,
            clock,
            idGenerator);
    }

    // проверяет успешную бронь: статус, сумму, реквизиты, резервирование, оплату, сохранение, время и ID
    [TestMethod]
    public void CreateBooking_ValidRequest_ReturnsConfirmedBooking()
    {
        // подготовка: подготовка корректного запроса на три места
        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // выполнение: вызов проверяемого метода
        Booking booking = service.CreateBooking(request);

        // проверка: сравнение реквизитов и статуса результата с ожидаемыми
        Assert.AreEqual(idGenerator.FixedId, booking.Id);
        Assert.AreEqual(clock.FixedTime, booking.CreatedAt);
        Assert.AreEqual(request.EventId, booking.EventId);
        Assert.AreEqual(request.CustomerEmail, booking.CustomerEmail);
        Assert.AreEqual(3, booking.SeatsCount);
        Assert.AreEqual(1500m, booking.TotalAmount);
        Assert.AreEqual(BookingStatus.Confirmed, booking.Status);

        // проверяем, что резервирование выполнено ровно один раз и места не возвращены
        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(3, eventRepository.ReservedSeats);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(17, eventRepository.AvailableSeats);

        // проверяем параметры единственного платежа
        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1500m, paymentService.ChargedAmount);
        Assert.AreEqual(request.CustomerEmail, paymentService.ChargedEmail);

        // проверяем, что запись сохранена ровно один раз
        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);

        // проверяем получение времени и идентификатора через зависимости
        Assert.AreEqual(1, clock.Calls);
        Assert.AreEqual(1, idGenerator.Calls);
    }

    // проверяет отказ оплаты: места освобождаются, статус PaymentFailed сохраняется, порядок операций корректен
    [TestMethod]
    public void CreateBooking_PaymentFails_ReleasesReservedSeats()
    {
        // подготовка: имитируем отказ оплаты
        paymentService.Result = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // выполнение: выполняем бронирование, которое завершится отказом оплаты
        Booking booking = service.CreateBooking(request);

        // проверка: проверяем статус, сумму и восстановление остатка
        Assert.AreEqual(BookingStatus.PaymentFailed, booking.Status);
        Assert.AreEqual(1000m, booking.TotalAmount);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(1, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(2, eventRepository.ReservedSeats);
        Assert.AreEqual(2, eventRepository.ReleasedSeats);
        Assert.AreEqual(20, eventRepository.AvailableSeats);
// проверяем последовательность: сначала резервирование, затем освобождение

        CollectionAssert.AreEqual(
            new[] { "Reserve", "Release" },
            eventRepository.Operations);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);
    }

    // проверяет исключение при оплате: резерв отменяется, исходное исключение передаётся наружу, бронь не сохраняется
    [TestMethod]
    public void CreateBooking_PaymentThrows_ReleasesSeatsAndRethrows()
    {
        // подготовка: задаём конкретный объект исключения от платёжного сервиса
        var expectedException = new InvalidOperationException(
            "Ошибка платёжного сервиса.");

        paymentService.ExceptionToThrow = expectedException;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // выполнение: ожидаем исключение при вызове CreateBooking
        var actualException =
            Assert.ThrowsException<InvalidOperationException>(
                () => service.CreateBooking(request));

        // проверка: проверяем, что выброшен тот же объект исключения
        Assert.AreSame(expectedException, actualException);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(1, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(2, eventRepository.ReleasedSeats);
        Assert.AreEqual(20, eventRepository.AvailableSeats);

        CollectionAssert.AreEqual(
            new[] { "Reserve", "Release" },
            eventRepository.Operations);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        // при исключении запись о бронировании сохраняться не должна
        Assert.AreEqual(0, bookingRepository.SaveCalls);
    }

    // проверяет отклонение количества мест вне диапазона от 1 до 10 и отсутствие внешних действий
    // seatsCount — недопустимое количество мест из DataRow
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(11)]
    [DataRow(100)]
    public void CreateBooking_InvalidSeatsCount_ThrowsException(
        int seatsCount)
    {
        // подготовка: формируем запрос с недопустимым количеством мест
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, 500m);

        // выполнение: ожидаем исключение проверки диапазона
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // проверка: проверяем, что внешние зависимости не вызывались
        AssertNoExternalActions();
    }

    // проверяет ошибку при отсутствующем мероприятии: остаток не запрашивается, бронь не создаётся
    [TestMethod]
    public void CreateBooking_EventDoesNotExist_ThrowsException()
    {
        // подготовка: имитируем отсутствующее мероприятие
        eventRepository.EventExists = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // выполнение: ожидаем исключение отсутствия мероприятия
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // проверка: проверяем, что проверка наличия мест не выполнялась
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    // проверяет ошибку при недостатке мест: платёж и резервирование не выполняются
    [TestMethod]
    public void CreateBooking_NotEnoughSeats_ThrowsException()
    {
        // подготовка: доступно одно место, запрашиваются три
        eventRepository.AvailableSeats = 1;

        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // выполнение: ожидаем исключение недостаточного остатка
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // проверка: проверяем обращения к репозиторию и отсутствие действий по бронированию
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(1, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    // проверяет вычисление итоговой суммы и передачу этой суммы в платёжный сервис для нескольких наборов данных
    // seatsCount — количество мест
    // pricePerSeat — цена одного места
    // expectedTotal — ожидаемая итоговая стоимость
    [DataTestMethod]
    [DataRow(1, 100, 100)]
    [DataRow(2, 500, 1000)]
    [DataRow(3, 1250, 3750)]
    public void CreateBooking_ValidPrices_CalculatesTotalAmount(
        int seatsCount,
        int pricePerSeat,
        int expectedTotal)
    {
        // подготовка: используем очередной набор параметров DataRow
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, pricePerSeat);

        // выполнение: создаём бронь с заданной ценой и количеством
        Booking booking = service.CreateBooking(request);

        // проверка: сравниваем ожидаемую сумму с записью брони и суммой списания
        Assert.AreEqual(
            (decimal)expectedTotal, booking.TotalAmount);

        Assert.AreEqual(
            (decimal)expectedTotal, paymentService.ChargedAmount);
    }

    // проверяет ArgumentNullException при передаче null вместо запроса и отсутствие внешних действий
    [TestMethod]
    public void CreateBooking_NullRequest_ThrowsException()
    {
        // подготовка: передаём отсутствующий объект запроса
        BookingRequest request = null!;

        // выполнение: ожидаем ArgumentNullException
        Assert.ThrowsException<ArgumentNullException>(
            () => service.CreateBooking(request));

        // проверка: проверяем отсутствие внешних вызовов
        AssertNoExternalActions();
    }

    // проверяет отклонение нулевого или отрицательного ID мероприятия
    // eventId — недопустимый идентификатор из DataRow
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void CreateBooking_InvalidEventId_ThrowsException(
        int eventId)
    {
        // подготовка: используем некорректный ID из DataRow
        var request = new BookingRequest(
            eventId, "customer@mail.ru", 2, 500m);

        // выполнение: ожидаем исключение проверки диапазона ID
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // проверка: проверяем отсутствие внешних вызовов
        AssertNoExternalActions();
    }

    // проверяет отклонение пустой строки, пробела и адреса без символа @
    // email — некорректный адрес из DataRow
    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("invalid-email")]
    public void CreateBooking_InvalidEmail_ThrowsException(
        string email)
    {
        // подготовка: подставляем очередной некорректный email
        var request = new BookingRequest(
            1, email, 2, 500m);

        // выполнение: ожидаем ArgumentException
        Assert.ThrowsException<ArgumentException>(
            () => service.CreateBooking(request));

        // проверка: проверяем отсутствие внешних вызовов
        AssertNoExternalActions();
    }

    // проверяет отклонение нулевой и отрицательной цены за место
    // price — недопустимая цена из DataRow
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-100)]
    public void CreateBooking_InvalidPrice_ThrowsException(
        int price)
    {
        // подготовка: подставляем недопустимую цену из DataRow
        var request = new BookingRequest(
            1, "customer@mail.ru", 2, price);

        // выполнение: ожидаем исключение проверки диапазона цены
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // проверка: проверяем отсутствие внешних вызовов
        AssertNoExternalActions();
    }

    // вспомогательная проверка отсутствия резервирования, освобождения, платежей, сохранения и получения времени или ID
    private void AssertNoBookingActions()
    {
        Assert.AreEqual(0, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(0, paymentService.ChargeCalls);
        Assert.AreEqual(0, bookingRepository.SaveCalls);
        Assert.AreEqual(0, clock.Calls);
        Assert.AreEqual(0, idGenerator.Calls);
    }

    // вспомогательная проверка полного отсутствия обращений к зависимостям, включая проверку мероприятия и остатка
    private void AssertNoExternalActions()
    {
        Assert.AreEqual(0, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }
}
