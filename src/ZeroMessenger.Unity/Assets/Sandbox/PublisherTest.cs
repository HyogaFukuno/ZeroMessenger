using UnityEngine;
using UnityEngine.UI;
using VContainer;
using ZeroMessenger;

namespace Sandbox
{
    public class PublisherTest : MonoBehaviour
    {
        [Inject] public IMessagePublisher<TestMessage> Publisher { get; set; }

        [SerializeField] Button button;
        [SerializeField] InputField inputField;

        void Start()
        {
            button.onClick.AddListener(() =>
            {
                Publisher.Publish(new()
                {
                    Message = inputField.text,
                });
            });
        }
    }
}