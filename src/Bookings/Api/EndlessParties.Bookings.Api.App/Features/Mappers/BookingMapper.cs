using EndlessParties.Bookings.Api.App.Features.Create;
using EndlessParties.Bookings.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Bookings.Api.App.Features.Mappers;

/// <summary>
/// Маппер для объектов <see cref="Booking"/> и <see cref="BookingResponse"/>
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class BookingMapper
{
    /// <summary>
    /// Преобразование из <see cref="Booking"/> в <see cref="BookingResponse"/>
    /// </summary>
    [MapProperty(nameof(Booking.Id), nameof(BookingResponse.BookingId))]
    public static partial BookingResponse Map(Booking source);
}