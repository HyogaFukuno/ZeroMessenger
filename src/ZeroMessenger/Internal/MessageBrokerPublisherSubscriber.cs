namespace ZeroMessenger.Internal;

[Preserve]
public sealed class MessageBrokerPublisher<T>(MessageBroker<T> messageBroker) : IMessagePublisher<T>
{
    public void Publish(T message, CancellationToken cancellationToken = default)
    {
        messageBroker.Publish(message, cancellationToken);
    }

    public ValueTask PublishAsync(T message, AsyncPublishStrategy publishStrategy = AsyncPublishStrategy.Parallel, CancellationToken cancellationToken = default)
    {
        return messageBroker.PublishAsync(message, publishStrategy, cancellationToken);
    }
}

[Preserve]
public sealed class MessageBrokerSubscriber<T>(MessageBroker<T> messageBroker) : IMessageSubscriber<T>
{
    public IDisposable Subscribe(MessageHandler<T> handler)
    {
        return messageBroker.Subscribe(handler);
    }

    public IDisposable SubscribeAwait(AsyncMessageHandler<T> handler, AsyncSubscribeStrategy subscribeStrategy = AsyncSubscribeStrategy.Sequential)
    {
        return messageBroker.SubscribeAwait(handler, subscribeStrategy);
    }
}