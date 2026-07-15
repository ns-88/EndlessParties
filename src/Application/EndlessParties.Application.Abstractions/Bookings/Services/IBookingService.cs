using EndlessParties.Application.Abstractions.Bookings.Models.Requests;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;

namespace EndlessParties.Application.Abstractions.Bookings.Services;

/// <summary>
/// Сервис для работы с бронированиями
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    Task<BookingResponse> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание бронирования
    /// </summary>
    Task<BookingResponse> Create(CreateBookingRequest model, CancellationToken cancellationToken);
}