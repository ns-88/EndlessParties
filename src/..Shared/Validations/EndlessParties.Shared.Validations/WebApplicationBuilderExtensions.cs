using System.Collections.Frozen;
using System.Reflection;
using EndlessParties.Shared.Validations.Configurations;
using EndlessParties.Shared.Validations.Dispatcher;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace EndlessParties.Shared.Validations;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilder"/> для добавления валидации приложения
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Добавление валидации приложения
    /// </summary>
    public static WebApplicationBuilder AddApplicationValidations(this WebApplicationBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => new
            {
                x.GetName().Name,
                Assembly = x
            })
            .Where(x => !x.Assembly.IsDynamic && x.Name!.StartsWith("EndlessParties"))
            .ToFrozenDictionary(k => k.Name!, v => v.Assembly);

        var libraries = DependencyContext.Default?.RuntimeLibraries
            .Where(x => x.Name.StartsWith("EndlessParties"));

        if (libraries == null)
        {
            return builder;
        }

        var validationAssemblies = new List<Assembly>();

        foreach (var library in libraries)
        {
            if (!assemblies.TryGetValue(library.Name, out var assembly))
            {
                assembly = Assembly.Load(library.Name);
            }

            validationAssemblies.Add(assembly);
        }

        builder.Services
            .AddValidatorsFromAssemblies(validationAssemblies, includeInternalTypes: true)
            .AddSingleton<ValidationDispatcher>()
            .AddScoped<ValidationFilter>()
            .ConfigureOptions<ConfigureMvcOptions>()
            .ConfigureOptions<ConfigureApiBehaviorOptions>();

        return builder;
    }
}