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
    public KafkaEventBus(KafkaSettings settings, IProducerAccessor producerAccessor)
    {
        _producers = settings.Producers;
        _producerAccessor = producerAccessor;
    }


    /// <inheritdoc />
    public async Task Publish(object @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var eventType = @event.GetType();

        try
        {
            if (!_producers.TryGetValue(eventType, out var producerSettings))
            {
                throw new InvalidOperationException("Не найден поставщик событий для указанного типа");
            }

            var eventKey = producerSettings.KeyProvider.GetKey(@event);
            var producer = _producerAccessor.GetProducer(producerSettings.Name);

            await producer.ProduceAsync(eventKey, @event);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка публикации события в очередь Kafka. Тип события: \"{eventType.Name}\"", ex);
        }
    }
}