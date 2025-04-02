namespace ZeroMessenger;

internal sealed class AnonymousMessageHandler<T>(Action<T> handler) : MessageHandler<T>
{
    protected override void HandleCore(T message)
    {
        handler(message);
    }
}

internal sealed class AnonymousMessageHandler<T, TState>(TState state, Action<T, TState> handler) : MessageHandler<T>
{
    protected override void HandleCore(T message)
    {
        handler(message, state);
    }
}

internal sealed class AnonymousAsyncMessageHandler<T>(Func<T, CancellationToken, ValueTask> handler) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return handler(message, cancellationToken);
    }
}

internal sealed class AnonymousAsyncMessageHandler<T, TState>(TState state, Func<T, TState, CancellationToken, ValueTask> handler) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return handler(message, state, cancellationToken);
    }
}