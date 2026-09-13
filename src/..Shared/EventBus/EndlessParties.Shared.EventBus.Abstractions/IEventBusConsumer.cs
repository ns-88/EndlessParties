namespace EndlessParties.Shared.EventBus.Abstractions;

/// <summary>
/// Потребитель данных в шине событий
/// </summary>
public interface IEventBusConsumer<in T> where T : class
{
    /// <summary>
    /// Обработка события
    /// </summary>
    Task Consume(T @event, CancellationToken cancellationToken);
}