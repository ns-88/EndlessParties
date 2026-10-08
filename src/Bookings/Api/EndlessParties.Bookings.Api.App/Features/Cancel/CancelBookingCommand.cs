using Mediator;

namespace EndlessParties.Bookings.Api.App.Features.Cancel;

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