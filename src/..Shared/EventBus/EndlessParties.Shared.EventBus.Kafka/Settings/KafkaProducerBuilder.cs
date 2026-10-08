namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Конфигуратор поставщиков
/// </summary>
public class KafkaProducerBuilder
{
    /// <summary>
    /// Список добавленных постащиков
    /// </summary>
    internal IReadOnlyDictionary<Type, KafkaProducerSettings> Producers { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaProducerBuilder()
    {
        Producers = new Dictionary<Type, KafkaProducerSettings>();
    }


    /// <summary>
    /// Добавление поставщика
    /// </summary>
    public KafkaProducerBuilder Add<TEvent>(string topicName, Func<TEvent, string> keySelector)
    {
        var producers = (Dictionary<Type, KafkaProducerSettings>)Producers;
        var type = typeof(TEvent);
        var name = $"{type.Name}-{topicName}";

        var settings = new KafkaProducerSettings
        {
            Name = name,
            TopicName = topicName,
            KeyProvider = new KafkaEventKeyProvider<TEvent>(keySelector)
        };

        return !producers.TryAdd(type, settings)
            ? throw new InvalidOperationException($"Поставщик уже добавлен. Тип события: \"{type.Name}\", топик: \"{topicName}\"")
            : this;
    }
}