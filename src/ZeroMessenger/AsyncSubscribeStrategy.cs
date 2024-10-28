namespace ZeroMessenger;

public enum AsyncSubscribeStrategy
{
    Sequential,
    Parallel,
    Switch,
    Drop,
}

internal sealed class SequentialAsyncMessageHandler<T>(AsyncMessageHandler<T> handler) : AsyncMessageHandler<T>
{
    readonly SemaphoreSlim publishLock = new(1, 1);

    protected override async ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        await publishLock.WaitAsync();
        try
        {
            await handler.HandleAsync(message, cancellationToken);
        }
        finally
        {
            publishLock.Release();
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        publishLock.Dispose();
    }
}

internal sealed class DropAsyncMessageHandler<T>(AsyncMessageHandler<T> handler) : AsyncMessageHandler<T>
{
    int flag;

    protected override async ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref flag, 1, 0) == 0)
        {
            try
            {
                await handler.HandleAsync(message, cancellationToken);
            }
            finally
            {
                Interlocked.Exchange(ref flag, 0);
            }
        }
    }
}

internal sealed class SwitchAsyncMessageHandler<T>(AsyncMessageHandler<T> handler) : AsyncMessageHandler<T>
{
    CancellationTokenSource? cts;

    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }

        cts = new CancellationTokenSource();
        var ct = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken).Token;

        return handler.HandleAsync(message, ct);
    }
}