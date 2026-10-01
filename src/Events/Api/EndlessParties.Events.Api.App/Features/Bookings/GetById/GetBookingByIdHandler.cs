using EndlessParties.Events.Api.App.Features.Bookings.Create;
using EndlessParties.Events.Api.App.Features.Bookings.Mappers;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.GetById;

/// <summary>
/// Обработчик <see cref="GetBookingByIdQuery"/>
/// </summary>
public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, BookingResponse>
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;

    /// <summary>
    /// Сервис <see cref="IUserContextAccessor"/>
    /// </summary>
    private readonly IUserContextAccessor _userContextAccessor;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetBookingByIdHandler(IBookingRepository bookingRepository, IUserContextAccessor userContextAccessor)
    {
        _bookingRepository = bookingRepository;
        _userContextAccessor = userContextAccessor;
    }


    /// <inheritdoc />
    public async ValueTask<BookingResponse> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var user = _userContextAccessor.Current;
        var booking = await _bookingRepository.GetById(request.Id, cancellationToken);

        if (booking.UserId != user.Id && user.Role != UserRole.Admin)
        {
            throw new ForbiddenException(ApplicationErrors.Bookings.NotPossibleReceivingBookingFromAnotherUser);
        }

        return BookingMapper.Map(booking);
    }
}