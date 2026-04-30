using System.Reflection;
using EndlessParties.Shared.Validation.Configurations;
using EndlessParties.Shared.Validation.Dispatcher;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Validation;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления валидации приложения
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление валидации приложения
    /// </summary>
    public static IServiceCollection AddApplicationValidation(this IServiceCollection services, Assembly assembly)
    {
        services
            .AddValidatorsFromAssembly(assembly)
            .AddSingleton<ValidationDispatcher>()
            .AddScoped<ValidationFilter>()
            .ConfigureOptions<ConfigureValidationFilterOptions>()
            .ConfigureOptions<ConfigureApiBehaviorOptions>();

        return services;
    }
}