using Mediator;

namespace EndlessParties.Identity.Api.App.Features.Login;

/// <summary>
/// Запрос аутентификации пользователя и выдачи JWT-токена
/// </summary>
public class LoginUserQuery(LoginUserRequest loginRequest) : IRequest<LoginUserResponse>
{
    /// <summary>
    /// Данные запроса аутентификации пользователя
    /// </summary>
    public LoginUserRequest LoginRequest { get; } = loginRequest;
}