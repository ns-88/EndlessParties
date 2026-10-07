namespace EndlessParties.Shared.Contracts.Bookings;

/// <summary>
/// Данные топика
/// </summary>
public static class BookingCreatedTopic
{
    /// <summary>
    /// Наименование
    /// </summary>
    public const string TopicName = "{0}.endless-parties.bookings.created.v1";

    /// <summary>
    /// Наименование группы
    /// </summary>
    public const string GroupName = "{0}.endless-parties.bookings.created-consumer-group";
}