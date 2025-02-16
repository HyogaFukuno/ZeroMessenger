using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ZeroMessenger;

internal sealed class FilteredMessageHandler<T>(MessageHandler<T> handler, IMessageFilter<T>[] filters) : AsyncMessageHandler<T>
{
    protected override async ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        var iterator = FilterIterator.Rent(handler, filters);

        try
        {
            await iterator.InvokeRecursiveAsync(message, cancellationToken);
        }
        finally
        {
            FilterIterator.Return(iterator);
        }
    }

    protected override void DisposeCore()
    {
        handler.Dispose();
    }

    sealed class FilterIterator
    {
        static readonly ConcurrentStack<FilterIterator> pool = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilterIterator Rent(MessageHandler<T> handler, IMessageFilter<T>[] filters)
        {
            if (!pool.TryPop(out var iterator))
            {
                iterator = new();
            }

            iterator.handler = handler;
            iterator.filters = filters;

            return iterator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(FilterIterator iterator)
        {
            iterator.handler = null;
            iterator.filters = null;
            pool.Push(iterator);
        }

        MessageHandler<T>? handler;
        IMessageFilter<T>[]? filters;
        int index;

        readonly Func<T, CancellationToken, ValueTask> invokeDelegate;

        public FilterIterator()
        {
            invokeDelegate = InvokeRecursiveAsync;
        }

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, invokeDelegate);
            }

            handler!.Handle(message);
            return default;
        }

        bool MoveNextFilter(out IMessageFilter<T> next)
        {
            while (index < filters!.Length)
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
    protected override async ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        var iterator = FilterIterator.Rent(handler, filters);

        try
        {
            await iterator.InvokeRecursiveAsync(message, cancellationToken);
        }
        finally
        {
            FilterIterator.Return(iterator);
        }
    }

    sealed class FilterIterator
    {
        static readonly ConcurrentStack<FilterIterator> pool = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FilterIterator Rent(AsyncMessageHandler<T> handler, IMessageFilter<T>[] filters)
        {
            if (!pool.TryPop(out var iterator))
            {
                iterator = new();
            }

            iterator.handler = handler;
            iterator.filters = filters;

            return iterator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(FilterIterator iterator)
        {
            iterator.handler = null;
            iterator.filters = null;
            pool.Push(iterator);
        }

        AsyncMessageHandler<T>? handler;
        IMessageFilter<T>[]? filters;
        int index;

        readonly Func<T, CancellationToken, ValueTask> invokeDelegate;

        public FilterIterator()
        {
            invokeDelegate = InvokeRecursiveAsync;
        }

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, invokeDelegate);
            }

            return handler!.HandleAsync(message, cancellationToken);
        }

        bool MoveNextFilter(out IMessageFilter<T> next)
        {
            while (index < filters!.Length)
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