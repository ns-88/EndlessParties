using EndlessParties.Application.Abstractions.Models.Requests;
using EndlessParties.Application.Abstractions.Models.Responses;

namespace EndlessParties.Application.Abstractions.Services;

/// <summary>
/// Сервис для работы с мероприятиями (событиями)
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получение всех событий
    /// </summary>
    Task<IReadOnlyList<EventResponseModel>> GetAll(CancellationToken cancellationToken);

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    Task<EventResponseModel> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Создание события
    /// </summary>
    Task<EventResponseModel> Create(EventRequestModel model, CancellationToken cancellationToken);

    /// <summary>
    /// Обновление события
    /// </summary>
    Task Update(Guid id, EventRequestModel model, CancellationToken cancellationToken);

    /// <summary>
    /// Удаление события
    /// </summary>
    Task Remove(Guid id, CancellationToken cancellationToken);
}