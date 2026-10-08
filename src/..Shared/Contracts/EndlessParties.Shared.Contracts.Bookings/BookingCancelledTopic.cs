namespace EndlessParties.Shared.Contracts.Bookings;

/// <summary>
/// Данные топика
/// </summary>
public static class BookingCancelledTopic
{
    /// <summary>
    /// Наименование
    /// </summary>
    public const string TopicName = "{0}.endless-parties.bookings.cancelled.v1";

    /// <summary>
    /// Наименование группы
    /// </summary>
    public const string GroupName = "{0}.endless-parties.bookings.cancelled-consumer-group";
}