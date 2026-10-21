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

    /// <inheritdoc />
    public IConnectionMultiplexer? Connection { get; private set; }


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

        Connection = await ConnectionMultiplexer.ConnectAsync(options);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Connection == null)
        {
            return;
        }

        await Connection.CloseAsync();
        await Connection.DisposeAsync();
    }
}