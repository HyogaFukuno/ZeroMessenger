using System.Runtime.CompilerServices;

namespace ZeroMessenger;

public abstract class AsyncMessageHandler<T> : MessageHandlerNode<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask HandleAsync(T message, CancellationToken cancellationToken = default)
    {
        return HandleAsyncCore(message, cancellationToken);
    }

    protected virtual ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return default;
    }
}