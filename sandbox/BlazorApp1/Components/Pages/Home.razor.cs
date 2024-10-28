
using Microsoft.AspNetCore.Components;
using ZeroMessenger;

namespace BlazorApp1.Components.Pages;

public partial class Home
{
    [Inject]
    public required IMessagePublisher<Message> Publisher { get; init; }

    [SupplyParameterFromForm]
    public string TextInput { get; set; } = "";

    public void OnSubmit()
    {
        Publisher.Publish(new Message(TextInput));
    }
}