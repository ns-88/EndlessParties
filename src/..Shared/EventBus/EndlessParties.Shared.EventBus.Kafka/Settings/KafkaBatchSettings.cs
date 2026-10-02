namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки параметров батчинга
/// </summary>
public class KafkaBatchSettings(int count, TimeSpan timeout)
{
    /// <summary>
    /// Значение настроек по умолчанию
    /// </summary>
    public static readonly KafkaBatchSettings Default = new();

    /// <summary>
    /// Размер пакета (батча)
    /// </summary>
    public int Count { get; } = count;

    /// <summary>
    /// Время ожидания накопления пакета (батча)
    /// </summary>
    public TimeSpan Timeout { get; } = timeout;


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaBatchSettings() : this(10, TimeSpan.FromSeconds(3))
    {
    }
}