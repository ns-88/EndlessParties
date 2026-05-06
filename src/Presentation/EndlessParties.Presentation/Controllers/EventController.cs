using EndlessParties.Application.Abstractions.Models.Requests;
using EndlessParties.Application.Abstractions.Models.Responses;
using EndlessParties.Application.Abstractions.Services;
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
    /// Конструктор
    /// </summary>
    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }


    /// <summary>
    /// Получение всех событий
    /// </summary>
    [ProducesResponseType(typeof(IReadOnlyList<EventResponseModel>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventResponseModel>>> GetAll(CancellationToken cancellationToken)
    {
        var eventModelList = await _eventService.GetAll(cancellationToken);

        return Ok(eventModelList);
    }

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    [ProducesResponseType(typeof(EventResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventResponseModel>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var eventModel = await _eventService.GetById(id, cancellationToken);

        return Ok(eventModel);
    }

    /// <summary>
    /// Создание события
    /// </summary>
    [ProducesResponseType(typeof(EventResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<EventResponseModel>> Create([FromBody] EventRequestModel model, CancellationToken cancellationToken)
    {
        var eventModel = await _eventService.Create(model, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = eventModel.Id }, eventModel);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EventRequestModel model, CancellationToken cancellationToken)
    {
        await _eventService.Update(id, model, cancellationToken);

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