using EndlessParties.Events.Domain.Events;
using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Contracts.Cache;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Updated;

/// <summary>
/// Обработчик <see cref="EventUpdatedDomainEvent"/>
/// </summary>
public class UpdatedEventHandler : INotificationHandler<EventUpdatedDomainEvent>
{
    /// <summary>
    /// Клиент <see cref="ICacheClient"/>
    /// </summary>
    private readonly ICacheClient _cacheClient;


    /// <summary>
    /// Конструктор
    /// </summary>
    public UpdatedEventHandler(ICacheClient cacheClient)
    {
        _cacheClient = cacheClient;
    }


    /// <inheritdoc />
    public async ValueTask Handle(EventUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var key = string.Format(EventsKeys.Single, notification.Id);
        await _cacheClient.Remove(key);
    }
}