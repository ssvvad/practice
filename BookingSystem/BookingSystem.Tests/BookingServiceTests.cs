
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookingSystem;

namespace BookingSystem.Tests;

/// <summary>
/// Тестовая реализация репозитория мероприятий: имитирует наличие мест и фиксирует обращения.
/// </summary>
public sealed class FakeEventRepository : IEventRepository
{
    /// <summary>
    /// Настраиваемый признак существования мероприятия.
    /// </summary>
    public bool EventExists { get; set; } = true;
    /// <summary>
    /// Текущее количество свободных мест в имитации репозитория.
    /// </summary>
    public int AvailableSeats { get; set; } = 20;

    /// <summary>
    /// Число вызовов Exists.
    /// </summary>
    public int ExistsCalls { get; private set; }
    /// <summary>
    /// Число вызовов GetAvailableSeats.
    /// </summary>
    public int GetAvailableSeatsCalls { get; private set; }
    /// <summary>
    /// Число вызовов ReserveSeats.
    /// </summary>
    public int ReserveSeatsCalls { get; private set; }
    /// <summary>
    /// Число вызовов ReleaseSeats.
    /// </summary>
    public int ReleaseSeatsCalls { get; private set; }

    /// <summary>
    /// Суммарное количество зарезервированных мест.
    /// </summary>
    public int ReservedSeats { get; private set; }
    /// <summary>
    /// Суммарное количество освобождённых мест.
    /// </summary>
    public int ReleasedSeats { get; private set; }

    /// <summary>
    /// Журнал порядка операций резервирования и освобождения.
    /// </summary>
    public List<string> Operations { get; } = new();

    /// <summary>
    /// Фиксирует проверку существования и возвращает настроенное значение.
    /// </summary>
    /// <param name="eventId">ID мероприятия; в тестовой реализации не влияет на результат.</param>
    /// <returns>Настроенный признак EventExists.</returns>
    public bool Exists(int eventId)
    // Счётчик позволяет проверить количество обращений сервиса.
    {
        ExistsCalls++;
        return EventExists;
    }

    /// <summary>
    /// Фиксирует запрос остатка мест.
    /// </summary>
    /// <param name="eventId">ID мероприятия; в тестовой реализации не влияет на результат.</param>
    /// <returns>Текущий остаток свободных мест.</returns>
    public int GetAvailableSeats(int eventId)
    {
        GetAvailableSeatsCalls++;
        return AvailableSeats;
    }

    /// <summary>
    /// Имитирует резервирование, уменьшает остаток и записывает операцию.
    /// </summary>
    /// <param name="eventId">ID мероприятия; в имитации не используется.</param>
    /// <param name="seatsCount">Количество резервируемых мест.</param>
    public void ReserveSeats(int eventId, int seatsCount)
    // Обновляем счётчики и уменьшаем доступный остаток.
    {
        ReserveSeatsCalls++;
        ReservedSeats += seatsCount;
        AvailableSeats -= seatsCount;

        Operations.Add("Reserve");
    }

    /// <summary>
    /// Имитирует освобождение, увеличивает остаток и записывает операцию.
    /// </summary>
    /// <param name="eventId">ID мероприятия; в имитации не используется.</param>
    /// <param name="seatsCount">Количество освобождаемых мест.</param>
    public void ReleaseSeats(int eventId, int seatsCount)
    // Обновляем счётчики и восстанавливаем доступный остаток.
    {
        ReleaseSeatsCalls++;
        ReleasedSeats += seatsCount;
        AvailableSeats += seatsCount;

        Operations.Add("Release");
    }
}

/// <summary>
/// Тестовый репозиторий бронирований, хранящий записи в памяти.
/// </summary>
public sealed class FakeBookingRepository : IBookingRepository
{
    /// <summary>
    /// Список записей, переданных на сохранение.
    /// </summary>
    public List<Booking> SavedBookings { get; } = new();

    /// <summary>
    /// Число сохранённых записей (число вызовов Save).
    /// </summary>
    public int SaveCalls => SavedBookings.Count;

    /// <summary>
    /// Добавляет бронирование в тестовый список.
    /// </summary>
    /// <param name="booking">Запись для сохранения.</param>
    public void Save(Booking booking)
    {
        SavedBookings.Add(booking);
    }
}

/// <summary>
/// Тестовая реализация оплаты с управляемым результатом или исключением.
/// </summary>
public sealed class FakePaymentService : IPaymentService
{
    /// <summary>
    /// Настраиваемый результат оплаты при отсутствии исключения.
    /// </summary>
    public bool Result { get; set; } = true;

