using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using MessagePipe;
using Microsoft.Extensions.DependencyInjection;
using R3;
using ZeroMessenger;

[MemoryDiagnoser]
public class PublishBenchmark
{
    class CSharpEventWrapper
    {
        public event Action<TestMessage>? Event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TestMessage message)
        {
            Event?.Invoke(message);
        }
    }

    const int SubscribeCount = 16;

    MessagePipe.IPublisher<TestMessage> messagePipePublisher = default!;
    MessagePipe.ISubscriber<TestMessage> messagePipeSubscriber = default!;

    ZeroMessenger.IMessagePublisher<TestMessage> zeroMessengerPublisher = default!;
    ZeroMessenger.IMessageSubscriber<TestMessage> zeroMessengerSubscriber = default!;

    R3.Subject<TestMessage> r3Subject = default!;

    CSharpEventWrapper csharpEvent = default!;

    [IterationSetup]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessagePipe();
        var provider = serviceCollection.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);

        messagePipePublisher = GlobalMessagePipe.GetPublisher<TestMessage>();
        messagePipeSubscriber = GlobalMessagePipe.GetSubscriber<TestMessage>();

        var zeroBroker = new ZeroMessenger.MessageBroker<TestMessage>();
        zeroMessengerPublisher = zeroBroker;
        zeroMessengerSubscriber = zeroBroker;

        r3Subject = new();
        csharpEvent = new();

        static void Method(int i)
        {
            
        }

        for (int i = 0; i < SubscribeCount; i++)
        {
            messagePipeSubscriber.Subscribe(x => Method(i));
        }

        for (int i = 0; i < SubscribeCount; i++)
        {
            zeroMessengerSubscriber.Subscribe(i, (x, state) => Method(state));
        }

        for (int i = 0; i < SubscribeCount; i++)
        {
            r3Subject.Subscribe(i, (x, state) => Method(state));
        }

        for (int i = 0; i < SubscribeCount; i++)
        {
            csharpEvent.Event += x => Method(i);
        }

        GC.Collect();
    }

    [Benchmark(Description = "Publish (MessagePipe)")]
    public void Benchmark_MessagePipe()
    {
        messagePipePublisher.Publish(default);
    }

    [Benchmark(Description = "Publish (ZeroMessenger)")]
    public void Benchmark_ZeroMessenger()
    {
        zeroMessengerPublisher.Publish(default);
    }

    [Benchmark(Description = "Publish (R3.Subject)")]
    public void Benchmark_R3Subject()
    {
        r3Subject.OnNext(default);
    }

    [Benchmark(Description = "Publish (C# event)")]
    public void Benchmark_Event()
    {
        csharpEvent.Invoke(default);
    }
}