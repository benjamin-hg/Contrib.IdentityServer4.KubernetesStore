using System.Collections.Generic;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public interface ICustomResourceWatcher<TSpec>
    {
        IEnumerable<TSpec> Resources { get; }
    }
}
