
using ZeroMessenger;

public class LogService(IMessageSubscriber<Message> subscriber) : IHostedService
{
    IDisposable? subscription;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Start:");

        subscription = subscriber.Subscribe(x =>
        {
            Console.WriteLine($"Received: {x.Text}");
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop:");

        subscription?.Dispose();
        return Task.CompletedTask;
    }
}