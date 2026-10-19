using EndlessParties.Events.Data.Abstractions.Models;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.GetTop10MostFilled;

/// <summary>
/// Команда получения 10 самых популярных событий
/// </summary>
public class GetTop10MostFilledQuery : IRequest<IReadOnlyList<EventTopModel>>;