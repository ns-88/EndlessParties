using Microsoft.Extensions.Logging;

namespace EndlessParties.Shared.EventBus.Kafka;

internal partial class MessageHandler<T>
{
    /// <summary>
    /// Получено сообщение для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получено сообщение для обработки. Тип: {MessageType}, топик: {TopicName}")]
    private partial void LogMessageForProcessingReceived(string messageType, string topicName);

    /// <summary>
    /// Ошибка обработки сообщения
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки сообщения. Тип: {MessageType}, топик: {TopicName}")]
    private partial void LogMessageProcessingError(Exception ex, string messageType, string topicName);
}