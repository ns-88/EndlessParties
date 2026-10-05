using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов авторизации и аутентификации
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Добавление сервисов авторизации и аутентификации
    /// </summary>
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IdentitySettings settings)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = settings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateLifetime = true,
                    RoleClaimType = "role"
                };
            });

        services
            .AddAuthorization();

        return services;
    }
}