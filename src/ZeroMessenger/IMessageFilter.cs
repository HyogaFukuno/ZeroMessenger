using ZeroMessenger.Internal;

namespace ZeroMessenger;

public interface IMessageFilterBase
{
}

[Preserve]
public interface IMessageFilter<T> : IMessageFilterBase
{
    ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next);
}