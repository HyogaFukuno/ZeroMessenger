namespace ZeroMessenger.Internal;

// for Dependency Injection

[Preserve]
[method: Preserve]
public sealed class MessageFilterProvider<T>(IEnumerable<IMessageFilterBase> untypedFilters, IEnumerable<IMessageFilter<T>> typedFilters)
{
    readonly IMessageFilter<T>[] filters = untypedFilters
        .OfType<IMessageFilter<T>>()
        .Concat(typedFilters)
        .Distinct()
        .ToArray();

    public IEnumerable<IMessageFilter<T>> GetGlobalFilters()
    {
        return filters;
    }
}
