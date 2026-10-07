using EndlessParties.Events.Api.App.Features.Create;
using EndlessParties.Events.Api.App.Features.Events.GetAll;
using EndlessParties.Events.Api.App.Features.GetAll;
using EndlessParties.Events.Api.App.Features.GetById;
using EndlessParties.Events.Api.App.Features.Remove;
using EndlessParties.Events.Api.App.Features.Shared;
using EndlessParties.Events.Api.App.Features.Update;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Events.Api.Controllers;

/// <summary>
/// Контроллер для работы с мероприятиями (событиями)
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Authorize]
public class EventsController : ControllerBase
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Получение всех событий
    /// </summary>
    [ProducesResponseType(typeof(EventPaginatedResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<EventPaginatedResponse>> GetAll([FromQuery] EventSearchFilter filter, CancellationToken cancellationToken)
    {
        var query = new GetAllEventsQuery(filter);
        var eventPaginated = await _mediator.Send(query, cancellationToken);

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
        var query = new GetEventByIdQuery(id);
        var eventModel = await _mediator.Send(query, cancellationToken);

        return Ok(eventModel);
    }

    /// <summary>
    /// Создание события
    /// </summary>
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EventResponse>> Create([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand(request);
        var eventModel = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = eventModel.Id }, eventModel);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand(id, request);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Удаление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remove([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteEventCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}