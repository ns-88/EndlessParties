using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;

namespace EndlessParties.Application.Abstractions.Events.Services;

/// <summary>
/// Сервис для работы с мероприятиями (событиями)
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получение всех событий
    /// </summary>
    Task<EventPaginatedResponse> GetAll(GetAllEventsQueryFilter filter, CancellationToken cancellationToken);

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<EventResponse> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание события
    /// </summary>
    Task<EventResponse> Create(CreateEventRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Обновление события
    /// </summary>
    Task Update(Guid id, CreateEventRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаление события
    /// </summary>
    Task Remove(Guid id, CancellationToken cancellationToken);
}