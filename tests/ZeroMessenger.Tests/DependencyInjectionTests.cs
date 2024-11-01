using Microsoft.Extensions.DependencyInjection;
using ZeroMessenger.DependencyInjection;

namespace ZeroMessenger.Tests;

public class DependencyInjectionTests
{
    ServiceProvider serviceProvider = default!;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();
        services.AddZeroMessenger();
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        serviceProvider.Dispose();
    }

    [Test]
    public void Test_DI_PublishSubscribe()
    {
        var publisher = serviceProvider.GetRequiredService<IMessagePublisher<int>>();
        var subscriber = serviceProvider.GetRequiredService<IMessageSubscriber<int>>();
        var result = 0;

        var subscription = subscriber.Subscribe(x =>
        {
            result = x;
        });

        for (int i = 0; i < 10000; i++)
        {
            publisher.Publish(i);
            Assert.That(result, Is.EqualTo(i));
        }

        result = -1;
        subscription.Dispose();

        publisher.Publish(100);
        Assert.That(result, Is.EqualTo(-1));
    }

    [Test]
    public async Task Test_DI_PublishAsyncSubscribeAwait()
    {
        var publisher = serviceProvider.GetRequiredService<IMessagePublisher<int>>();
        var subscriber = serviceProvider.GetRequiredService<IMessageSubscriber<int>>();
        var result = 0;

        var subscription = subscriber.SubscribeAwait(async (x, ct) =>
        {
            await Task.Delay(1, ct);
            result = x;
        });

        for (int i = 0; i < 100; i++)
        {
            await publisher.PublishAsync(i);
            Assert.That(result, Is.EqualTo(i));
        }

        result = -1;
        subscription.Dispose();

        await publisher.PublishAsync(100);
        Assert.That(result, Is.EqualTo(-1));
    }
}