
namespace ZeroMessenger;

internal sealed class FilteredMessageHandler<T>(MessageHandler<T> handler, IMessageFilter<T>[] filters) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return new FilterIterator(handler, filters).InvokeRecursiveAsync(message, cancellationToken);
    }

    protected override void DisposeCore()
    {
        handler.Dispose();
    }

    sealed class FilterIterator
    {
        readonly MessageHandler<T> handler;
        readonly IMessageFilter<T>[] filters;
        readonly Func<T, CancellationToken, ValueTask> invokeDelegate;

        int index;

        public FilterIterator(MessageHandler<T> handler, IMessageFilter<T>[] filters)
        {
            this.handler = handler;
            this.filters = filters;
            this.invokeDelegate = InvokeRecursiveAsync;
        }

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, invokeDelegate);
            }

            handler.Handle(message);
            return default;
        }

        bool MoveNextFilter(out IMessageFilter<T> next)
        {
            while (index < filters.Length)
            {
                next = filters[index];
                index++;
                return true;
            }

            next = default!;
            return false;
        }
    }
}

internal sealed class FilteredAsyncMessageHandler<T>(AsyncMessageHandler<T> handler, IMessageFilter<T>[] filters) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return new FilterIterator(handler, filters).InvokeRecursiveAsync(message, cancellationToken);
    }

    sealed class FilterIterator
    {
        readonly AsyncMessageHandler<T> handler;
        readonly IMessageFilter<T>[] filters;
        readonly Func<T, CancellationToken, ValueTask> invokeDelegate;

        int index;

        public FilterIterator(AsyncMessageHandler<T> handler, IMessageFilter<T>[] filters)
        {
            this.handler = handler;
            this.filters = filters;
            invokeDelegate = InvokeRecursiveAsync;
        }

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, invokeDelegate);
            }

            return handler.HandleAsync(message, cancellationToken);
        }

        bool MoveNextFilter(out IMessageFilter<T> next)
        {
            while (index < filters.Length)
            {
                next = filters[index];
                index++;
                return true;
            }

            next = default!;
            return false;
        }
    }
}