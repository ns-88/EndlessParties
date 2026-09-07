using EndlessParties.Events.Domain.Models;

namespace EndlessParties.Events.Repositories.Abstractions;

/// <summary>
/// Репозиторий для работы с бронированиями
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    Task<Booking> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание бронирования
    /// </summary>
    Task Create(Booking model, CancellationToken cancellationToken);
}