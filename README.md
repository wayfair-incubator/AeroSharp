# AeroSharp

[![OSS Template Version](https://img.shields.io/badge/OSS%20Template-0.3.5-7f187f.svg)](https://github.com/wayfair-incubator/oss-template/blob/main/CHANGELOG.md)
[![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg)](CODE_OF_CONDUCT.md)

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
    .WithSet("my_set")
    .UseMessagePackSerializer()
    .Build<MyDataType>();

await keyValueStore.WriteAsync("record_key", new MyDataType("some data"), CancellationToken.None);

KeyValuePair<string, MyDataType> keyValueResult = await keyValueStore.ReadAsync("record_key", CancellationToken.None);
// keyValueResult contains [ Key = "record_key", Value = MyDataType("some data") ]

var list = ListBuilder
    .Configure(clientProvider)
    .WithSet("my_set")
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
