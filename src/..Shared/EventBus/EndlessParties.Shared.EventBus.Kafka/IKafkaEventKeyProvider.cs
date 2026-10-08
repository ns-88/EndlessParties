namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// Провайдер для получения значения ключа события, отправляемого в очередь Kafka
/// </summary>
public interface IKafkaEventKeyProvider
{
    /// <summary>
    /// Получение ключа
    /// </summary>
    string GetKey(object @event);
}