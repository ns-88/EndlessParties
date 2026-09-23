namespace EndlessParties.Identity.Cryptography.Abstractions.Models;

/// <summary>
/// Данные JWT-токена
/// </summary>
public readonly struct JwtTokenModel(string token, int expires)
{
    /// <summary>
    /// Значение токена
    /// </summary>
    public string Token { get; } = token;

    /// <summary>
    /// Время жизни в секундах
    /// </summary>
    public int Expires { get; } = expires;
}