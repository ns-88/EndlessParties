using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.EventBus.Kafka.Settings;
using KafkaFlow.Producers;

namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// Распределенная шина событий на основе очереди Kafka
/// </summary>
internal class KafkaEventBus : IEventBus
{
    /// <summary>
    /// Провайдер <see cref="IProducerAccessor"/>
    /// </summary>
    private readonly IProducerAccessor _producerAccessor;

    /// <summary>
    /// Поставщики
    /// </summary>
    private readonly IReadOnlyDictionary<Type, KafkaProducerSettings> _producers;


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaEventBus(KafkaEventBusSettings settings, IProducerAccessor producerAccessor)
    {
        _producers = settings.Producers;
        _producerAccessor = producerAccessor;
    }


    /// <inheritdoc />
    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        var eventType = @event.GetType();

        if (!_producers.TryGetValue(eventType, out var producerSettings))
        {
            throw new InvalidOperationException($"Не найден поставщик событий для указанного типа. Тип события: \"{eventType.Name}\"");
        }

        var producer = _producerAccessor.GetProducer(producerSettings.Name);

        await producer.ProduceAsync(Guid.NewGuid().ToString(), @event);
    }
}