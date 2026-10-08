namespace EndlessParties.Shared.EventBus.Kafka;

/// <inheritdoc />
internal class KafkaEventKeyProvider<TEvent> : IKafkaEventKeyProvider
{
    /// <summary>
    /// Селектор ключа
    /// </summary>
    private readonly Func<TEvent, string> _keySelector;


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaEventKeyProvider(Func<TEvent, string> keySelector)
    {
        _keySelector = keySelector;
    }


    /// <inheritdoc />
    public string GetKey(object message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message is not TEvent typedMessage)
        {
            throw new InvalidOperationException("Тип события не является ожидаемым. " +
                                                $"Полученный тип: \"{message.GetType().Name}\", ожидаемый тип: \"{typeof(TEvent).Name}\"");
        }

        return _keySelector(typedMessage);
    }
}