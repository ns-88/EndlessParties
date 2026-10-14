using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.Cache.Redis;

/// <summary>
/// Сервис отслеживания событий жизненного цикла хоста для Redis
/// </summary>
internal class RedisLifecycleManager : IHostedLifecycleService
{
    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Провайдер <see cref="RedisConnectionProvider"/>
    /// </summary>
    private readonly RedisConnectionProvider _connectionProvider;


    /// <summary>
    /// Конструктор
    /// </summary>
    public RedisLifecycleManager(RedisConnectionProvider connectionProvider, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(nameof(RedisLifecycleManager));
        _connectionProvider = connectionProvider;
    }


    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connectionProvider is { Connection: null })
        {
            return;
        }

        _logger.LogInformation("Остановка соединения с сервером Redis. Адрес: \"{RedisAddress}\"",
            _connectionProvider.Address);

        try
        {
            await _connectionProvider.Connection.DisposeAsync();

            _logger.LogInformation("Остановлено соединение с сервером Redis. Адрес: \"{RedisAddress}\"",
                _connectionProvider.Address);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка остановки соединения с сервером Redis. Адрес: \"{RedisAddress}\"",
                _connectionProvider.Address);
        }
    }

    /// <inheritdoc />
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Инициализация подключения к серверу Redis. Адрес: \"{RedisAddress}\"",
            _connectionProvider.Address);

        try
        {
            await _connectionProvider.Initialize();

            if (_connectionProvider.Connection.IsConnected)
            {
                _logger.LogInformation("Установлено подключение к серверу Redis. Адрес: \"{RedisAddress}\"",
                    _connectionProvider.Address);
            }
            else
            {
                _logger.LogWarning("Не установлено подключение к серверу Redis. Адрес: \"{RedisAddress}\"",
                    _connectionProvider.Address);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка инициализации подключения к серверу Redis. Адрес: \"{RedisAddress}\"",
                _connectionProvider.Address);
        }
    }

    /// <inheritdoc />
    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}