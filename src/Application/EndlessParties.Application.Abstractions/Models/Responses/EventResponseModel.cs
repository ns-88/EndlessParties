using EndlessParties.Application.Abstractions.Models.Requests;

namespace EndlessParties.Application.Abstractions.Models.Responses;

/// <summary>
/// Модель чтения данных мероприятия (события)
/// </summary>
public class EventResponseModel : EventRequestModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; init; }
}