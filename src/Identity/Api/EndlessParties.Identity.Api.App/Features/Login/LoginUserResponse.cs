using EndlessParties.Identity.Domain.Enums;

namespace EndlessParties.Identity.Api.App.Features.Login;

/// <summary>
/// Данные аутентификации пользователя 
/// </summary>
public class LoginUserResponse
{
    /// <summary>
    /// JWT-токен
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Время жизни токена в секундах
    /// </summary>
    public required int ExpiresIn { get; init; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public required UserRole UserRole { get; init; }
}