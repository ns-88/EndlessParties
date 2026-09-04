using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Api.App.Features.Events.Update;

/// <summary>
/// Обработчик <see cref="UpdateEventCommand"/>
/// </summary>
internal class UpdateEventHandler : IRequestHandler<UpdateEventCommand>
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
    public UpdateEventHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateRequest;

        try
        {
            var @event = await _eventRepository.GetById(request.Id, cancellationToken);
            var startAtUtc = updateRequest.StartAt.ToUniversalTime();
            var endAtUtc = updateRequest.EndAt.ToUniversalTime();

            @event.ChangeTitle(updateRequest.Title);
            @event.ChangeDescription(updateRequest.Description);
            @event.ChangeTotalSeats(updateRequest.TotalSeats);
            @event.ChangeStartAndEndAt(startAtUtc, endAtUtc);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(string.Format(ApplicationErrors.Events.ModifiedByAnotherUserOrSystem, request.Id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken) && ex is not NotFoundException)
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Update, request.Id), ex);
        }

        return Unit.Value;
    }
}