using Microsoft.AspNetCore.Builder;
using Serilog;

namespace EndlessParties.Shared.Utils.Logger;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilderExtensions"/> для добавления логгера Serilog
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Добавление логгера Serilog
    /// </summary>
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services);
        });

        return builder;
    }
}