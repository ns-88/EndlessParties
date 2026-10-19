using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.GetTop10MostFilled;

/// <summary>
/// Обработчик <see cref="GetTop10MostFilledQuery"/>
/// </summary>
public class GetTop10MostFilledHandler : IRequestHandler<GetTop10MostFilledQuery, IReadOnlyList<EventTopModel>>
{
    /// <summary>
    /// Провайдер данных <see cref="IEventDataProvider"/>
    /// </summary>
    private readonly IEventDataProvider _eventDataProvider;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetTop10MostFilledHandler(IEventDataProvider eventDataProvider)
    {
        _eventDataProvider = eventDataProvider;
    }


    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<EventTopModel>> Handle(GetTop10MostFilledQuery request, CancellationToken cancellationToken)
    {
        return await _eventDataProvider.GetTop10MostFilled(cancellationToken);
    }
}