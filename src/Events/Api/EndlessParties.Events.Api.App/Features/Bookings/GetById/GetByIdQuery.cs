using EndlessParties.Events.Api.App.Features.Bookings.Create;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.GetById;

/// <summary>
/// Запрос получения бронирования по идентификатору
/// </summary>
public class GetByIdQuery(Guid id) : IRequest<BookingResponse>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}