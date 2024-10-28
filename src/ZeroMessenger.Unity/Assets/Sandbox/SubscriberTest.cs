using System;
using UnityEngine;
using VContainer;
using ZeroMessenger;

namespace Sandbox
{
    public class SubscriberTest : MonoBehaviour
    {
        [Inject] public IMessageSubscriber<TestMessage> Subscriber { get; set; }

        IDisposable disposable;

        void Start()
        {
            disposable = Subscriber.Subscribe(x =>
            {
                Debug.Log(x.Message);
            });
        }

        void OnDestroy()
        {
            disposable.Dispose();
        }
    }
}