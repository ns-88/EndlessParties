using EndlessParties.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Presentation.Controllers;

/// <summary>
/// Эхо контроллер
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EchoController : ControllerBase
{
    /// <summary>
    /// Выполнение эхо-запроса
    /// </summary>
    /// <returns>Дата и время выполнения запроса</returns>
    [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
    [HttpGet]
    public Task<DateTime> Echo([FromQuery] EchoModel model)
    {
        return Task.FromResult(DateTime.Now);
    }
}