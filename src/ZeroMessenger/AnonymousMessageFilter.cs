namespace ZeroMessenger;

internal sealed class AnonymousMessageFilter<T>(Func<T, CancellationToken, Func<T, CancellationToken, ValueTask>, ValueTask> filter) : MessageFilter<T>
{
    public override ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
    {
        return filter(message, cancellationToken, next);
    }
}