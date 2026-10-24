using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Фильтр запуска для включения конечной точки получения метрик
/// </summary>
internal class MetricsStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseOpenTelemetryPrometheusScrapingEndpoint();
            next(builder);
        };
    }
}