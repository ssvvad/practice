
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookingSystem;

namespace BookingSystem.Tests;

public sealed class FakeEventRepository : IEventRepository
{
    public bool EventExists { get; set; } = true;
    public int AvailableSeats { get; set; } = 20;

    public int ExistsCalls { get; private set; }
    public int GetAvailableSeatsCalls { get; private set; }
    public int ReserveSeatsCalls { get; private set; }
    public int ReleaseSeatsCalls { get; private set; }

    public int ReservedSeats { get; private set; }
    public int ReleasedSeats { get; private set; }

    public List<string> Operations { get; } = new();

    public bool Exists(int eventId)
    {
        ExistsCalls++;
        return EventExists;
    }

    public int GetAvailableSeats(int eventId)
    {
        GetAvailableSeatsCalls++;
        return AvailableSeats;
    }

    public void ReserveSeats(int eventId, int seatsCount)
    {
        ReserveSeatsCalls++;
        ReservedSeats += seatsCount;
        AvailableSeats -= seatsCount;

        Operations.Add("Reserve");
    }

    public void ReleaseSeats(int eventId, int seatsCount)
    {
        ReleaseSeatsCalls++;
        ReleasedSeats += seatsCount;
        AvailableSeats += seatsCount;

        Operations.Add("Release");
    }
}

public sealed class FakeBookingRepository : IBookingRepository
{
    public List<Booking> SavedBookings { get; } = new();

    public int SaveCalls => SavedBookings.Count;

    public void Save(Booking booking)
    {
        SavedBookings.Add(booking);
    }
}

public sealed class FakePaymentService : IPaymentService
{
    public bool Result { get; set; } = true;

    public Exception? ExceptionToThrow { get; set; }

    public int ChargeCalls { get; private set; }

    public decimal ChargedAmount { get; private set; }

    public string? ChargedEmail { get; private set; }

    public bool Charge(string customerEmail, decimal amount)
    {
        ChargeCalls++;
        ChargedEmail = customerEmail;
        ChargedAmount = amount;

        if (ExceptionToThrow != null)
            throw ExceptionToThrow;

        return Result;
    }
}

public sealed class FakeClock : IClock
{
    public DateTimeOffset FixedTime { get; set; } =
        new DateTimeOffset(
            2026, 11, 27, 10, 0, 0, TimeSpan.Zero);

    public int Calls { get; private set; }

    public DateTimeOffset UtcNow
    {
        get
        {
            Calls++;
            return FixedTime;
        }
    }
}

public sealed class FakeBookingIdGenerator : IBookingIdGenerator
{
    public Guid FixedId { get; set; } =
        Guid.Parse("12345678-1234-1234-1234-123456789012");

    public int Calls { get; private set; }

    public Guid Create()
    {
        Calls++;
        return FixedId;
    }
}

[TestClass]
public class BookingServiceTests
{
    private FakeEventRepository eventRepository = null!;
    private FakeBookingRepository bookingRepository = null!;
    private FakePaymentService paymentService = null!;
    private FakeClock clock = null!;
    private FakeBookingIdGenerator idGenerator = null!;
    private BookingService service = null!;

    [TestInitialize]
    public void Initialize()
    {
        eventRepository = new FakeEventRepository();
        bookingRepository = new FakeBookingRepository();
        paymentService = new FakePaymentService();
        clock = new FakeClock();
        idGenerator = new FakeBookingIdGenerator();

        service = new BookingService(
            eventRepository,
            bookingRepository,
            paymentService,
            clock,
            idGenerator);
    }

