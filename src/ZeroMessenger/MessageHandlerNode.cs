using ZeroMessenger.Internal;

namespace ZeroMessenger;

public abstract class MessageHandlerNode<T> : IDisposable
{
    internal MessageHandlerList<T> Parent = default!;
    internal MessageHandlerNode<T>? PreviousNode;
    internal MessageHandlerNode<T>? NextNode;
    internal ulong Version;

    bool disposed;
    public bool IsDisposed => disposed;

    public virtual void Dispose()
    {
        ThrowHelper.ThrowObjectDisposedIf(IsDisposed, typeof(MessageHandlerNode<T>));

        Parent.Remove(this);
        Volatile.Write(ref disposed, true);
        Volatile.Write(ref Parent!, null);
    }
}
