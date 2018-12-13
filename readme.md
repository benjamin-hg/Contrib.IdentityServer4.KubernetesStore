
# Contrib.IdentityServer4.KubernetesStore

[![NuGet package](https://img.shields.io/nuget/v/Contrib.IdentityServer4.KubernetesStore.svg)](https://www.nuget.org/packages/Contrib.IdentityServer4.KubernetesStore/)
[![Build status](https://img.shields.io/appveyor/ci/AXOOM/contrib-kubeclient-customresources.svg)](https://ci.appveyor.com/project/AXOOM/contrib-kubeclient-customresources)

Provides custom stores for [IdentityServer4](http://docs.identityserver.io) that fetch Clients, API Resources, etc. from Kubernetes Resources.

## Usage

Register the [Custom Resource Definitions](https://kubernetes.io/docs/concepts/extend-kubernetes/api-extension/custom-resources/) in your Kubernetes cluster:

    git clone https://github.com/AXOOM/Contrib.IdentityServer4.KubernetesStore.git
    cd Contrib.IdentityServer4.KubernetesStore/crd
    kubectl apply -f *.yaml

Reference the [`Contrib.IdentityServer4.KubernetesStore` NuGet package](https://www.nuget.org/packages/Contrib.IdentityServer4.KubernetesStore/) in your [IdentityServer4](http://docs.identityserver.io) project and add something like this to your `Startup.cs`:

```csharp
services.AddIdentityServer(...)
        .AddKubernetesConfigurationStore();
```

## Development

Run `build.ps1` to compile the source code and create NuGet packages.
This script takes a version number as an input argument. The source code itself contains no version numbers. Instead version numbers should be determined at build time using [GitVersion](http://gitversion.readthedocs.io/).
