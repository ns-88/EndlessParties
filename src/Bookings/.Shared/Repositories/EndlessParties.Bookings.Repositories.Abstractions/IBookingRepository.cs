using EndlessParties.Bookings.Domain.Models;

namespace EndlessParties.Bookings.Repositories.Abstractions;

/// <summary>
/// Репозиторий для работы с бронированиями
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Создание бронирования
    /// </summary>
    Task Create(Booking model, CancellationToken cancellationToken);

    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    Task<Booking> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Получение числа активных бронирований указанного пользователя
    /// </summary>
    Task<int> GetActiveCountByUserId(Guid id, CancellationToken cancellationToken);
}