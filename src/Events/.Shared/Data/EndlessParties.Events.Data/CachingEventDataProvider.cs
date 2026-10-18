using System;
using System.Threading;
using System.Threading.Tasks;
using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Events.Domain.Models;
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
}