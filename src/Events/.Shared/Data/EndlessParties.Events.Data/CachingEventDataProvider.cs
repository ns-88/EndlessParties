using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Contracts.Cache;

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
    private readonly IEventDataProvider _innerDataProvider;

    /// <summary>
    /// Клиент <see cref="ICacheClient"/>
    /// </summary>
    private readonly ICacheClient _cacheClient;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CachingEventDataProvider(IEventDataProvider innerDataProvider, ICacheClient cacheClient)
    {
        _innerDataProvider = innerDataProvider;
        _cacheClient = cacheClient;
    }


    /// <inheritdoc />
    public async Task<EventModel> GetById(Guid id, CancellationToken cancellationToken)
    {
        var key = string.Format(EventsKeys.Single, id);
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
        var events = await _cacheClient.Get<IReadOnlyList<EventTopModel>>(EventsKeys.Top10MostFilled);
        if (events != null)
        {
            return events;
        }

        events = await _innerDataProvider.GetTop10MostFilled(cancellationToken);
        if (events.Count != 0)
        {
            await _cacheClient.Set(EventsKeys.Top10MostFilled, events, _ttlTop10MostFilled);
        }
        else
        {
            await _cacheClient.Set(EventsKeys.Top10MostFilled, events, _ttlTop10MostFilledEmpty);
        }

        return events;
    }
}