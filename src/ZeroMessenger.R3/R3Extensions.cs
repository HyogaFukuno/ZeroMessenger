using R3;

namespace ZeroMessenger.R3;

public static class MessageSubscriberR3Extensions
{
    public static Observable<T> ToObservable<T>(this IMessageSubscriber<T> subscriber)
    {
        return new ObservableSubscriber<T>(subscriber);
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