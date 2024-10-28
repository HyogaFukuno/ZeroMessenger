

namespace ZeroMessenger;

public sealed class PredicateFilter<T>(Predicate<T> predicate) : MessageFilter<T>
{
    public override ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
    {
        if (predicate(message))
        {
            return next(message, cancellationToken);
        }
        else
        {
            return default;
        }
    }
}