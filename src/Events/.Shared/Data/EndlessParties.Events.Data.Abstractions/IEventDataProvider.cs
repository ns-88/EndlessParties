using EndlessParties.Events.Data.Abstractions.Models;

namespace EndlessParties.Events.Data.Abstractions;

/// <summary>
/// Провайдер данных мероприятий
/// </summary>
public interface IEventDataProvider
{
    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<EventModel> GetById(Guid id, CancellationToken cancellationToken);
}