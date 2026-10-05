namespace EndlessParties.Bookings.Domain.Enums;

/// <summary>
/// Статус бронирования
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Создана, ожидает обработки
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Подтверждена
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Отклонена
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Отменена
    /// </summary>
    Canceled = 4
}