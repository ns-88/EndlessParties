namespace EndlessParties.Shared.Utils.Database.Settings;

/// <summary>
/// Настройки подключения к БД
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Строка подключения к базе данных
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    /// Количество попыток подключения к БД
    /// </summary>
    public required int RetryReconnectDatabaseCount { get; init; }
}