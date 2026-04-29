using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Application;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов прикладного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов прикладного слоя
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}