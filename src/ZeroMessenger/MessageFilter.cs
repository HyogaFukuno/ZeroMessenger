using ZeroMessenger.Internal;

namespace ZeroMessenger;

public abstract class MessageFilterBase
{
}

[Preserve]
public abstract class MessageFilter<T> : MessageFilterBase
{
    public abstract ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next);
}