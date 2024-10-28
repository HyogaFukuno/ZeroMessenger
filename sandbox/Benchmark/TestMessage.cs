
using MediatR;
using VitalRouter;

public class TestMessage : PubSubEvent<TestMessage>, INotification, ICommand
{
}