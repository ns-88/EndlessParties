using EndlessParties.Events.Domain.Events;
using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Contracts.Cache;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Deleted;

/// <summary>
/// Обработчик <see cref="EventDeletedDomainEvent"/>
/// </summary>
public class DeletedEventHandler : INotificationHandler<EventDeletedDomainEvent>
{
    /// <summary>
    /// Клиент <see cref="ICacheClient"/>
    /// </summary>
    private readonly ICacheClient _cacheClient;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DeletedEventHandler(ICacheClient cacheClient)
    {
        _cacheClient = cacheClient;
    }


    /// <inheritdoc />
    public async ValueTask Handle(EventDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var key = string.Format(EventsKeys.Single, notification.Id);
        await _cacheClient.Remove(key);
    }
}