using Mediator;

namespace EndlessParties.Events.Api.App.Features.Reserve;

/// <summary>
/// Команда резервирования свободных мест в мероприятии (событии)
/// </summary>
public class ReserveEventSeatsCommand : IRequest
{
    /// <summary>
    /// Список запросов резервирования свободного места в мероприятии (событии)
    /// </summary>
    public IReadOnlyList<ReserveEventSeatRequest> ReserveRequests { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public ReserveEventSeatsCommand(IReadOnlyList<ReserveEventSeatRequest> reserveRequests)
    {
        ReserveRequests = reserveRequests;
    }
}