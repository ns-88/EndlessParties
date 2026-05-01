using EndlessParties.Application.Abstractions.Models.Requests;
using EndlessParties.Application.Abstractions.Models.Responses;
using EndlessParties.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Presentation.Controllers;

/// <summary>
/// Контроллер для работы с мероприятиями (событиями)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventController : ControllerBase
{
    /// <summary>
    /// Сервис <see cref="IEventService"/>
    /// </summary>
    private readonly IEventService _eventService;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }


    /// <summary>
    /// Получение всех событий
    /// </summary>
    [ProducesResponseType(typeof(IReadOnlyList<EventResponseModel>), StatusCodes.Status200OK)]
    [HttpGet]
    public Task<IReadOnlyList<EventResponseModel>> GetAll(CancellationToken cancellationToken)
    {
        return _eventService.GetAll(cancellationToken);
    }

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    [ProducesResponseType(typeof(EventResponseModel), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public Task<EventResponseModel> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return _eventService.GetById(id, cancellationToken);
    }

    /// <summary>
    /// Создание события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public Task Create([FromBody] EventRequestModel model, CancellationToken cancellationToken)
    {
        return _eventService.Create(model, cancellationToken);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut("{id:guid}")]
    public Task Update([FromRoute] Guid id, [FromBody] EventRequestModel model, CancellationToken cancellationToken)
    {
        return _eventService.Update(id, model, cancellationToken);
    }

    /// <summary>
    /// Удаление события
    /// </summary>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("{id:guid}")]
    public Task Remove([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return _eventService.Remove(id, cancellationToken);
    }
}