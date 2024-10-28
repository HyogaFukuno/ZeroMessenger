namespace ZeroMessenger;

public interface IMessagePublisher<T>
{
    void Publish(T message, CancellationToken cancellationToken = default);
    ValueTask PublishAsync(T message, AsyncPublishStrategy publishStrategy = AsyncPublishStrategy.Parallel, CancellationToken cancellationToken = default);
}
