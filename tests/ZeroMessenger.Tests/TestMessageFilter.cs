
namespace ZeroMessenger.Tests;

public class TestMessageFilter<T>(Func<T, T> preprocess) : IMessageFilter<T>
{
    public async ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
    {
        message = preprocess(message);
        await next(message, cancellationToken);
    }
}

public class IgnoreMessageFilter<T> : IMessageFilter<T>
{
    public ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
    {
        // ignore message
        return default;
    }
}