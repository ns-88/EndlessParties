using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// Сервис отслеживания событий жизненного цикла хоста для шины <see cref="IKafkaBus"/>
/// </summary>
public class KafkaBusLifecycleManager : IHostedLifecycleService
{
    /// <summary>
    /// Логгер
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Провайдер <see cref="IServiceProvider"/>
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Шина <see cref="IKafkaBus"/>
    /// </summary>
    private IKafkaBus? _kafkaBus;


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaBusLifecycleManager(IServiceProvider serviceProvider)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        _logger = loggerFactory.CreateLogger(nameof(KafkaBusLifecycleManager));
        _serviceProvider = serviceProvider;
    }


    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание и запуск шины событий Kafka");
        
        try
        {
            _kafkaBus = _serviceProvider.CreateKafkaBus();
            await _kafkaBus.StartAsync(cancellationToken);

            _logger.LogInformation("Шина событий Kafka успешно создана и запущена");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Ошибка создания и запуска шины событий Kafka");

            throw;
        }
    }

    /// <inheritdoc />
    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        if (_kafkaBus == null)
        {
            return;
        }
        
        await _kafkaBus.StopAsync();
    }
}