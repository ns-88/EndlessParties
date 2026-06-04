using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Application.Abstractions.Models.Responses;

/// <summary>
/// Модель получения данных списка мероприятий (событий) с поддержкой пагинации
/// </summary>
public class EventPaginatedResponseModel : PaginatedResponseModel<EventResponseModel>;