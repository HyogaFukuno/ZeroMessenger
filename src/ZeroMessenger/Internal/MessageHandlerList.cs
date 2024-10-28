using System.Runtime.CompilerServices;

namespace ZeroMessenger.Internal;

/// <summary>
/// MessageHandlerNode list
/// </summary>
internal sealed class MessageHandlerList<T>(object gate) : IDisposable
{
    MessageHandlerNode<T>? root;
    ulong version;
    bool isDisposed;

    public MessageHandlerNode<T>? Root => root;
    public bool IsDisposed => isDisposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(MessageHandlerNode<T> node)
    {
        lock (gate)
        {
            if (IsDisposed) return;

            node.Parent = this;
            node.Version = version;

            if (root == null)
            {
                root = node;
                node.NextNode = null;
                node.PreviousNode = null;
            }
            else
            {
                var lastNode = root.PreviousNode ?? root;

                lastNode.NextNode = node;
                node.PreviousNode = lastNode;
                root.PreviousNode = node;

                node.NextNode = null;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(MessageHandlerNode<T> node)
    {
        lock (gate)
        {
            if (IsDisposed) return;

            if (node == root)
            {
                if (node.PreviousNode == null || node.NextNode == null)
                {
                    // case of single list
                    root = null;
                }
                else
                {
                    // otherwise, root is next node.
                    var nextRoot = node.NextNode;

                    // single list.
                    if (nextRoot.NextNode == null)
                    {
                        nextRoot.PreviousNode = null;
                    }
                    else
                    {
                        nextRoot.PreviousNode = node.PreviousNode; // as last.
                    }

                    root = nextRoot;
                }
            }
            else
            {
                // node is not root, previous must exists
                node.PreviousNode!.NextNode = node.NextNode;

                if (node.NextNode != null)
                {
                    node.NextNode.PreviousNode = node.PreviousNode;
                }
                else
                {
                    // next does not exists, previous is last node so modify root
                    root!.PreviousNode = node.PreviousNode;
                }
            }
        }
    }

    public void Dispose()
    {
        lock (gate)
        {
            ThrowHelper.ThrowObjectDisposedIf(IsDisposed, typeof(MessageHandlerList<T>));

            root = null;
            isDisposed = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetVersion()
    {
        ulong currentVersion;
        if (version == ulong.MaxValue)
        {
            ResetAllHandlerVersion();
            currentVersion = 0;
        }
        else
        {
            currentVersion = version++;
        }
        return currentVersion;

        void ResetAllHandlerVersion()
        {
            lock (gate)
            {
                var node = root;
                while (node != null)
                {
                    node.Version = 0;
                    node = node.NextNode;
                }

                version = 1; // also reset version
            }
        }
    }
}
