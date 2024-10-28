namespace ZeroMessenger;

public interface IMessageSubscriber<T>
{
    IDisposable Subscribe(MessageHandler<T> handler);
    IDisposable SubscribeAwait(AsyncMessageHandler<T> handler, AsyncSubscribeStrategy subscribeStrategy = AsyncSubscribeStrategy.Sequential);
}