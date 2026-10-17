namespace EndlessParties.Shared.Cache.Abstractions;

/// <summary>
/// Клиент сервиса кэширования
/// </summary>
public interface ICacheClient
{
    /// <summary>
    /// Запись значения в кэш
    /// </summary>
    Task Set<T>(string key, T value, TimeSpan? expired = null) where T : class;

    /// <summary>
    /// Получение значения из кэша
    /// </summary>
    Task<T?> Get<T>(string key) where T : class;

    /// <summary>
    /// Удаление значения из кэша
    /// </summary>
    Task Remove(string key);
}