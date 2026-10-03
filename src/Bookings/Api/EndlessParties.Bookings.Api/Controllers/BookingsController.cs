using EndlessParties.Bookings.Api.App.Features.Cancel;
using EndlessParties.Bookings.Api.App.Features.Create;
using EndlessParties.Bookings.Api.App.Features.GetById;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Bookings.Api.Controllers;

/// <summary>
/// Контроллер для работы с бронированиями
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Authorize]
public class BookingsController : ControllerBase
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Создание бронирования
    /// </summary>
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [HttpPost("{id:guid}/book")]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CreateBookingCommand(id);
        var bookingModel = await _mediator.Send(command, cancellationToken);

        return AcceptedAtAction(nameof(GetById), "Bookings", new { id = bookingModel.Id }, bookingModel);
    }

    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookingByIdQuery(id);
        var bookingModel = await _mediator.Send(query, cancellationToken);

        return Ok(bookingModel);
    }

    /// <summary>
    /// Отмена бронирования по идентификатору
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelBookingCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}