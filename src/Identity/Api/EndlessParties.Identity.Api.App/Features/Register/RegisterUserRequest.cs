using EndlessParties.Identity.Domain.Enums;

namespace EndlessParties.Identity.Api.App.Features.Register;

/// <summary>
/// Данные запроса регистрации пользователя
/// </summary>
public class RegisterUserRequest
{
    /// <summary>
    /// Имя (логин)
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Роль
    /// </summary>
    public required UserRole Role { get; init; }
}