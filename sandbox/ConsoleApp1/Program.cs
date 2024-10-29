using Microsoft.Extensions.DependencyInjection;
using ZeroMessenger;
using ZeroMessenger.DependencyInjection;

var services = new ServiceCollection();

services.AddZeroMessenger(configure =>
{
    configure.AddFilter<LoggingFilter<TestMessage>>();
});

var provider = services.BuildServiceProvider();
var publisher = provider.GetRequiredService<IMessagePublisher<TestMessage>>();
var subscriber = provider.GetRequiredService<IMessageSubscriber<TestMessage>>();

var subscription = subscriber
    .Subscribe(x =>
    {
        Console.WriteLine(x.Value);
    });

for (int i = 0; i < 10; i++)
{
    publisher.Publish(new(i));
}

subscription.Dispose();

public readonly record struct TestMessage(int Value);

public class LoggingFilter<T> : IMessageFilter<T>
{
    public async ValueTask InvokeAsync(T message, CancellationToken cancellationToken, Func<T, CancellationToken, ValueTask> next)
    {
        Console.WriteLine("Before");
        await next(message, cancellationToken);
        Console.WriteLine("After");
    }
}