    /// <summary>
    /// Исключение, которое следует выбросить при оплате; null означает обычную обработку.
    /// </summary>
    public Exception? ExceptionToThrow { get; set; }

    /// <summary>
    /// Число попыток оплаты.
    /// </summary>
    public int ChargeCalls { get; private set; }

    /// <summary>
    /// Сумма последней попытки оплаты.
    /// </summary>
    public decimal ChargedAmount { get; private set; }

    /// <summary>
    /// Почта клиента при последней попытке оплаты.
    /// </summary>
    public string? ChargedEmail { get; private set; }

    /// <summary>
    /// Запоминает параметры платежа и имитирует заданный результат или ошибку.
    /// </summary>
    /// <param name="customerEmail">Почта клиента.</param>
    /// <param name="amount">Сумма списания.</param>
    /// <returns>Настроенный результат оплаты, если исключение не задано.</returns>
    public bool Charge(string customerEmail, decimal amount)
    {
        // Запоминаем фактические аргументы, даже если оплата выбросит исключение.
        ChargeCalls++;
        ChargedEmail = customerEmail;
        ChargedAmount = amount;

        // Имитируем исключение платёжного сервиса по настройке теста.
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;

        return Result;
    }
}

/// <summary>
/// Тестовый источник времени, возвращающий предсказуемое значение.
/// </summary>
public sealed class FakeClock : IClock
{
    /// <summary>
    /// Фиксированная дата и время UTC для воспроизводимости тестов.
    /// </summary>
    public DateTimeOffset FixedTime { get; set; } =
        new DateTimeOffset(
            2026, 11, 27, 10, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Число обращений к свойству UtcNow.
    /// </summary>
    public int Calls { get; private set; }

    /// <summary>
    /// Возвращает фиксированное время и увеличивает счётчик обращений.
    /// </summary>
    public DateTimeOffset UtcNow
    {
        get
        {
            // Считаем обращения и возвращаем заранее известное время.
            Calls++;
            return FixedTime;
        }
    }
}

/// <summary>
/// Тестовый генератор идентификаторов, возвращающий фиксированный Guid.
/// </summary>
public sealed class FakeBookingIdGenerator : IBookingIdGenerator
{
    /// <summary>
    /// Предсказуемый идентификатор, используемый в тестах.
    /// </summary>
    public Guid FixedId { get; set; } =
        Guid.Parse("12345678-1234-1234-1234-123456789012");

    /// <summary>
    /// Число вызовов генератора.
    /// </summary>
    public int Calls { get; private set; }

    /// <summary>
    /// Фиксирует вызов и возвращает заданный идентификатор.
    /// </summary>
    /// <returns>Значение FixedId.</returns>
    public Guid Create()
    {
        // Считаем вызовы генератора.
        Calls++;
        return FixedId;
    }
}

/// <summary>
/// Набор модульных тестов BookingService на MSTest с подменой внешних зависимостей.
/// </summary>
[TestClass]
public class BookingServiceTests
{
    // Тестовые зависимости и проверяемый сервис; null! подавляет предупреждение до Initialize.
    private FakeEventRepository eventRepository = null!;
    private FakeBookingRepository bookingRepository = null!;
    private FakePaymentService paymentService = null!;
    private FakeClock clock = null!;
    private FakeBookingIdGenerator idGenerator = null!;
    private BookingService service = null!;

    /// <summary>
    /// Перед каждым тестом создаёт новые тестовые зависимости и экземпляр сервиса, чтобы сценарии не влияли друг на друга.
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        // Создаём чистые имитации для независимости каждого теста.
        eventRepository = new FakeEventRepository();
        bookingRepository = new FakeBookingRepository();
        paymentService = new FakePaymentService();
        clock = new FakeClock();
        idGenerator = new FakeBookingIdGenerator();

        // Внедряем имитации вместо настоящих хранилищ, часов и платёжного сервиса.
        service = new BookingService(
            eventRepository,
            bookingRepository,
            paymentService,
            clock,
            idGenerator);
    }

    /// <summary>
    /// Проверяет успешную бронь: статус, сумму, реквизиты, резервирование, оплату, сохранение, время и ID.
    /// </summary>
    [TestMethod]
    public void CreateBooking_ValidRequest_ReturnsConfirmedBooking()
    {
        // Arrange — подготовка корректного запроса на три места.
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // Act — вызов проверяемого метода.
        // Act
        Booking booking = service.CreateBooking(request);

        // Assert — сравнение реквизитов и статуса результата с ожидаемыми.
        // Assert
        Assert.AreEqual(idGenerator.FixedId, booking.Id);
        Assert.AreEqual(clock.FixedTime, booking.CreatedAt);
        Assert.AreEqual(request.EventId, booking.EventId);
        Assert.AreEqual(request.CustomerEmail, booking.CustomerEmail);
        Assert.AreEqual(3, booking.SeatsCount);
        Assert.AreEqual(1500m, booking.TotalAmount);
        Assert.AreEqual(BookingStatus.Confirmed, booking.Status);

        // Проверяем, что резервирование выполнено ровно один раз и места не возвращены.
        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(3, eventRepository.ReservedSeats);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(17, eventRepository.AvailableSeats);

        // Проверяем параметры единственного платежа.
        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1500m, paymentService.ChargedAmount);
        Assert.AreEqual(request.CustomerEmail, paymentService.ChargedEmail);

