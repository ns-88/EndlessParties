using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Repositories;

namespace EndlessParties.Tests.Infrastructure.Extensions;

/// <summary>
/// Набор методов-расширений для типа <see cref="EventRepository"/>
/// </summary>
internal static class EventRepositoryExtensions
{
    extension(EventRepository)
    {
        /// <summary>
        /// Создание репозитория со списком событий <see cref="Event"/>
        /// </summary>
        public static EventRepository FromData(IReadOnlyList<Event> events)
        {
            var eventRepository = new EventRepository();

            FillRepository().Wait();

            return eventRepository;

            async Task FillRepository()
            {
                foreach (var @event in events)
                {
                    await eventRepository.Create(@event, CancellationToken.None);
                }
            }
        }
    }
}