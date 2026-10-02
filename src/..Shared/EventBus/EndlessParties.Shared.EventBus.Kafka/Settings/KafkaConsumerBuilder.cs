using System.Reflection;
using EndlessParties.Shared.EventBus.Abstractions;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Конфигуратор потребителей
/// </summary>
public class KafkaConsumerBuilder
{
    /// <summary>
    /// Текст ошибки "Потребитель уже добавлен"
    /// </summary>
    private const string ErrorConsumerExists = "Потребитель уже добавлен. Тип события: \"{0}\", топик: \"{1}\"";

    /// <summary>
    /// Текст ошибки "Обработчик для потребителя не найден"
    /// </summary>
    private const string ErrorHandlerNotFound = "Обработчик для потребителя не найден. Тип события: \"{0}\", топик: \"{1}\"";

    /// <summary>
    /// Типы найденных в сборке потребителей
    /// </summary>
    private readonly IReadOnlyDictionary<Type, Type> _consumerTypes;

    /// <summary>
    /// Список добавленных потребителей
    /// </summary>
    internal IReadOnlyDictionary<Type, KafkaConsumerSettings> Consumers { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaConsumerBuilder(Assembly assembly)
    {
        _consumerTypes = GetConsumerTypes(assembly);
        Consumers = new Dictionary<Type, KafkaConsumerSettings>();
    }


    /// <summary>
    /// Добавление потребителя
    /// </summary>
    public KafkaConsumerBuilder Add<TEvent>(string groupId, string topicName) where TEvent : class
    {
        var consumers = (Dictionary<Type, KafkaConsumerSettings>)Consumers;
        var type = typeof(TEvent);

        if (!_consumerTypes.TryGetValue(type, out var targetImplementationType))
        {
            throw new InvalidOperationException(string.Format(ErrorHandlerNotFound, type.Name, topicName));
        }

        var settings = new KafkaConsumerSettings
        {
            GroupId = groupId,
            TopicName = topicName,
            EventType = typeof(TEvent),
            ProxyType = typeof(MessageHandler<TEvent>),
            TargetInterfaceType = typeof(IEventBusConsumer<TEvent>),
            TargetImplementationType = targetImplementationType
        };

        return !consumers.TryAdd(type, settings)
            ? throw new InvalidOperationException(string.Format(ErrorConsumerExists, type.Name, topicName))
            : this;
    }

    /// <summary>
    /// Добавление потребителя с поддержкой объединения входящих сообщений в пакет (батч)
    /// </summary>
    public KafkaConsumerBuilder Add<TEvent>(string groupId, string topicName, KafkaBatchSettings batchSettings) where TEvent : class
    {
        var consumers = (Dictionary<Type, KafkaConsumerSettings>)Consumers;
        var type = typeof(TEvent);

        if (!_consumerTypes.TryGetValue(type, out var targetImplementationType))
        {
            throw new InvalidOperationException(string.Format(ErrorHandlerNotFound, type.Name, topicName));
        }

        var settings = new KafkaConsumerBatchSettings
        {
            GroupId = groupId,
            TopicName = topicName,
            EventType = typeof(TEvent),
            ProxyType = typeof(MessageBatchMiddleware<TEvent>),
            TargetInterfaceType = typeof(IEventBusBatchConsumer<TEvent>),
            TargetImplementationType = targetImplementationType,
            Count = batchSettings.Count,
            Timeout = batchSettings.Timeout
        };

        return !consumers.TryAdd(type, settings)
            ? throw new InvalidOperationException(string.Format(ErrorConsumerExists, type.Name, topicName))
            : this;
    }

    /// <summary>
    /// Получить типы найденных в сборке потребителей с ожидаемыми интерфейсами обработчиков
    /// </summary>
    private static IReadOnlyDictionary<Type, Type> GetConsumerTypes(Assembly assembly)
    {
        var map = new Dictionary<Type, Type>();
        var types = assembly.DefinedTypes.Where(x => x is { IsClass: true, IsAbstract: false });

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }

                var typeDefinition = @interface.GetGenericTypeDefinition();
                if (typeDefinition != typeof(IEventBusConsumer<>) && typeDefinition != typeof(IEventBusBatchConsumer<>))
                {
                    continue;
                }

                var argument = @interface.GetGenericArguments()[0];
                if (!map.TryAdd(argument, type))
                {
                    throw new InvalidOperationException($"Для события найдено более одного обработчика. Наименование события: \"{type.Name}\"");
                }
            }
        }

        return map;
    }
}