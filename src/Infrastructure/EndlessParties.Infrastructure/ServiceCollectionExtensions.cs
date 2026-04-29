using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Infrastructure;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов инфраструктурного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов инфраструктурного слоя
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}