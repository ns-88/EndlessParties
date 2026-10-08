using Mediator;

namespace EndlessParties.Events.Api.App.Features.ReleaseSeats;

/// <summary>
/// Команда освобождения мест в мероприятии (событии)
/// </summary>
public class ReleaseEventSeatsCommand : IRequest
{
    /// <summary>
    /// Список запросов освобождения свободного места в мероприятии (событии)
    /// </summary>
    public IReadOnlyList<ReleaseEventSeatRequest> ReleaseRequests { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public ReleaseEventSeatsCommand(IReadOnlyList<ReleaseEventSeatRequest> releaseRequests)
    {
        ReleaseRequests = releaseRequests;
    }
}