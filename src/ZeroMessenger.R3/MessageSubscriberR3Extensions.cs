using R3;

namespace ZeroMessenger.R3;

public static class MessageSubscriberR3Extensions
{
    public static Observable<T> ToObservable<T>(this IMessageSubscriber<T> subscriber)
    {
        return new ObservableSubscriber<T>(subscriber);
    }

    public static IDisposable SubscribeToPublish<T>(this Observable<T> source, IMessagePublisher<T> publisher)
    {
        return source.Subscribe(publisher, (x, p) => p.Publish(x));
    }

    public static IDisposable SubscribeAwaitToPublish<T>(this Observable<T> source, IMessagePublisher<T> publisher, AwaitOperation awaitOperation = AwaitOperation.Sequential, AsyncPublishStrategy publishStrategy = AsyncPublishStrategy.Parallel)
    {
        return source.SubscribeAwait(async (x, ct) => await publisher.PublishAsync(x, publishStrategy, ct), awaitOperation);
    }
}

internal sealed class ObservableSubscriber<T>(IMessageSubscriber<T> subscriber) : Observable<T>
{
    protected override IDisposable SubscribeCore(Observer<T> observer)
    {
        return subscriber.Subscribe(new ObserverMessageHandler<T>(observer));
    }
}

internal sealed class ObserverMessageHandler<T>(Observer<T> observer) : MessageHandler<T>
{
    protected override void HandleCore(T message)
    {
        observer.OnNext(message);
    }
}