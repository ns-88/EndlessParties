using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Shared.Cache.Abstractions;

namespace EndlessParties.Events.Data;

/// <summary>
/// Декоратор провайдера данных <see cref="EventDataProvider"/>, добавляющий поддержку кэширования
/// </summary>
internal class CachingEventDataProvider : IEventDataProvider
{
    /// <summary>
    /// TTL одиночного события
    /// </summary>
    private readonly TimeSpan _ttlSingleEvent = TimeSpan.FromMinutes(5);

    /// <summary>
    /// TTL списка 10 самых популярных событий
    /// </summary>
    private readonly TimeSpan _ttlTop10MostFilled = TimeSpan.FromMinutes(15);

    /// <summary>
    /// TTL списка 10 самых популярных событий (если список пуст)
    /// </summary>
    private readonly TimeSpan _ttlTop10MostFilledEmpty = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Внутренний провайдер данных для работы с событиями
    /// </summary>
    private readonly EventDataProvider _innerDataProvider;

    /// <summary>
    /// Клиент <see cref="ICacheClient"/>
    /// </summary>
    private readonly ICacheClient _cacheClient;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CachingEventDataProvider(EventDataProvider innerDataProvider, ICacheClient cacheClient)
    {
        _innerDataProvider = innerDataProvider;
        _cacheClient = cacheClient;
    }


    /// <inheritdoc />
    public async Task<EventModel> GetById(Guid id, CancellationToken cancellationToken)
    {
        var key = $"events:{id}";
        var @event = await _cacheClient.Get<EventModel>(key);
        if (@event != null)
        {
            return @event;
        }

        @event = await _innerDataProvider.GetById(id, cancellationToken);
        await _cacheClient.Set(key, @event, _ttlSingleEvent);

        return @event;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EventTopModel>> GetTop10MostFilled(CancellationToken cancellationToken)
    {
        const string key = "events:top10";
        var events = await _cacheClient.Get<IReadOnlyList<EventTopModel>>(key);
        if (events != null)
        {
            return events;
        }

        events = await _innerDataProvider.GetTop10MostFilled(cancellationToken);
        if (events.Count != 0)
        {
            await _cacheClient.Set(key, events, _ttlTop10MostFilled);
        }
        else
        {
            await _cacheClient.Set(key, events, _ttlTop10MostFilledEmpty);
        }

        return events;
    }
}