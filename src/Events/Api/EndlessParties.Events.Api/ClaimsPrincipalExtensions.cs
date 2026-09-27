using System.Security.Claims;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Events.Api;

/// <summary>
/// Набор методов-расширений для типа <see cref="ClaimsPrincipal"/>
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получение идентификатора пользователя из данных аутентификации
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirst("sub")?.Value;

        return !Guid.TryParse(value, out var userId)
            ? throw new LogicException("Ошибка получения идентификатора пользователя")
            : userId;
    }
}