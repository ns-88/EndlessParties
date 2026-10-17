using System.Text.Json;
using EndlessParties.Shared.Cache.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EndlessParties.Shared.Cache.Redis;

/// <summary>
/// Клиент СУБД Redis
/// </summary>
internal class RedisCacheClient : ICacheClient
{
    /// <summary>
    /// Логгер
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Объект управления базой данных
    /// </summary>
    private readonly IDatabase _database;


    /// <summary>
    /// Конструктор
    /// </summary>
    public RedisCacheClient(IRedisConnectionProvider connectionProvider, ILoggerFactory loggerFactory)
    {
        _database = connectionProvider.Connection.GetDatabase();
        _logger = loggerFactory.CreateLogger(nameof(RedisCacheClient));
    }


    /// <inheritdoc />
    public async Task Set<T>(string key, T value, TimeSpan? expired = null) where T : class
    {
        try
        {
            var textValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, textValue, expired, false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка записи значения в кэш. Ключ: \"{RedisStringKey}\"", key);
        }
    }

    /// <inheritdoc />
    public async Task<T?> Get<T>(string key) where T : class
    {
        T? value = null;

        try
        {
            var rawValue = await _database.StringGetAsync(key);
            if (rawValue.HasValue)
            {
                value = JsonSerializer.Deserialize<T>(rawValue.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка получения значения из кэша. Ключ: \"{RedisStringKey}\"", key);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task Remove(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка удаления значения из кэша. Ключ: \"{RedisStringKey}\"", key);
        }
    }
}