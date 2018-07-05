using System;
using System.Collections.Generic;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public interface ICustomResourceWatcher<TSpec>
    {
        IEnumerable<TSpec> Resources { get; }
        IEnumerable<CustomResource<TSpec>> RawResources { get; }
        event EventHandler<Exception> OnConnectionError;
        event EventHandler OnConnected;
        void StartWatching();
    }
}
