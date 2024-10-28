
namespace ZeroMessenger;

internal sealed class FilteredMessageSubscriber<TMessage>(IMessageSubscriber<TMessage> subscriber, MessageFilter<TMessage>[] filters) : IMessageSubscriber<TMessage>
{
    public IMessageSubscriber<TMessage> Subscriber { get; } = subscriber;
    public MessageFilter<TMessage>[] Filters { get; } = filters;

    public IDisposable Subscribe(MessageHandler<TMessage> handler)
    {
        return Subscriber.SubscribeAwait(new FilteredMessageHandler<TMessage>(handler, Filters));
    }

    public IDisposable SubscribeAwait(AsyncMessageHandler<TMessage> handler, AsyncSubscribeStrategy subscribeStrategy = AsyncSubscribeStrategy.Sequential)
    {
        return Subscriber.SubscribeAwait(new FilteredAsyncMessageHandler<TMessage>(handler, Filters), subscribeStrategy);
    }
}