using ZeroMessenger.Internal;

namespace ZeroMessenger;

public interface IMessageFilterBase
{
}

[Preserve]
public interface IMessageFilter<T> : IMessageFilterBase
{
    public abstract ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next);
}