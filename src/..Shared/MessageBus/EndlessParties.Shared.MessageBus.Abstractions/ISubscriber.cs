namespace EndlessParties.Shared.MessageBus.Abstractions;

/// <summary>
/// Подписчик на сообщения с типом <typeparamref name="T"/>
/// </summary>
public interface ISubscriber<T>
{
    /// <summary>
    /// Получение сообщения
    /// </summary>
    Task<T> Read(CancellationToken cancellationToken);
}