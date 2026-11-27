
using System;

namespace BookingSystem;

public enum BookingStatus
{
    Confirmed,
    PaymentFailed
}

public sealed record BookingRequest(
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal PricePerSeat);

public sealed record Booking(
    Guid Id,
    int EventId,
    string CustomerEmail,
    int SeatsCount,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    BookingStatus Status);

public interface IEventRepository
{
    bool Exists(int eventId);

    int GetAvailableSeats(int eventId);

    void ReserveSeats(int eventId, int seatsCount);

    void ReleaseSeats(int eventId, int seatsCount);
}

public interface IBookingRepository
{
    void Save(Booking booking);
}

public interface IPaymentService
{
    bool Charge(string customerEmail, decimal amount);
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public interface IBookingIdGenerator
{
    Guid Create();
}

public sealed class BookingService
{
    private readonly IEventRepository eventRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly IPaymentService paymentService;
    private readonly IClock clock;
    private readonly IBookingIdGenerator idGenerator;

    public BookingService(
        IEventRepository eventRepository,
        IBookingRepository bookingRepository,
        IPaymentService paymentService,
        IClock clock,
        IBookingIdGenerator idGenerator)
    {
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

    public Booking CreateBooking(BookingRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.EventId <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.EventId));

        if (string.IsNullOrWhiteSpace(request.CustomerEmail)
            || !request.CustomerEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Некорректный email.",
                nameof(request.CustomerEmail));
        }

        if (request.SeatsCount < 1 || request.SeatsCount > 10)
            throw new ArgumentOutOfRangeException(nameof(request.SeatsCount));

        if (request.PricePerSeat <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.PricePerSeat));

        if (!eventRepository.Exists(request.EventId))
            throw new InvalidOperationException(
                "Мероприятие не существует.");

        if (eventRepository.GetAvailableSeats(request.EventId)
            < request.SeatsCount)
        {
            throw new InvalidOperationException(
                "Недостаточно свободных мест.");
        }

        decimal totalAmount =
            request.SeatsCount * request.PricePerSeat;

        Guid id = idGenerator.Create();
        DateTimeOffset createdAt = clock.UtcNow;

        eventRepository.ReserveSeats(
            request.EventId,
            request.SeatsCount);

        bool paymentSucceeded;

        try
        {
            paymentSucceeded = paymentService.Charge(
                request.CustomerEmail,
                totalAmount);
        }
        catch
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);

            throw;
        }

        if (!paymentSucceeded)
        {
            eventRepository.ReleaseSeats(
                request.EventId,
                request.SeatsCount);
        }

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

        bookingRepository.Save(booking);

        return booking;
    }
}
