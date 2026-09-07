using EndlessParties.Events.Repositories.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.Remove;

/// <summary>
/// Обработчик <see cref="DeleteEventCommand"/>
/// </summary>
internal class DeleteEventHandler : IRequestHandler<DeleteEventCommand>
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DeleteEventHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        await _eventRepository.Remove(request.Id, cancellationToken);

        return Unit.Value;
    }
}