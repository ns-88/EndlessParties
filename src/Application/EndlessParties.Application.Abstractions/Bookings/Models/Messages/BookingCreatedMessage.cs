namespace EndlessParties.Application.Abstractions.Bookings.Models.Messages;

/// <summary>
/// Сообщение с данными бронирования, подлежащего обработке
/// </summary>
public readonly struct BookingCreatedMessage(Guid id)
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}