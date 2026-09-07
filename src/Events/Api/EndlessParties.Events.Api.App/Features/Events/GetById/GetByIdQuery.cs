using EndlessParties.Events.Api.App.Features.Events.Shared;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.GetById;

/// <summary>
/// Запрос получения мероприятия (события) по идентификатору
/// </summary>
public class GetByIdQuery(Guid id) : IRequest<EventResponse>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}