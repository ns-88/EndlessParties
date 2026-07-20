using System.Threading.Channels;
using EndlessParties.Shared.MessageBus.Abstractions;

namespace EndlessParties.Shared.ChannelMessageBus;

/// <summary>
/// Шина сообщений на основе каналов
/// </summary>
internal class ChannelMessageBus<T> : IPublisher<T>, ISubscriber<T>
{
    /// <summary>
    /// Максимальное число сообщений в канале
    /// </summary>
    private const int MaxMessagesCount = 1000;

    /// <summary>
    /// Потребитель сообщений из канала
    /// </summary>
    private readonly ChannelReader<T> _reader;

    /// <summary>
    /// Постащик сообщений в канал
    /// </summary>
    private readonly ChannelWriter<T> _writer;


    /// <summary>
    /// Конструктор
    /// </summary>
    public ChannelMessageBus()
    {
        var options = new BoundedChannelOptions(MaxMessagesCount)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        var channel = Channel.CreateBounded<T>(options);

        _reader = channel.Reader;
        _writer = channel.Writer;
    }


    /// <inheritdoc />
    public bool TryPublish(T message)
    {
        return _writer.TryWrite(message);
    }

    /// <inheritdoc />
    public Task<T> Read(CancellationToken cancellationToken)
    {
        return _reader.ReadAsync(cancellationToken).AsTask();
    }
}