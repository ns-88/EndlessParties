using System.Reflection;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Конфигуратор шины событий на основе очереди Kafka
/// </summary>
public class KafkaSettingsBuilder
{
    /// <summary>
    /// Настройки <see cref="KafkaConnectionSettings"/>
    /// </summary>
    private readonly KafkaConnectionSettings _connectionSettings;

    /// <summary>
    /// Поставщики
    /// </summary>
    private IReadOnlyDictionary<Type, KafkaProducerSettings> _producers;

    /// <summary>
    /// Потребители
    /// </summary>
    private IReadOnlyDictionary<Type, KafkaConsumerSettings> _consumers;


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaSettingsBuilder(KafkaConnectionSettings connectionSettings)
    {
        _producers = new Dictionary<Type, KafkaProducerSettings>();
        _consumers = new Dictionary<Type, KafkaConsumerSettings>();
        _connectionSettings = connectionSettings;
    }


    /// <summary>
    /// Конфигурирование поставщиков
    /// </summary>
    public KafkaSettingsBuilder WithProducers(Action<KafkaProducerBuilder> setup)
    {
        var producerBuilder = new KafkaProducerBuilder();
        setup(producerBuilder);

        _producers = producerBuilder.Producers;

        return this;
    }

    /// <summary>
    /// Конфигурирование потребителей
    /// </summary>
    public KafkaSettingsBuilder WithConsumers(Assembly assembly, Action<KafkaConsumerBuilder> setup)
    {
        var consumerBuilder = new KafkaConsumerBuilder(assembly);
        setup(consumerBuilder);

        _consumers = consumerBuilder.Consumers;

        return this;
    }

    /// <summary>
    /// Получение настроек шины
    /// </summary>
    public KafkaSettings Build()
    {
        return new KafkaSettings
        {
            Producers = _producers,
            Consumers = _consumers,
            BrokerAddress = _connectionSettings.BrokerAddress
        };
    }
}