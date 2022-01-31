# AeroSharp

[![OSS Template Version][oss-template-shield]][oss-template-url]

[![Status][github-status-shield]][github-status-url]
[![Coverage][coverage-shield]][coverage-url]

[![Version][nuget-version-shield]][nuget-url]
[![Downloads][nuget-downloads-shield]][nuget-url]

[![Contributor Covenant][contributor-covenant-shield]][contributor-covenant-url]
[![Contributors][contributors-shield]][contributors-url]
[![Commits][last-commit-shield]][last-commit-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]

[![MIT License][license-shield]][license-url]

## About The Project

AeroSharp is a wrapper around the .NET Aerospike client. This library provides a
variety of generic methods for storing and retrieving data on Aerospike while
handling serialization, client-side compression, policy validation, and other
features under the hood.

## Installation

Get the latest version with NuGet:

```shell
// NuGet package manager console
Install-Package AeroSharp
```

## Usage

In AeroSharp, accessing data stored in Aerospike (e.g. blobs or
[lists](https://www.aerospike.com/docs/guide/cdt-list.html)) generally involves
two steps:

1. building a client provider that specifies how connections to Aerospike are established (e.g. cluster connection strings, credentials), and
2. building a data access object that provides an easy-to-use interface for interacting with the Aerospike database.

In general, you should only need to build one client provider and the underlying
[Aerospike client](https://www.aerospike.com/docs/client/csharp/usage/connect_sync.html)
will maintain connections to all nodes in the Aerospike cluster. Once a client
provider is built, you can then build a variety of data access objects to store
and retrieve your various data types in Aerospike.

For example, this code builds a client provider that connects to a local
instance of Aerospike and then writes and reads a blob of a custom data type
(via KeyValueStore) and appends a few items to a list (via List).

```C#
var clientProvider = ClientProviderBuilder
    .Configure()
    .WithBootstrapServers(new string[] { "localhost" })
    .WithoutCredentials()
    .Build(); // Only do this once.

var keyValueStore = KeyValueStoreBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build<MyDataType>();

await keyValueStore.WriteAsync("record_key", new MyDataType("some data"), CancellationToken.None);

KeyValuePair<string, MyDataType> keyValueResult = await keyValueStore.ReadAsync("record_key", CancellationToken.None);
// keyValueResult contains [ Key = "record_key", Value = MyDataType("some data") ]

var list = ListBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .WithKey("list_record_key")
    .Build<MyDataType>();

await list.AppendAsync(new MyDataType("list item 1"), CancellationToken.None);
await list.AppendAsync(new MyDataType("list item 2"), CancellationToken.None);

IEnumerable<MyDataType> listResult = await list.ReadAllAsync(CancellationToken.None);
// listResult contains [ MyDataType("list item 1"), MyDataType("list item 2") ]
```

Full library documentation can be found at the [docs site](https://wayfair-incubator.github.io/AeroSharp/)

Code examples can be found in the [examples directory](./examples)

## Roadmap

See the [open issues](https://github.com/wayfair-incubator/AeroSharp/issues) for a list of proposed features (and known issues).

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**. For detailed contributing guidelines, please see [CONTRIBUTING.md](CONTRIBUTING.md)

## License

Distributed under the `Apache 2.0` License. See [LICENSE](LICENSE) for more information.

## Contact

Project Link: [https://github.com/wayfair-incubator/AeroSharp](https://github.com/wayfair-incubator/AeroSharp)

## References

- [Aerospike](https://www.aerospike.com/docs/)
- [Aerospike C# Client](https://docs.aerospike.com/docs/client/csharp/index.html)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/wayfair-incubator/AeroSharp.svg?style=for-the-badge
[contributors-url]: https://github.com/wayfair-incubator/AeroSharp/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/wayfair-incubator/AeroSharp.svg?style=for-the-badge
[forks-url]: https://github.com/wayfair-incubator/AeroSharp/network/members
[stars-shield]: https://img.shields.io/github/stars/wayfair-incubator/AeroSharp.svg?style=for-the-badge
[stars-url]: https://github.com/wayfair-incubator/AeroSharp/stargazers
[issues-shield]: https://img.shields.io/github/issues/wayfair-incubator/AeroSharp.svg?style=for-the-badge
[issues-url]: https://github.com/wayfair-incubator/AeroSharp/issues
[license-shield]: https://img.shields.io/github/license/wayfair-incubator/AeroSharp.svg?style=for-the-badge
[license-url]: https://github.com/wayfair-incubator/AeroSharp/blob/main/LICENSE
[github-status-shield]: https://img.shields.io/github/checks-status/wayfair-incubator/AeroSharp/main?style=for-the-badge
[github-status-url]: https://github.com/wayfair-incubator/AeroSharp/actions
[coverage-shield]: https://img.shields.io/codecov/c/github/wayfair-incubator/AeroSharp?style=for-the-badge
[coverage-url]: https://app.codecov.io/gh/wayfair-incubator/AeroSharp
[nuget-version-shield]: https://img.shields.io/nuget/v/AeroSharp?style=for-the-badge
[nuget-downloads-shield]: https://img.shields.io/nuget/dt/AeroSharp?style=for-the-badge
[nuget-url]: https://www.nuget.org/packages/AeroSharp/
[last-commit-shield]: https://img.shields.io/github/last-commit/wayfair-incubator/AeroSharp?style=for-the-badge
[last-commit-url]: https://github.com/wayfair-incubator/AeroSharp/commits/main
[oss-template-shield]: https://img.shields.io/badge/OSS%20Template-0.3.5-7f187f.svg?style=for-the-badge
[oss-template-url]: https://github.com/wayfair-incubator/oss-template/blob/main/CHANGELOG.md
[contributor-covenant-shield]: https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg?style=for-the-badge
[contributor-covenant-url]: https://github.com/wayfair-incubator/AeroSharp/blob/main/CODE_OF_CONDUCT.md
