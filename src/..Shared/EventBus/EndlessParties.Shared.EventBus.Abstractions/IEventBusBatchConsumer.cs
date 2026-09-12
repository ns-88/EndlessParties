namespace EndlessParties.Shared.EventBus.Abstractions;

/// <summary>
/// Потребитель данных в шине событий с поддержкой батчинга
/// </summary>
public interface IEventBusBatchConsumer<in T>
{
    /// <summary>
    /// Обработка списка события
    /// </summary>
    Task Consume(IReadOnlyList<T> events, CancellationToken cancellationToken);
}