namespace EndlessParties.Shared.MessageBus.Abstractions;

/// <summary>
/// Публикатор сообщений с типом <typeparamref name="T"/>
/// </summary>
public interface IPublisher<in T>
{
    /// <summary>
    /// Публикация сообщения
    /// </summary>
    bool TryPublish(T message);
}