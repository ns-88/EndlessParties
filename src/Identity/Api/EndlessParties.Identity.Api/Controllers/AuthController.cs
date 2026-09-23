using EndlessParties.Identity.Api.App.Features.Login;
using EndlessParties.Identity.Api.App.Features.Register;
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


    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Аутентификация пользователя и выдача JWT-токена
    /// </summary>
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request);
        var loginUserModel = await _mediator.Send(query, cancellationToken);

        return Ok(loginUserModel);
    }
}