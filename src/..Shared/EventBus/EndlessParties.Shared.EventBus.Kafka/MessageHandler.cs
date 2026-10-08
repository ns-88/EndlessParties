using EndlessParties.Shared.EventBus.Abstractions;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// Обработчик сообщений, поступающих из очереди
/// </summary>
internal partial class MessageHandler<T> : IMessageHandler<T> where T : class
{
    /// <summary>
    /// Логгер
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Фабрика <see cref="IServiceScopeFactory"/>
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;


    /// <summary>
    /// Конструктор
    /// </summary>
    public MessageHandler(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(MessageHandler<>));
    }


    /// <inheritdoc />
    public async Task Handle(IMessageContext context, T message)
    {
        var messageType = typeof(T).Name;
        var topicName = context.ConsumerContext.Topic;

        LogMessageForProcessingReceived(messageType, topicName);

        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IEventBusConsumer<T>>();

            await consumer.Consume(message, context.ConsumerContext.WorkerStopped);
        }
        catch (Exception ex)
        {
            LogMessageProcessingError(ex, messageType, topicName);
        }
    }
}