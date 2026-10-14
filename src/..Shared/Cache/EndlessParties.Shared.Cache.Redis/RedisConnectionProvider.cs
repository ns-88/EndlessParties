using EndlessParties.Shared.Cache.Redis.Settings;
using StackExchange.Redis;

namespace EndlessParties.Shared.Cache.Redis;

/// <inheritdoc cref="IRedisConnectionProvider" />
internal class RedisConnectionProvider : IRedisConnectionProvider, IAsyncDisposable
{
    /// <summary>
    /// Настройки клиента
    /// </summary>
    private readonly RedisClientSettings _settings;

    /// <summary>
    /// Адрес подключения в виде строки
    /// </summary>
    public string Address { get; }

    #region Connection
    private IConnectionMultiplexer? _connection;
    /// <inheritdoc />
    public IConnectionMultiplexer Connection =>
        _connection ?? throw new InvalidOperationException("Подключение к серверу Redis не инициализировано");
    #endregion


    /// <summary>
    /// Конструктор
    /// </summary>
    public RedisConnectionProvider(RedisClientSettings settings)
    {
        _settings = settings;
        Address = $"{settings.Host}:{settings.Port}";
    }


    /// <summary>
    /// Инициализация подключения
    /// </summary>
    public async Task Initialize()
    {
        if (_connection != null)
        {
            return;
        }

        var connectTimeout = _settings.ConnectTimeout * (int)TimeSpan.MillisecondsPerSecond;
        var queryTimeout = _settings.QueryTimeout * (int)TimeSpan.MillisecondsPerSecond;
        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ClientName = _settings.ClientName,
            ConnectTimeout = connectTimeout,
            AsyncTimeout = queryTimeout,
            SyncTimeout = queryTimeout
        };
        options.EndPoints.Add(_settings.Host, _settings.Port);

        _connection = await ConnectionMultiplexer.ConnectAsync(options);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_connection == null)
        {
            return;
        }

        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
}