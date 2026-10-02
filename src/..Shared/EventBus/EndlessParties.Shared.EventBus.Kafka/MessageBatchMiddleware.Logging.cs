using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

internal partial class MessageBatchMiddleware<T>
{
    /// <summary>
    /// Получен список сообщений для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен список сообщений для обработки. Сообщений всего: {MessageCount}, тип: {MessageType}, топик: {TopicName}")]
    private partial void LogMessagesForProcessingReceived(int messageCount, string messageType, string topicName);

    /// <summary>
    /// Ошибка обработки списка сообщений
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки списка сообщений. Тип: {MessageType}, топик: {TopicName}")]
    private partial void LogMessagesProcessingError(Exception ex, string messageType, string topicName);
}