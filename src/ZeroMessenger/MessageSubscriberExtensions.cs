using System.Runtime.CompilerServices;

namespace ZeroMessenger;

public static class MessageSubscriberExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable Subscribe<T>(this IMessageSubscriber<T> subscriber, Action<T> handler)
    {
        return subscriber.Subscribe(new AnonymousMessageHandler<T>(handler));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable Subscribe<T, TState>(this IMessageSubscriber<T> subscriber, TState state, Action<T, TState> handler)
    {
        return subscriber.Subscribe(new AnonymousMessageHandler<T, TState>(state, handler));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable SubscribeAwait<T>(this IMessageSubscriber<T> subscriber, Func<T, CancellationToken, ValueTask> handler, AsyncSubscribeStrategy subscribeStrategy = AsyncSubscribeStrategy.Sequential)
    {
        return subscriber.SubscribeAwait(new AnonymousAsyncMessageHandler<T>(handler), subscribeStrategy);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable SubscribeAwait<T, TState>(this IMessageSubscriber<T> subscriber, TState state, Func<T, TState, CancellationToken, ValueTask> handler, AsyncSubscribeStrategy subscribeStrategy = AsyncSubscribeStrategy.Sequential)
    {
        return subscriber.SubscribeAwait(new AnonymousAsyncMessageHandler<T, TState>(state, handler), subscribeStrategy);
    }
}