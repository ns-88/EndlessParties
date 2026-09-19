using EndlessParties.Identity.Database;
using EndlessParties.Identity.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Identity.Api.Infrastructure;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов инфраструктурного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов инфраструктурного слоя
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, InfrastructureSettings settings)
    {
        settings.Validate();

        services
            .AddRepositories()
            .AddIdentityDatabase(settings.IdentityDatabase);

        return services;
    }
}