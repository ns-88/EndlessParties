using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Shared.Contracts;
using EndlessParties.Shared.Contracts.Models;
using Xunit;

namespace EndlessParties.Tests.Infrastructure.Repositories;

/// <summary>
/// Тестовые данные для теста получения списка событий
/// </summary>
public sealed class EventFilterTestData : TheoryData<EventFilterTestCase>
{
    /// <summary>
    /// Событие 'Литературный вечер'
    /// </summary>
    private readonly Event _eventLiteraryEvening = new(
        "Литературный вечер 'Поэзия серебряного века",
        "Чтение стихов Ахматовой и Гумилева под аккомпанемент скрипки.",
        new DateTimeOffset(new DateTime(2024, 06, 17, 19, 00, 00), TimeSpan.Zero),
        new DateTimeOffset(new DateTime(2024, 06, 17, 21, 00, 00), TimeSpan.Zero));

    /// <summary>
    /// Событие 'Общегородской субботник'
    /// </summary>
    private readonly Event _eventCitywideCleanDay = new(
        "Общегородской субботник",
        "Уборка территории набережной и высадка молодых саженцев.",
        new DateTimeOffset(new DateTime(2024, 07, 20, 09, 00, 00), TimeSpan.Zero),
        new DateTimeOffset(new DateTime(2024, 07, 20, 14, 00, 00), TimeSpan.Zero));


    /// <inheritdoc />
    public EventFilterTestData()
    {
        #region Наименование
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = "вечер",
                Description = null,
                From = null,
                To = null,
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(1, [_eventLiteraryEvening])
        });
        #endregion

        #region Описание
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = "уборка",
                From = null,
                To = null,
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(1, [_eventCitywideCleanDay])
        });
        #endregion

        #region Дата и время начала
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = new DateTimeOffset(new DateTime(2024, 07, 20, 08, 00, 00), TimeSpan.Zero),
                To = null,
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(1, [_eventCitywideCleanDay])
        });
        #endregion

        #region Дата и время завершения
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = null,
                To = new DateTimeOffset(new DateTime(2024, 06, 17, 22, 00, 00), TimeSpan.Zero),
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(1, [_eventLiteraryEvening])
        });
        #endregion

        #region Сортировка
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = null,
                To = null,
                Count = 2,
                Offset = 0,
                SortDirection = SortDirection.Ascending
            },
            CollectionResult = new CollectionResult<Event>(2,
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ])
        });

        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = null,
                To = null,
                Count = 2,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(2,
            [
                _eventCitywideCleanDay,
                _eventLiteraryEvening
            ])
        });
        #endregion

        #region Пагинация
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = null,
                To = null,
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(2, [_eventCitywideCleanDay])
        });

        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = null,
                Description = null,
                From = null,
                To = null,
                Count = 1,
                Offset = 1,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(2, [_eventLiteraryEvening])
        });
        #endregion

        #region Комбинирование фильтров
        Add(new EventFilterTestCase
        {
            Events =
            [
                _eventLiteraryEvening,
                _eventCitywideCleanDay
            ],
            Filter = new GetAllEventsFilter
            {
                Title = "вечер",
                Description = "стих",
                From = new DateTimeOffset(new DateTime(2024, 06, 17, 15, 00, 00), TimeSpan.Zero),
                To = new DateTimeOffset(new DateTime(2024, 06, 17, 22, 00, 00), TimeSpan.Zero),
                Count = 1,
                Offset = 0,
                SortDirection = SortDirection.Descending
            },
            CollectionResult = new CollectionResult<Event>(1, [_eventLiteraryEvening])
        });
        #endregion
    }
}