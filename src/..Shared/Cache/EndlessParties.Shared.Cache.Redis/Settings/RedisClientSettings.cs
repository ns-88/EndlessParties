using FluentValidation;

namespace EndlessParties.Shared.Cache.Redis.Settings;

/// <summary>
/// Настройки клиента СУБД Redis
/// </summary>
public class RedisClientSettings
{
    /// <summary>
    /// Адрес хоста
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Порт
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// Таймаут установления подключения (в секундах)
    /// </summary>
    public int ConnectTimeout { get; init; } = 5;

    /// <summary>
    /// Таймаут выполнения запросов (в секундах)
    /// </summary>
    public int QueryTimeout { get; init; } = 3;

    /// <summary>
    /// Наименование клиента
    /// </summary>
    public required string ClientName { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new RedisClientSettingsValidator().ValidateAndThrow(this);
}