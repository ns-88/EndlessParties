using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Formatting.Compact;

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
                .WriteTo.Console(new CompactJsonFormatter())
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
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(new CompactJsonFormatter())
                    .ReadFrom.Services(services);
            });

            return builder;
        }
    }
}