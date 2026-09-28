using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;

namespace EndlessParties.Shared.Utils.UserContext.Abstractions.Models;

/// <summary>
/// Контекст пользователя
/// </summary>
public class UserContext
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Роль
    /// </summary>
    public required UserRole Role { get; init; }
}