    [TestMethod]
    public void CreateBooking_ValidRequest_ReturnsConfirmedBooking()
    {
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // Act
        Booking booking = service.CreateBooking(request);

        // Assert
        Assert.AreEqual(idGenerator.FixedId, booking.Id);
        Assert.AreEqual(clock.FixedTime, booking.CreatedAt);
        Assert.AreEqual(request.EventId, booking.EventId);
        Assert.AreEqual(request.CustomerEmail, booking.CustomerEmail);
        Assert.AreEqual(3, booking.SeatsCount);
        Assert.AreEqual(1500m, booking.TotalAmount);
        Assert.AreEqual(BookingStatus.Confirmed, booking.Status);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(3, eventRepository.ReservedSeats);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(17, eventRepository.AvailableSeats);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1500m, paymentService.ChargedAmount);
        Assert.AreEqual(request.CustomerEmail, paymentService.ChargedEmail);

        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);

        Assert.AreEqual(1, clock.Calls);
        Assert.AreEqual(1, idGenerator.Calls);
    }

    [TestMethod]
    public void CreateBooking_PaymentFails_ReleasesReservedSeats()
    {
        // Arrange
        paymentService.Result = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act
        Booking booking = service.CreateBooking(request);

        // Assert
        Assert.AreEqual(BookingStatus.PaymentFailed, booking.Status);
        Assert.AreEqual(1000m, booking.TotalAmount);

        Assert.AreEqual(1, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(1, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(2, eventRepository.ReservedSeats);
        Assert.AreEqual(2, eventRepository.ReleasedSeats);
        Assert.AreEqual(20, eventRepository.AvailableSeats);

        CollectionAssert.AreEqual(
            new[] { "Reserve", "Release" },
            eventRepository.Operations);

        Assert.AreEqual(1, paymentService.ChargeCalls);
        Assert.AreEqual(1, bookingRepository.SaveCalls);
        Assert.AreEqual(booking, bookingRepository.SavedBookings[0]);
    }

    [TestMethod]
    public void CreateBooking_PaymentThrows_ReleasesSeatsAndRethrows()
    {
        // Arrange
        var expectedException = new InvalidOperationException(
            "Ошибка платёжного сервиса.");

        paymentService.ExceptionToThrow = expectedException;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act
        var actualException =
            Assert.ThrowsException<InvalidOperationException>(
                () => service.CreateBooking(request));

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
        Assert.AreEqual(0, bookingRepository.SaveCalls);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(11)]
    [DataRow(100)]
    public void CreateBooking_InvalidSeatsCount_ThrowsException(
        int seatsCount)
    {
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, 500m);

        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert
        AssertNoExternalActions();
    }

    [TestMethod]
    public void CreateBooking_EventDoesNotExist_ThrowsException()
    {
        // Arrange
        eventRepository.EventExists = false;

        var request = new BookingRequest(
            1, "customer@mail.ru", 2, 500m);

        // Act
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // Assert
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    [TestMethod]
    public void CreateBooking_NotEnoughSeats_ThrowsException()
    {
        // Arrange
        eventRepository.AvailableSeats = 1;

        var request = new BookingRequest(
            1, "customer@mail.ru", 3, 500m);

        // Act
        Assert.ThrowsException<InvalidOperationException>(
            () => service.CreateBooking(request));

        // Assert
        Assert.AreEqual(1, eventRepository.ExistsCalls);
        Assert.AreEqual(1, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }

    [DataTestMethod]
    [DataRow(1, 100, 100)]
    [DataRow(2, 500, 1000)]
    [DataRow(3, 1250, 3750)]
    public void CreateBooking_ValidPrices_CalculatesTotalAmount(
        int seatsCount,
        int pricePerSeat,
        int expectedTotal)
    {
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", seatsCount, pricePerSeat);

        // Act
        Booking booking = service.CreateBooking(request);

        // Assert
        Assert.AreEqual(
            (decimal)expectedTotal, booking.TotalAmount);

        Assert.AreEqual(
            (decimal)expectedTotal, paymentService.ChargedAmount);
    }

    [TestMethod]
    public void CreateBooking_NullRequest_ThrowsException()
    {
        // Arrange
        BookingRequest request = null!;

        // Act
        Assert.ThrowsException<ArgumentNullException>(
            () => service.CreateBooking(request));

        // Assert
        AssertNoExternalActions();
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void CreateBooking_InvalidEventId_ThrowsException(
        int eventId)
    {
        // Arrange
        var request = new BookingRequest(
            eventId, "customer@mail.ru", 2, 500m);

        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert
        AssertNoExternalActions();
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("invalid-email")]
    public void CreateBooking_InvalidEmail_ThrowsException(
        string email)
    {
        // Arrange
        var request = new BookingRequest(
            1, email, 2, 500m);

        // Act
        Assert.ThrowsException<ArgumentException>(
            () => service.CreateBooking(request));

        // Assert
        AssertNoExternalActions();
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-100)]
    public void CreateBooking_InvalidPrice_ThrowsException(
        int price)
    {
        // Arrange
        var request = new BookingRequest(
            1, "customer@mail.ru", 2, price);

        // Act
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => service.CreateBooking(request));

        // Assert
        AssertNoExternalActions();
    }

    private void AssertNoBookingActions()
    {
        Assert.AreEqual(0, eventRepository.ReserveSeatsCalls);
        Assert.AreEqual(0, eventRepository.ReleaseSeatsCalls);
        Assert.AreEqual(0, paymentService.ChargeCalls);
        Assert.AreEqual(0, bookingRepository.SaveCalls);
        Assert.AreEqual(0, clock.Calls);
        Assert.AreEqual(0, idGenerator.Calls);
    }

    private void AssertNoExternalActions()
    {
        Assert.AreEqual(0, eventRepository.ExistsCalls);
        Assert.AreEqual(0, eventRepository.GetAvailableSeatsCalls);

        AssertNoBookingActions();
    }
}
