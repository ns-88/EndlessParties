using System.Data.Common;
using EndlessParties.Shared.Utils.Ddd;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Utils.Database;

/// <summary>
/// Перехватчик EF Core методов сохранения контекста и завершения транзакции
/// </summary>
internal class DomainEventsInterceptor : SaveChangesInterceptor, IDbTransactionInterceptor
{
    /// <summary>
    /// Фабрика <see cref="IServiceScopeFactory"/>
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DomainEventsInterceptor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context == null || eventData.Context.Database.CurrentTransaction != null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        await EventsPublication(eventData.Context, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context == null)
        {
            return;
        }

        await EventsPublication(eventData.Context, cancellationToken);
    }

    /// <summary>
    /// Публикация доменных событий
    /// </summary>
    private async Task EventsPublication(DbContext dbContext, CancellationToken cancellationToken)
    {
        var roots = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.Events.Count != 0)
            .Select(x => x.Entity)
            .ToList();

        var events = roots
            .SelectMany(x => x.Events)
            .ToList();

        if (events.Count == 0)
        {
            return;
        }

        foreach (var root in roots)
        {
            root.ClearEvents();
        }

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        foreach (var @event in events)
        {
            await mediator.Publish(@event, cancellationToken);
        }
    }
}