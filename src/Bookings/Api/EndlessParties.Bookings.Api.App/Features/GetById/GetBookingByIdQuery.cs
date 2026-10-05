using EndlessParties.Bookings.Api.App.Features.Create;
using Mediator;

namespace EndlessParties.Bookings.Api.App.Features.GetById;

/// <summary>
/// Запрос получения бронирования по идентификатору
/// </summary>
public class GetBookingByIdQuery(Guid id) : IRequest<BookingResponse>
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}