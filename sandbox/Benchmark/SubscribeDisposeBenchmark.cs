using BenchmarkDotNet.Attributes;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using R3;
using ZeroMessenger;

[MemoryDiagnoser]
public class SubscribeDisposeBenchmark
{
    const int Count = 10000;

    IDisposable[] disposables = default!;

    MessagePipe.ISubscriber<TestMessage> messagePipeSubscriber = default!;
    ZeroMessenger.IMessageSubscriber<TestMessage> zeroMessengerSubscriber = default!;
    R3.Subject<TestMessage> r3Subject = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessagePipe();
        var provider = serviceCollection.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);
    }

    [IterationSetup]
    public void Setup()
    {
        disposables = new IDisposable[Count];
        messagePipeSubscriber = GlobalMessagePipe.GetSubscriber<TestMessage>();
        zeroMessengerSubscriber = new ZeroMessenger.MessageBroker<TestMessage>();
        r3Subject = new();

        GC.Collect();
    }

    [Benchmark(Description = "Subscribe (MessagePipe)")]
    public void Benchmark_MessagePipe()
    {
        for (int i = 0; i < Count; i++)
        {
            disposables[i] = messagePipeSubscriber.Subscribe(x => { });
        }
        for (int i = 0; i < Count; i++)
        {
            disposables[i].Dispose();
        }
    }

    [Benchmark(Description = "Subscribe (ZeroMessenger)")]
    public void Benchmark_ZeroMessenger()
    {
        for (int i = 0; i < Count; i++)
        {
            disposables[i] = zeroMessengerSubscriber.Subscribe(x => { });
        }
        for (int i = 0; i < Count; i++)
        {
            disposables[i].Dispose();
        }
    }


    [Benchmark(Description = "Subscribe (R3.Subject)")]
    public void Benchmark_R3Subject()
    {
        for (int i = 0; i < Count; i++)
        {
            disposables[i] = r3Subject.Subscribe(x => { });
        }
        for (int i = 0; i < Count; i++)
        {
            disposables[i].Dispose();
        }
    }
}