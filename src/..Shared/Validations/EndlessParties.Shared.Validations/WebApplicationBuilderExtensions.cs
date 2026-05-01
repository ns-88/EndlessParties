using System.Reflection;
using EndlessParties.Shared.Validations.Configurations;
using EndlessParties.Shared.Validations.Dispatcher;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Validations;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilder"/> для добавления валидации приложения
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Добавление валидации приложения
    /// </summary>
    public static WebApplicationBuilder AddApplicationValidations(this WebApplicationBuilder builder, Assembly assembly)
    {
        builder.Services
            .AddValidatorsFromAssembly(assembly)
            .AddSingleton<ValidationDispatcher>()
            .AddScoped<ValidationFilter>()
            .ConfigureOptions<ConfigureValidationFilterOptions>()
            .ConfigureOptions<ConfigureApiBehaviorOptions>();

        return builder;
    }
}