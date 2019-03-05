# Contrib.IdentityServer4.KubernetesStore

[![NuGet package](https://img.shields.io/nuget/v/Contrib.IdentityServer4.KubernetesStore.svg)](https://www.nuget.org/packages/Contrib.IdentityServer4.KubernetesStore/)
[![Build status](https://img.shields.io/appveyor/ci/AXOOM/contrib-kubeclient-customresources.svg)](https://ci.appveyor.com/project/AXOOM/contrib-kubeclient-customresources)

Allows [IdentityServer4](http://docs.identityserver.io) to fetch Clients, API Resources, etc. using [Kubernetes Custom Resources](https://kubernetes.io/docs/concepts/extend-kubernetes/api-extension/custom-resources/) rather than a database.

## Usage

### Identity Server Project

Add the [`Contrib.IdentityServer4.KubernetesStore` NuGet package](https://www.nuget.org/packages/Contrib.IdentityServer4.KubernetesStore/) to your existing [IdentityServer4](http://docs.identityserver.io) project. You can then activate it in your `Startup.cs`:

```csharp
services.AddKubeClient(Configuration.GetSection("Kubernetes"))
        .AddIdentityServer(...)
        .AddKubernetesConfigurationStore();
```

This will default to connecting to the Kubernetes API using the Pod's service account. See the documentation of the [KubeClient library](https://github.com/tintoy/dotnet-kube-client) for more options.

### Kubernetes Resources

Register the [Custom Resource Definitions](crd.yaml) in your Kubernetes cluster:

    kubectl apply -f https://raw.githubusercontent.com/AXOOM/Contrib.IdentityServer4.KubernetesStore/develop/crd.yaml

You can then create Identity Server objects with Kubernetes resources.

#### Client

```yaml
apiVersion: contrib.identityserver.io/v1
kind: OauthClient
metadata:
  namespace: mynamespace
  name: myvendor-myapp
spec:
  #clientId: mynamespace-myvendor-myapp # Automatically generated from meta.namespace+name
  clientName: My App
  accessTokenType: reference
  allowAccessTokensViaBrowser: true
  requireConsent: false
  allowedGrantTypes:
    - implicit
  allowedScopes:
    - openid
    - profile
    - email
    - tenant
    - myvendor-myapp.api
  redirectUris:
    - https://www.example.com/
  allowedCorsOrigins:
    - https://www.example.com/
  frontChannelLogoutUri: https://www.example.com/
```

#### ApiResource

```yaml
apiVersion: contrib.identityserver.io/v1
kind: ApiResource
metadata:
  namespace: mynamespace
  name: myvendor-myapp
spec:
  #name: mynamespace-myvendor-myapp # Automatically generated from meta.namespace+name
  apiSecrets:
    - value: somesecret
  scopes:
    - name: myvendor-myapp.api
      userClaims:
        - access
```

## Development

Run `build.ps1` to compile the source code and create NuGet packages.
This script takes a version number as an input argument. The source code itself contains no version numbers. Instead version numbers should be determined at build time using [GitVersion](http://gitversion.readthedocs.io/).
