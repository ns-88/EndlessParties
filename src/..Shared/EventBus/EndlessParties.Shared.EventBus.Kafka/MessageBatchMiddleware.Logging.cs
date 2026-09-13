using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

internal partial class MessageBatchMiddleware<T>
{
    /// <summary>
    /// Получен список событий для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен список событий для обработки. Событий всего: {EventCount}, тип: {EventType}, топик: {TopicName}")]
    private partial void LogEventsForProcessingReceived(int eventCount, string eventType, string topicName);

    [LoggerMessage(LogLevel.Error, "Ошибка обработки списка событий. Тип: {EventType}, топик: {TopicName}")]
    private partial void LogEventsProcessingError(Exception ex, string eventType, string topicName);
}