using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Данные списка мероприятий (событий) с поддержкой пагинации
/// </summary>
public class EventPaginatedResponse : PaginatedResponse<EventResponse>;