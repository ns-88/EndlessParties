using EndlessParties.Events.Api.App.Features.Shared;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Events.Api.App.Features.GetAll;

/// <summary>
/// Cписок мероприятий (событий) с поддержкой пагинации
/// </summary>
public class EventPaginatedResponse : PaginatedResponse<EventResponse>;