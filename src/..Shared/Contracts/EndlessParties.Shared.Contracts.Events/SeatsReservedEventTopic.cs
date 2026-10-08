namespace EndlessParties.Shared.Contracts.Events;

/// <summary>
/// Данные топика
/// </summary>
public class SeatsReservedEventTopic
{
    /// <summary>
    /// Наименование топика
    /// </summary>
    public const string TopicName = "{0}.endless-parties.events.seats-reserved.v1";

    /// <summary>
    /// Наименование группы
    /// </summary>
    public const string GroupName = "{0}.endless-parties.events.seats-reserved-consumer-group";
}