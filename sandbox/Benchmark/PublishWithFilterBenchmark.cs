using BenchmarkDotNet.Attributes;
using MessagePipe;
using R3;
using VitalRouter;
using ZeroMessenger;

[MemoryDiagnoser]
[InvocationCount(10000)]
public class PublishWithFilterBenchmark
{
    const int SubscribeCount = 8;

    ZeroMessenger.MessageBroker<TestMessage> zeroMessengerBroker = default!;
    TestMessage message = new();

    [IterationSetup]
    public void Setup()
    {
        static void Method(int i)
        {

        }

        zeroMessengerBroker = new ZeroMessenger.MessageBroker<TestMessage>();
        zeroMessengerBroker.AddFilter(x => true);

        for (int i = 0; i < SubscribeCount; i++)
        {
            zeroMessengerBroker.Subscribe(i, (x, state) => Method(state));
        }
    }

    [Benchmark(Description = "Publish (ZeroMessenger)")]
    public void Benchmark_ZeroMessenger()
    {
        zeroMessengerBroker.Publish(message);
    }
}