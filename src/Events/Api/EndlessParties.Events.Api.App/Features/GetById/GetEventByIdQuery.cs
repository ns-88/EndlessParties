using EndlessParties.Events.Data.Abstractions.Models;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.GetById;

/// <summary>
/// Запрос получения мероприятия (события) по идентификатору
/// </summary>
public class GetEventByIdQuery(Guid id) : IRequest<EventModel>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}