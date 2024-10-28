using VContainer;
using VContainer.Unity;
using ZeroMessenger.VContainer;

namespace Sandbox
{
    public class SandboxLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.AddZeroMessenger();
        }
    }
}