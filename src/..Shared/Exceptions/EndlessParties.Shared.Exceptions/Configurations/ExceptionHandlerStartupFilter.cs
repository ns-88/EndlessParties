using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EndlessParties.Shared.Exceptions.Configurations;

/// <summary>
/// Фильтр запуска для включения обработчика исключений
/// </summary>
internal class ExceptionHandlerStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseExceptionHandler();
            next(builder);
        };
    }
}