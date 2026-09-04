using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Запрос получения списка событий с возможностью применения фильтров
/// </summary>
public class GetAllEventsQuery(EventSearchFilter filter) : IRequest<EventPaginatedResponse>
{
    /// <summary>
    /// Фильтр запроса
    /// </summary>
    public EventSearchFilter Filter { get; } = filter;
}