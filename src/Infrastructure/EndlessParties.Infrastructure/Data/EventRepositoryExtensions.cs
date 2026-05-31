using System.Text.Json;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Repositories;

namespace EndlessParties.Infrastructure.Data;

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
        public static EventRepository FromData()
        {
            var events = JsonSerializer.Deserialize<IReadOnlyList<Event>>(Resource.Events);
            var eventMap = events!.ToDictionary(k => k.Id, v => v);

            return new EventRepository(eventMap);
        }
    }
}