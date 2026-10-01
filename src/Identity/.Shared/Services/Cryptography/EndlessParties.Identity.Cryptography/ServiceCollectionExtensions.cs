using EndlessParties.Identity.Cryptography.Abstractions;
using EndlessParties.Identity.Cryptography.Services;
using EndlessParties.Identity.Cryptography.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Identity.Cryptography;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления криптографических сервисов
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление криптографических сервисов
    /// </summary>
    public static IServiceCollection AddCryptographyServices(this IServiceCollection services, JwtTokenSettings settings)
    {
        settings.Validate();

        services
            .AddSingleton(settings)
            .AddSingleton<IPasswordManager, PasswordManager>()
            .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}