using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.Cancel;

/// <summary>
/// Команда отмены бронирования
/// </summary>
public class CancelBookingCommand(Guid id) : IRequest
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}