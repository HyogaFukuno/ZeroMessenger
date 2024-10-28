namespace ZeroMessenger.Internal;

// for Dependency Injection

[Preserve]
public sealed class MessageFilterProvider<T>(IEnumerable<MessageFilterBase> untypedFilters, IEnumerable<MessageFilter<T>> typedFilters)
{
    readonly MessageFilter<T>[] filters = untypedFilters
        .OfType<MessageFilter<T>>()
        .Concat(typedFilters)
        .Distinct()
        .ToArray();

    public IEnumerable<MessageFilter<T>> GetGlobalFilters()
    {
        return filters;
    }
}
