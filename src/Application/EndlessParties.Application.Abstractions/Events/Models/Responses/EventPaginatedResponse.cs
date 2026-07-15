using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Application.Abstractions.Events.Models.Responses;

/// <summary>
/// Данные списка мероприятий (событий) с поддержкой пагинации
/// </summary>
public class EventPaginatedResponse : PaginatedResponse<EventResponse>;