        // Проверяем, что запись сохранена ровно один раз.
        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);

        // Проверяем получение времени и идентификатора через зависимости.
        Assert.AreEqual(1, clock.Calls);
        Assert.AreEqual(1, idGenerator.Calls);
    }

    /// <summary>
    /// Проверяет отказ оплаты: места освобождаются, статус PaymentFailed сохраняется, порядок операций корректен.
    /// </summary>
    [TestMethod]
    public void CreateBooking_PaymentFails_ReleasesReservedSeats()
    {
        // Arrange — имитируем отказ оплаты.
        // Arrange
        paymentService.Result = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act — выполняем бронирование, которое завершится отказом оплаты.
        // Act
        Booking booking = service.CreateBooking(request);

        // Assert — проверяем статус, сумму и восстановление остатка.
        // Assert
        Assert.AreEqual(BookingStatus.PaymentFailed, booking.Status);
        Assert.AreEqual(1000m, booking.TotalAmount);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(1, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(2, eventRepository.ReservedSeats);
        Assert.AreEqual(2, eventRepository.ReleasedSeats);
        Assert.AreEqual(20, eventRepository.AvailableSeats);
// Проверяем последовательность: сначала резервирование, затем освобождение.

        CollectionAssert.AreEqual(
            new[] { "Reserve", "Release" },
            eventRepository.Operations);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);
    }

    /// <summary>
    /// Проверяет исключение при оплате: резерв отменяется, исходное исключение передаётся наружу, бронь не сохраняется.
    /// </summary>
    [TestMethod]
    public void CreateBooking_PaymentThrows_ReleasesSeatsAndRethrows()
    {
        // Arrange — задаём конкретный объект исключения от платёжного сервиса.
        // Arrange
        var expectedException = new InvalidOperationException(
            "Ошибка платёжного сервиса.");

        paymentService.ExceptionToThrow = expectedException;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act — ожидаем исключение при вызове CreateBooking.
        // Act
        var actualException =
            Assert.ThrowsException<InvalidOperationException>(
                () => service.CreateBooking(request));

        // Assert — проверяем, что выброшен тот же объект исключения.
        // Assert
        Assert.AreSame(expectedException, actualException);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(1, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(2, eventRepository.ReleasedSeats);
        Assert.AreEqual(20, eventRepository.AvailableSeats);

        CollectionAssert.AreEqual(
            new[] { "Reserve", "Release" },
            eventRepository.Operations);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        // При исключении запись о бронировании сохраняться не должна.
        Assert.AreEqual(0, bookingRepository.SaveCalls);
    }

    /// <summary>
    /// Проверяет отклонение количества мест вне диапазона от 1 до 10 и отсутствие внешних действий.
    /// </summary>
    /// <param name="seatsCount">Недопустимое количество мест из DataRow.</param>
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(11)]
    [DataRow(100)]
    public void CreateBooking_InvalidSeatsCount_ThrowsException(
        int seatsCount)
    {
        // Arrange — формируем запрос с недопустимым количеством мест.
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, 500m);

        // Act — ожидаем исключение проверки диапазона.
        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert — проверяем, что внешние зависимости не вызывались.
        // Assert
        AssertNoExternalActions();
    }

    /// <summary>
    /// Проверяет ошибку при отсутствующем мероприятии: остаток не запрашивается, бронь не создаётся.
    /// </summary>
    [TestMethod]
    public void CreateBooking_EventDoesNotExist_ThrowsException()
    {
        // Arrange — имитируем отсутствующее мероприятие.
        // Arrange
        eventRepository.EventExists = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act — ожидаем исключение отсутствия мероприятия.
        // Act
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // Assert — проверяем, что проверка наличия мест не выполнялась.
        // Assert
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    /// <summary>
    /// Проверяет ошибку при недостатке мест: платёж и резервирование не выполняются.
    /// </summary>
    [TestMethod]
    public void CreateBooking_NotEnoughSeats_ThrowsException()
    {
        // Arrange — доступно одно место, запрашиваются три.
        // Arrange
        eventRepository.AvailableSeats = 1;

        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // Act — ожидаем исключение недостаточного остатка.
        // Act
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // Assert — проверяем обращения к репозиторию и отсутствие действий по бронированию.
        // Assert
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(1, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    /// <summary>
    /// Проверяет вычисление итоговой суммы и передачу этой суммы в платёжный сервис для нескольких наборов данных.
    /// </summary>
    /// <param name="seatsCount">Количество мест.</param>
    /// <param name="pricePerSeat">Цена одного места.</param>
    /// <param name="expectedTotal">Ожидаемая итоговая стоимость.</param>
    [DataTestMethod]
    [DataRow(1, 100, 100)]
    [DataRow(2, 500, 1000)]
    [DataRow(3, 1250, 3750)]
    public void CreateBooking_ValidPrices_CalculatesTotalAmount(
        int seatsCount,
        int pricePerSeat,
        int expectedTotal)
    {
        // Arrange — используем очередной набор параметров DataRow.
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, pricePerSeat);

        // Act — создаём бронь с заданной ценой и количеством.
        // Act
        Booking booking = service.CreateBooking(request);

        // Assert — сравниваем ожидаемую сумму с записью брони и суммой списания.
        // Assert
        Assert.AreEqual(
            (decimal)expectedTotal, booking.TotalAmount);

        Assert.AreEqual(
            (decimal)expectedTotal, paymentService.ChargedAmount);
    }

    /// <summary>
    /// Проверяет ArgumentNullException при передаче null вместо запроса и отсутствие внешних действий.
    /// </summary>
    [TestMethod]
    public void CreateBooking_NullRequest_ThrowsException()
    {
        // Arrange — передаём отсутствующий объект запроса.
        // Arrange
        BookingRequest request = null!;

        // Act — ожидаем ArgumentNullException.
        // Act
        Assert.ThrowsException<ArgumentNullException>(
            () => service.CreateBooking(request));

        // Assert — проверяем отсутствие внешних вызовов.
        // Assert
        AssertNoExternalActions();
    }

    /// <summary>
    /// Проверяет отклонение нулевого или отрицательного ID мероприятия.
    /// </summary>
    /// <param name="eventId">Недопустимый идентификатор из DataRow.</param>
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void CreateBooking_InvalidEventId_ThrowsException(
        int eventId)
    {
        // Arrange — используем некорректный ID из DataRow.
        // Arrange
        var request = new BookingRequest(
            eventId, "customer@mail.ru", 2, 500m);

        // Act — ожидаем исключение проверки диапазона ID.
        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert — проверяем отсутствие внешних вызовов.
        // Assert
        AssertNoExternalActions();
    }

    /// <summary>
    /// Проверяет отклонение пустой строки, пробела и адреса без символа @.
    /// </summary>
    /// <param name="email">Некорректный адрес из DataRow.</param>
    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("invalid-email")]
    public void CreateBooking_InvalidEmail_ThrowsException(
        string email)
    {
        // Arrange — подставляем очередной некорректный email.
        // Arrange
        var request = new BookingRequest(
            1, email, 2, 500m);

        // Act — ожидаем ArgumentException.
        // Act
        Assert.ThrowsException<ArgumentException>(
            () => service.CreateBooking(request));

        // Assert — проверяем отсутствие внешних вызовов.
        // Assert
        AssertNoExternalActions();
    }

    /// <summary>
    /// Проверяет отклонение нулевой и отрицательной цены за место.
    /// </summary>
    /// <param name="price">Недопустимая цена из DataRow.</param>
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-100)]
    public void CreateBooking_InvalidPrice_ThrowsException(
        int price)
    {
        // Arrange — подставляем недопустимую цену из DataRow.
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", 2, price);

        // Act — ожидаем исключение проверки диапазона цены.
        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert — проверяем отсутствие внешних вызовов.
        // Assert
        AssertNoExternalActions();
    }

    /// <summary>
    /// Вспомогательная проверка отсутствия резервирования, освобождения, платежей, сохранения и получения времени или ID.
    /// </summary>
    private void AssertNoBookingActions()
    {
        Assert.AreEqual(0, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(0, paymentService.ChargeCalls);
        Assert.AreEqual(0, bookingRepository.SaveCalls);
        Assert.AreEqual(0, clock.Calls);
        Assert.AreEqual(0, idGenerator.Calls);
    }

    /// <summary>
    /// Вспомогательная проверка полного отсутствия обращений к зависимостям, включая проверку мероприятия и остатка.
    /// </summary>
    private void AssertNoExternalActions()
    {
        Assert.AreEqual(0, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }
}
