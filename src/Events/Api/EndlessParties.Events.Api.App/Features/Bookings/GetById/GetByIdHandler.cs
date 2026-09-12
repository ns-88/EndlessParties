using EndlessParties.Events.Api.App.Features.Bookings.Create;
using EndlessParties.Events.Api.App.Features.Bookings.Mappers;
using EndlessParties.Events.Repositories.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.GetById;

/// <summary>
/// Обработчик <see cref="GetByIdQuery"/>
/// </summary>
public class GetByIdHandler : IRequestHandler<GetByIdQuery, BookingResponse>
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetByIdHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    /// <inheritdoc />
    public async ValueTask<BookingResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetById(request.Id, cancellationToken);

        return BookingMapper.Map(booking);
    }
}