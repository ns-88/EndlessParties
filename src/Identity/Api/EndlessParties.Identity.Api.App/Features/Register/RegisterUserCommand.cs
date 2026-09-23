using Mediator;

namespace EndlessParties.Identity.Api.App.Features.Register;

/// <summary>
/// Команда регистрации пользователя
/// </summary>
public class RegisterUserCommand(RegisterUserRequest registerRequest) : IRequest
{
    /// <summary>
    /// Запрос регистрации пользователя
    /// </summary>
    public RegisterUserRequest RegisterRequest { get; } = registerRequest;
}