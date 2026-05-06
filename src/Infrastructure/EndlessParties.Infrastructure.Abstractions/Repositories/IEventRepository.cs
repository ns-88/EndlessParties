using EndlessParties.Domain.Models;

namespace EndlessParties.Infrastructure.Abstractions.Repositories;

/// <summary>
/// Репозиторий для работы с мероприятиями (событиями)
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Получение всех событий
    /// </summary>
    Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken);

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<Event> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание события
    /// </summary>
    Task Create(Event model, CancellationToken cancellationToken);

    /// <summary>
    /// Обновление события
    /// </summary>
    Task Update(Guid id, Event model, CancellationToken cancellationToken);

    /// <summary>
    /// Удаление события
    /// </summary>
    Task Remove(Guid id, CancellationToken cancellationToken);
}