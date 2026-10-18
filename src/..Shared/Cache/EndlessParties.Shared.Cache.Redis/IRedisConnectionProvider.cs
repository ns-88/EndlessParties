using StackExchange.Redis;

namespace EndlessParties.Shared.Cache.Redis;

/// <summary>
/// Провайдер для доступа к подключению Redis
/// </summary>
internal interface IRedisConnectionProvider
{
    /// <summary>
    /// Подключение к серверу Redis
    /// </summary>
    IConnectionMultiplexer Connection { get; }
}