using EndlessParties.Identity.Cryptography.Abstractions.Models;
using EndlessParties.Identity.Domain.Models;

namespace EndlessParties.Identity.Cryptography.Abstractions;

/// <summary>
/// Генератор JWT-токенов
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Генерация токена
    /// </summary>
    JwtTokenModel Generate(User user);
}