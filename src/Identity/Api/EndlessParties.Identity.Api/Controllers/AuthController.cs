using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EndlessParties.Identity.Api.Controllers;

/// <summary>
/// Контроллер аутентификации и управления учетными записями пользователей
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
}