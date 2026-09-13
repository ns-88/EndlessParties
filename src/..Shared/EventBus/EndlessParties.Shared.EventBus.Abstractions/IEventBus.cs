namespace EndlessParties.Shared.EventBus.Abstractions;

/// <summary>
/// Распределенная шина событий
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Публикация сообщения в шину
    /// </summary>
    Task Publish(object @event, CancellationToken cancellationToken);
}