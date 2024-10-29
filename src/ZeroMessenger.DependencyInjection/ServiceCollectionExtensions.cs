using Microsoft.Extensions.DependencyInjection;
using ZeroMessenger.Internal;

namespace ZeroMessenger.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddZeroMessenger(this IServiceCollection services)
    {
        AddZeroMessenger(services, _ => { });
    }

    public static void AddZeroMessenger(this IServiceCollection services, Action<MessageBrokerBuilder> configuration)
    {
        var builder = new MessageBrokerBuilder(services);
        configuration(builder);

        services.AddSingleton(typeof(MessageBroker<>));
        services.AddSingleton(typeof(IMessagePublisher<>), typeof(MessageBrokerPublisher<>));
        services.AddSingleton(typeof(IMessageSubscriber<>), typeof(MessageBrokerSubscriber<>));
        services.AddSingleton(typeof(MessageFilterProvider<>));
    }
}

public readonly struct MessageBrokerBuilder(IServiceCollection services)
{
    public void AddFilter<TFilter>() where TFilter : class, IMessageFilterBase
    {
        services.AddTransient<IMessageFilterBase, TFilter>();
    }

    public void AddFilter<TFilter>(TFilter filter) where TFilter : class, IMessageFilterBase
    {
        services.AddSingleton<IMessageFilterBase>(filter);
    }

    public void AddFilter(Type type)
    {
        services.AddTransient(typeof(IMessageFilter<>), type);
    }
}