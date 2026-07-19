using EndlessParties.Domain.Models;

namespace EndlessParties.Infrastructure.Abstractions.Repositories;

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

    /// <summary>
    /// Обновление бронирования
    /// </summary>
    Task Update(Guid id, Booking model, CancellationToken cancellationToken);
}