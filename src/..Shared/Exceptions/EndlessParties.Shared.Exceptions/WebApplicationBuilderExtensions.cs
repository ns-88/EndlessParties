using EndlessParties.Shared.Exceptions.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Exceptions;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilder"/> для добавления поддержки глобальной обработки исключений
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Добавление поддержки глобальной обработки исключений
    /// </summary>
    public static WebApplicationBuilder AddApplicationExceptions(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddProblemDetails()
            .AddTransient<IStartupFilter, ExceptionHandlerFilter>()
            .AddSingleton<IExceptionHandler, ExceptionHandler>()
            .ConfigureOptions<ConfigureMvcOptions>();

        return builder;
    }
}