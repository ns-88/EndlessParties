using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Delete;

/// <summary>
/// Обработчик <see cref="DeleteEventCommand"/>
/// </summary>
public class DeleteEventHandler : IRequestHandler<DeleteEventCommand>
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DeleteEventHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var @event = await _eventRepository.GetById(request.Id, cancellationToken);

            if (@event.IsDeleted) return Unit.Value;
            @event.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken) && ex is not NotFoundException)
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Deletion, request.Id));
        }

        return Unit.Value;
    }
}