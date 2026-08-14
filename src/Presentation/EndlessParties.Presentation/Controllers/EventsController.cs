using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Abstractions.Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Presentation.Controllers;

/// <summary>
/// Контроллер для работы с мероприятиями (событиями)
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class EventsController : ControllerBase
{
    /// <summary>
    /// Сервис <see cref="IEventService"/>
    /// </summary>
    private readonly IEventService _eventService;

    /// <summary>
    /// Сервис <see cref="IBookingService"/>
    /// </summary>
    private readonly IBookingService _bookingService;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventsController(IEventService eventService, IBookingService bookingService)
    {
        _eventService = eventService;
        _bookingService = bookingService;
    }


    /// <summary>
    /// Получение всех событий
    /// </summary>
    [ProducesResponseType(typeof(EventPaginatedResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<EventPaginatedResponse>> GetAll([FromQuery] GetAllEventsQueryFilter filter, CancellationToken cancellationToken)
    {
        var eventPaginated = await _eventService.GetAll(filter, cancellationToken);

        return Ok(eventPaginated);
    }

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var eventModel = await _eventService.GetById(id, cancellationToken);

        return Ok(eventModel);
    }

    /// <summary>
    /// Создание события
    /// </summary>
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<EventResponse>> Create([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        var eventModel = await _eventService.Create(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = eventModel.Id }, eventModel);
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
        var bookingModel = await _bookingService.Create(id, cancellationToken);

        return AcceptedAtAction(nameof(BookingsController.GetById), "Bookings", new { id = bookingModel.Id }, bookingModel);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        await _eventService.Update(id, request, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Удаление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _eventService.Remove(id, cancellationToken);

        return NoContent();
    }
}