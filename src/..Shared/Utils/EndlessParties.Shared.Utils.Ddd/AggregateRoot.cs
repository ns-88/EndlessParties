namespace EndlessParties.Shared.Utils.Ddd;

/// <summary>
/// Базовый класс корня агрегата
/// </summary>
public class AggregateRoot
{
    /// <summary>
    /// Словарь доменных событий
    /// </summary>
    private readonly Dictionary<Type, IDomainEvent> _events;

    /// <summary>
    /// Доменные события
    /// </summary>
    internal IReadOnlyCollection<IDomainEvent> Events => _events.Values;


    /// <summary>
    /// Конструктор
    /// </summary>
    protected AggregateRoot()
    {
        _events = new Dictionary<Type, IDomainEvent>();
    }


    /// <summary>
    /// Публикация доменного события
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _events.TryAdd(domainEvent.GetType(), domainEvent);
    }

    /// <summary>
    /// Очистка списка доменных событий
    /// </summary>
    internal void ClearEvents()
    {
        _events.Clear();
    }
}