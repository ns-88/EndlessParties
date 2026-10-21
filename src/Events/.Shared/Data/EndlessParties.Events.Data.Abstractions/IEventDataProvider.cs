using EndlessParties.Events.Data.Abstractions.Models;

namespace EndlessParties.Events.Data.Abstractions;

/// <summary>
/// Провайдер данных мероприятий (событий)
/// </summary>
public interface IEventDataProvider
{
    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<EventModel> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Получение 10 самых популярных событий
    /// </summary>
    Task<IReadOnlyList<EventTopModel>> GetTop10MostFilled(CancellationToken cancellationToken);
}