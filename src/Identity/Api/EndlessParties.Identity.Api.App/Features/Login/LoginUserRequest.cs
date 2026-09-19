namespace EndlessParties.Identity.Api.App.Features.Login;

/// <summary>
/// Данные запроса аутентификации пользователя
/// </summary>
public class LoginUserRequest
{
    /// <summary>
    /// Имя (логин)
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string Password { get; init; }
}