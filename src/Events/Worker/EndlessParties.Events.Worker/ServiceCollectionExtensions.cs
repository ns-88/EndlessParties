using System.Text;

namespace EndlessParties.Events.Worker;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов worker
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов worker
    /// </summary>
    public static IServiceCollection AddWorker(this IServiceCollection services)
    {
        services
            .AddHealthChecks();

        services
            .AddEndpointsApiExplorer();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return services;
    }
}