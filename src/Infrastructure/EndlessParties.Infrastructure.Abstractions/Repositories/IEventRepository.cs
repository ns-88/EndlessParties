using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Infrastructure.Abstractions.Repositories;

/// <summary>
/// Репозиторий для работы с мероприятиями (событиями)
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Получение всех событий
    /// </summary>
    Task<CollectionResult<Event>> GetAll(GetAllEventsFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<Event> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Получение события по идентификатору с блокировкой таблицы на уровне строки
    /// </summary>
    Task<Event> GetByIdWithLock(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание события
    /// </summary>
    Task Create(Event model, CancellationToken cancellationToken);

    /// <summary>
    /// Удаление события
    /// </summary>
    Task Remove(Guid id, CancellationToken cancellationToken);
}