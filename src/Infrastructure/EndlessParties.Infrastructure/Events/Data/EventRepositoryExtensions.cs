using System.Text.Json;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Events.Repositories;

namespace EndlessParties.Infrastructure.Events.Data;

/// <summary>
/// Набор методов-расширений для типа <see cref="EventRepository"/>
/// </summary>
internal static class EventRepositoryExtensions
{
    extension(EventRepository)
    {
        /// <summary>
        /// Создание репозитория со списком событий <see cref="Event"/> полученных из json файла
        /// </summary>
        public static EventRepository FromData()
        {
            var events = JsonSerializer.Deserialize<IReadOnlyList<Event>>(Resource.Events);

            if (events == null)
            {
                throw new InvalidOperationException("Ошибка получения списка событий");
            }

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