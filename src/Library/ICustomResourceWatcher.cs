using System;
using System.Collections.Generic;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public interface ICustomResourceWatcher<out TSpec>
    {
        IEnumerable<TSpec> Resources { get; }
        event EventHandler<Exception> OnConnectionError;
        event EventHandler OnConnected;
        void StartWatching();
    }
}
