using EndlessParties.Shared.EventBus.Abstractions;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// ПО промежуточного слоя для доступа к списку событий, получаемых из очереди
/// </summary>
internal partial class MessageBatchMiddleware<T> : IMessageMiddleware where T : class
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
    public MessageBatchMiddleware(IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(MessageBatchMiddleware<>));
    }


    /// <inheritdoc />
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var batch = context.GetMessagesBatch();

        LogEventsForProcessingReceived(batch.Count, typeof(T).Name, context.ConsumerContext.Topic);

        try
        {
            var typedList = batch.Select(x => x.Message.Value).Cast<T>().ToArray();
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IEventBusBatchConsumer<T>>();

            await consumer.Consume(typedList, context.ConsumerContext.WorkerStopped);
        }
        catch (Exception ex)
        {
            LogEventsProcessingError(ex, typeof(T).Name, context.ConsumerContext.Topic);
        }

        await next(context);
    }
}