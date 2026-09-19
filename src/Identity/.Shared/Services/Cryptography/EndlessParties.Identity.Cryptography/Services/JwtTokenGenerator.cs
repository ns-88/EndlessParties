using System.Security.Claims;
using System.Text;
using EndlessParties.Identity.Cryptography.Abstractions;
using EndlessParties.Identity.Cryptography.Abstractions.Models;
using EndlessParties.Identity.Cryptography.Settings;
using EndlessParties.Identity.Domain.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EndlessParties.Identity.Cryptography.Services;

/// <inheritdoc />
public class JwtTokenGenerator : IJwtTokenGenerator
{
    /// <summary>
    /// Настройки <see cref="JwtTokenSettings"/>
    /// </summary>
    private readonly JwtTokenSettings _settings;


    /// <summary>
    /// Конструктор
    /// </summary>
    public JwtTokenGenerator(JwtTokenSettings settings)
    {
        _settings = settings;
    }


    /// <inheritdoc />
    public JwtTokenModel Generate(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("role", user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.Expires),
            Issuer = _settings.Issuer,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = credentials
        };

        foreach (var audience in _settings.Audiences)
        {
            tokenDescriptor.Audiences.Add(audience);
        }

        var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        var expires = (int)TimeSpan.FromMinutes(_settings.Expires).TotalSeconds;

        return new JwtTokenModel(token, expires);
    }
}