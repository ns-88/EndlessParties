using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Application.Bookings.Mappers;

/// <summary>
/// Маппер для объектов <see cref="Booking"/> и <see cref="BookingResponse"/>
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class BookingMapper
{
    /// <summary>
    /// Преобразование из <see cref="Booking"/> в <see cref="BookingResponse"/>
    /// </summary>
    public static partial BookingResponse Map(Booking source);
}