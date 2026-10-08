using Microsoft.AspNetCore.Builder;
using Serilog;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Набор методов-расширений для добавления логгера Serilog
/// </summary>
public static class LoggingExtensions
{
    extension(Log)
    {
        /// <summary>
        /// Добавление базового логгера
        /// </summary>
        public static void AddBootstrapLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateBootstrapLogger();
        }
    }

    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Добавление логгера Serilog
        /// </summary>
        public WebApplicationBuilder AddSerilog()
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
}