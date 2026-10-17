using EndlessParties.Events.Domain.Events;
using EndlessParties.Shared.Cache.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Updated;

/// <summary>
/// Обработчик <see cref="EventUpdatedDomainEvent"/>
/// </summary>
public class UpdatedEventHandlerTest : INotificationHandler<EventUpdatedDomainEvent>
{
    /// <summary>
    /// Клиент <see cref="ICacheClient"/>
    /// </summary>
    private readonly ICacheClient _cacheClient;


    /// <summary>
    /// Конструктор
    /// </summary>
    public UpdatedEventHandlerTest(ICacheClient cacheClient)
    {
        _cacheClient = cacheClient;
    }


    /// <inheritdoc />
    public async ValueTask Handle(EventUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var key = $"events:{notification.Id}";
        await _cacheClient.Remove(key);
    }
}