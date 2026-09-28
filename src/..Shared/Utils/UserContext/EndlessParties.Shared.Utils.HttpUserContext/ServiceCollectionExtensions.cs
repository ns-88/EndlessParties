using EndlessParties.Shared.Utils.UserContext.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Utils.HttpUserContext;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервиса доступа к HTTP-контексту пользователя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервиса доступа к HTTP-контексту пользователя
    /// </summary>
    public static IServiceCollection AddHttpUserContextAccessor(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddScoped<IUserContextAccessor, HttpUserContextAccessor>();

        return services;
    }
}