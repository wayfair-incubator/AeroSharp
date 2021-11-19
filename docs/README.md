# AeroSharp Docs

- [Getting Started](#getting-started)
- [Client Provider](#client-provider)
- [Serialization](#serialization)
- [Data Context](#data-context)
- [Data Access Types](#data-access-types)
  - [Configuration](#configuration)
  - [KeyValueStore](#keyvaluestore)
  - [List](#list)
  - [Operator](#operator)
  - [General](#general)
    - [SetTruncator](#settruncator)
    - [KeyOperator](#keyoperator)

## Getting Started

If you're not familiar with Aerospike, take a look at the [official documentation](https://www.aerospike.com/docs/) before using this library.

In this library, access to data stored in Aerospike (e.g. blobs or [lists](https://www.aerospike.com/docs/guide/cdt-list.html)) generally involves two steps:

1. building a [client provider](#client-provider) that specifies how connections to Aerospike are established (e.g. cluster connection strings, credentials), and
2. building a [data access object](#data-access-types) that provides an easy-to-use interface for interacting with the Aerospike database.

In general, you should only need to build one client provider and the underlying [Aerospike client](https://www.aerospike.com/docs/client/csharp/usage/connect_sync.html) will maintain connections to all nodes in the Aerospike cluster. Once a client provider is built, you can then build a variety of data access objects to store and retrieve your various data types in Aerospike.

For example, this code builds a client provider that connects to a local instance of Aerospike and then writes and reads a blob of a custom data type (via [KeyValueStore](#keyvaluestore)) and appends a few items to a list (via [List](#list)).

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

## Client Provider

Aerospike provides a [client](https://www.aerospike.com/docs/client/csharp/) that maintains a pool of connections to an Aerospike cluster. The client needs to know the location of at least one node in the cluster and can discover the remaining nodes. The client can also accept username/password credentials as well as other parameters (e.g. connection timeout).

This library uses a [client provider](../src/AeroSharp/Connection/IClientProvider.cs) to give the data access objects access to an Aerospike client. Use the `ClientProviderBuilder` to configure and build a client provider, e.g.:

```C#
var clientProvider = ClientProviderBuilder 
    .Configure()
    .WithBootstrapServers(new string[] { "localhost" })
    .WithUsernameAndPassword("my_username", "my_password")
    .WithConnectionConfiguration(new ConnectionConfiguration { ConnectionTimeout = TimeSpan.FromMinutes(1) })
    .Build();
```

`WithBootstrapServers(...)` is shorthand for `WithConnectionContext(...)` that assumes a default port (3000).

`WithConnectionConfiguration` is optional and exposes [ClientPolicy](https://www.aerospike.com/docs/guide/policies.html) parameters, with the exception of the username and password credentials which we require in a separate configuration step.

❗ **In most cases you only need one client provider!** The underlying Aerospike client is thread-safe and will maintain connection threads for your entire parallel/concurrent application. You can store a reference to your client provider in a singleton or register it as such with your DI container, for example.

### Implementing `IClientProvider`

You may need to implement custom connection logic, such as falling back to a separate Aerospike cluster when feature toggle is toggled. You can achieve this by implementing
the `IClientProvider` interface and passing your client provider to the data access object builders' `Configure(...)` method.

The recommended approach is to use `ClientProviderBuilder` to build the various connections that you will need, and put them in a wrapper that implements `IClientProvider`. Keep in mind that _connections are not established until the first request is made_, so you can safely build client providers in advance without actually connecting to any Aerospike cluster.

If you _must_ construct your own [AerospikeClient](https://www.aerospike.com/docs/client/csharp/) or [AsyncClient](https://www.aerospike.com/docs/client/csharp/usage/async/connect_async.html), keep in mind that the client providers return an instance of `ClientWrapper`. `ClientWrapper` simply accepts an Aerospike client in its constructor and only exposes it to internal classes.

## Serialization

The goal of this library is to provide performant access to Aerospike in such a way that makes it hard to corrupt your data or find yourself reading unexpected bytes. This is accomplished by factoring out as many request parameters as possible into one-time configuration of data access objects; and by handling the serialization of your data types under the hood.

We provide built-in serializers:

- [MessagePack](https://github.com/neuecc/MessagePack-CSharp)
- [ProtoBuf](https://github.com/protobuf-net/protobuf-net)

Custom serializers can also be provided by implementing `ISerializer`.

### Using built-in serializers

In general, to use built-in components call builder methods like `UseMessagePackSerializer()` or `UseProtobufSerializer()`. Both of these built-in libraries feature C# contract definitions via attributes. For example, you can define your serializable data types like this:

```C#
// MessagePack
[MessagePackObject]
class MyType
{
    [Key(0)]
    public int Value { get; set; }
    [Key(1)]
    public string Text { get; set; }
}

// Protobuf
[ProtoContract]
class MyType
{
    [ProtoMember(1)]
    public int Value { get; set; }
    [ProtoMember(2)]
    public string Text { get; set; }
}
```

Based on some benchmarking, we recommend using MessagePack.

## Compressors

After serialization, the resulting bytes can be compressed (or otherwise transformed) by supplying an `ICompressor`. We provide built-in compressors:

- [LZ4](https://github.com/MiloszKrajewski/lz4net)

## Data Context

Most data access types require a data context. The data context is simply the namespace and set that the object will access, and is generally configured with `WithDataContext(new DataContext("namespace", "set"))`.

⚠️ Aerospike sets are created the first time you write to them, so care should be taken before writing new data to production.
⚠️ There's nothing stopping you from writing to a set that isn't configured with its own credentials. So, be sure you never accidentally write to anyone else's set!

## Data Access Types

The goal of this library is to provide performant access to Aerospike in such a way that makes it hard to corrupt your data or find yourself reading unexpected bytes. This is accomplished by factoring out as many request parameters as possible into one-time configuration of data access objects; and by handling the serialization of your data types under the hood. Each data access type is designed to suit a different access pattern.

## Configuration

Aerospike offers many different request behaviors that are configurable through [policies](https://www.aerospike.com/docs/guide/policies.html). This library uses configuration objects (e.g. `ReadConfiguration`, `WriteConfiguration`) to expose policy parameters. These are generally configured with builder methods, e.g.:

```C#
var keyValueStore = KeyValueStoreBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .WithReadConfiguration(new ReadConfiguration
    {
        RetryCount = 2,
        // ...set other configuration parameters or leave them as their default values
    })
    .Build<MyDataType>(); // Access to one bin with a default bin name
```

Supplying a configuration object is optional--default values will work for development in most cases and can be tuned when your service is nearing production.

### Configuration overrides

In some cases, certain requests may require a different configuration than what was provided when the data access object was built. In these cases, configurations can be overridden using `.Override()`.

_This feature is currently only implemented on the `KeyValueStore` data access type._

For example:

```C#
var keyValueStore = KeyValueStoreBuilder.Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .WithWriteConfiguration(new WriteConfiguration
    {
        RecordExistsAction = RecordExistsAction.CreateOnly
    })
    .Build<MyType>();

// Do things that only operate on non-existent keys
...

// Override to operate once on keys that already exist
await keyValueStore
    .Override((WriteConfiguration config) => {
        // `config` object has values from initial configuration--only set parameters that need to change
        config.RecordExistsAction = RecordExistsAction.UpdateOnly;
        return config;
    })
    .WriteAsync("key", value, token);
```

## KeyValueStore

`IKeyValueStore<T>` provides a simple interface for writing data as blobs to Aerospike records and reading them back in batches. For example:

```C#
await keyValueStore.WriteAsync("record_key_1", myData1, CancellationToken.None);
await keyValueStore.WriteAsync("record_key_2", myData2, CancellationToken.None);
var result = await keyValueStore.ReadAsync(new [] { "record_key_1", "record_key_2" }, CancellationToken.None); 
// result is an array of key-value pairs containing { "record_key_1", myData1 } and { "record_key_2", myData2 }
```

This interface comes in two flavors: one more safe and one more flexible.

The safe version is configured with a data type and a bin, and it will only read or write data of that type from the specified bin. In our experience, this is most common usage of key-value storage in Aerospike. The flexible version can read or write data of any type in any bin.

### "Safe" KeyValueStore access

Often, different parts of your code need to only read or write data of a single type (such as a repository class). In these cases, configure a `KeyValueStore` access object to read or write only that type of data.
Configure a key-value store data access object with `KeyValueStoreBuilder`:

```C#
var keyValueStore = KeyValueStoreBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build<MyDataType>(); // Access to one bin with a default name

// Write some data
MyDataType data = ... // get data to write from somewhere
await keyValueStore.WriteAsync("some_key", data, cancellationToken);

// Read the data
var readResult = keyValuestore.ReadAsync("some_key", cancellationToken);
// readResult is a key-value pair of type <string, MyDataType>
```

The final call to `Build<T>` is overloaded to use a default bin name or a given bin name. This interface currently supports access of up to three bins. For example:

```C#
// Configure a KeyValueStore to operate on two bins
var keyValueStore = KeyValueStoreBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build<MyDataType, OtherType>();

// Write some data
MyDataType data = ... // get data to write from somewhere
OtherType otherData = ... // get some more data
await keyValueStore.WriteAsync("some_key", data, otherData, cancellationToken);

// Read the data
var readResult = keyValuestore.ReadAsync("some_key", cancellationToken); 
// readResult contains a tuple of type (string, MyDataType, OtherType) where the first item is the record key
```

#### A tangent on Aerospike bins

Are you sure you need more than one bin? Bins are not SQL columns. That is, you can't relate records using values in bins. If you are only ever reading and writing one bin per request, you can just as easily write your data into different records. One common approach is to build key prefixes that differentiate between the different data types stored in a set. i.e.,

```text
my_service.birthday.Will
my_service.favorite_food.Will
my_service.favorite_color.Will
```

In this example, data for Will is stored in three records.

If you have a complex object with many properties and you only want to read and write it as a whole, just serialize that object into one bin. You can combine different data types by building a wrapper class to contain them both, or use a value tuple (e.g. `.Build<(MyDataType, MyOtherDataType)>()`).

Keep in mind that Aerospike records have a size limit. Breaking your data into mutiple records is not only easy to do, but it may save you a headache down the road when your data size starts to grow.

### "Flexible" KeyValueStore access

`IKeyValueStore` provides a more flexible interface that allows you to specify the data type and bin per read and write operation. In cases where you have a diverse set of data types to store in Aerospike that are related to some common behavior (i.e. accessed from the same place in your code), configure a flexible `KeyValueStore` by omitting the type parameter when calling `Build()`:

```C#

var keyValueStore = KeyValueStoreBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build();

// Write some data
MyDataType data = ... // get data to write from somewhere
await keyValueStore.WriteAsync("some_key", "bin", data, cancellationToken); // Generic parameter is inferred

// Read the data
var readResult = keyValuestore.ReadAsync<MyDataType>("some_key", "bin", cancellationToken);
// readResult is a key-value pair of type <string, MyDataType>
```

### Read-Modify-Write Transactions

The Read-Modify-Write approach is designed to permit reading a record into memory, updating it, and then writing it with changes in a thread-safe, transactional manner. We do this by performing the write only if the generation is equal to that at the time of the read. If there was an interim change, the write will fail and the entire Read-Write-Modify transaction pattern may be retried.

`KeyValueStoreBuilder` allows for providing a `ReadModifyWritePolicy` to configure the retry behavior:

```C#
var readModifyWritePolicy = new ReadModifyWritePolicy
{
    MaxRetries = 5,
    WaitTimeInMilliseconds = 10,
    WithExponentialBackoff = true
};

var keyValueStore = KeyValueStoreBuilder
    .Configure(_clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .WithReadModifyWriteConfiguration(readModifyWritePolicy)
    .Build<MyType>("some_bin");
```

The `KeyValueStore` can then be used to `ReadModifyWriteAsync`. Note that this requires the user to specify both an `addOperation` and `updateOperation`, like so:

```C#
var addOperation = new Func<MyType>(() =>
{
    return new MyType
    {
        Text = "Hello!",
        Value = 2
    };
});

var updateOperation = new Func<MyType, MyType>((x) =>
{
    x.Value += 2;
    return x;
});

await keyValueStore.ReadModifyWriteAsync(
    "some_key", 
    addOperation, 
    updateOperation, 
    timeToLive: TimeSpan.FromHours(5), 
    CancellationToken.None
);
```

This mimics the AddOrUpdate method on the ConcurrentDictionary object in C#. In this way, the user does not have to care whether the record already exists or not: the library will handle creating or updating the record appropriately.

### Plugins

The `IKeyValueStorePlugin` interface allows you to write hooks to execute on read or write actions, like so:

```C#
    public class MyKeyValueStorePlugin : IKeyValueStorePlugin
    {
        public Task OnWriteCompletedAsync(DataContext dataContext, string key, Bin[] bins, Type[] types, TimeSpan duration,
            CancellationToken cancellationToken)
        {
            return textLogger.WriteAsync($"Successfully wrote to {dataContext.Set}!");
        }
        ...
    }
```

Custom plugin(s) can then be used by adding them to the `KeyValueStore` during configuration:

```C#
 var keyValueStore = KeyValueStoreBuilder.Configure(clientProvider)
     .WithDataContext(new DataContext("my_namespace", "my_set"))
     .UseMessagePackSerializer()
     .WithPlugin(myPlugin)
     .Build<MyType>();
```

## List

The list interfaces provide access to an [Aerospike List](https://www.aerospike.com/docs/guide/cdt-list.html).

To access a single list, build an `IList<T>` and provide the record key where the list will be stored:

```C#
 var list = ListBuilder.Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build<TestType>("list_record_key"); // A bin name can also be specified here
```

To operate on multiple lists that contain the same data type, build an `IListOperator<T>` by omitting the key when building, i.e:

```C#
var listOperator = ListBuilder.Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializer()
    .Build<TestType>();
```

### ReadAll()

Both available interfaces accept a generic type during configuration (and therefore only operate on one type of list) in order to avoid mixing data types in a single list. To illustrate the problem, consider the `ReadAll()` method that returns all items in a list. Without knowing the size of the list, it must deserialize every item into some data type. If an item of a different type is accidentally added to the list, it will be impossible to deserialize without handling the different type specially. The interfaces are designed to (hopefully) prevent this from happening.

If a complex data structure involving lists with multiple types is needed, use the [Operator](#operator) data access object.

## Operator

The `IOperator` interface allows you to build and execute multi-operation transactions on a single record. Generally, this wraps the [`client.Operate`](https://www.aerospike.com/docs/client/csharp/usage/kvs/multiops.html) functionality provided by Aerospike.

`IOperator` currently only supports reading one result from a arbitrarily long list of operations. i.e., you may perform multiple operations at once (e.g. remove an item from a list in one bin, add an item to a list in another bin), but only retrieve one piece of data back (e.g. get the size of the second list).

For example:

```C#
// Configure an operator
_operator = OperatorBuilder
    .Configure(clientProvider)
    .WithDataContext(new DataContext("my_namespace", "my_set"))
    .UseMessagePackSerializerWithLz4Compression()
    .Build();
    
var list2Size = await _operator.Key(RecordKey) // The record you're operating on
    .List.RemoveByValue("list_1_bin", listItem)
    .List.Append("list_2_bin", listItem)
    .SizeAsync("list_2_bin", cancellationToken); // Return the result of the "size" operation
```

### Batch Operations

_This feature is not yet implemented._

Aerospike does not support batching update or write operations on multiple records in the same request. It does, however, support [fetching batches of records](https://docs.aerospike.com/docs/guide/batch.html#known-limitations). The [KeyValueStore](#keyvaluestore) data access object fetches records in batches when reading multiple keys of the same type.

## Set Scans

Using the `ISetScanner` interface, you're able to scan all records in a specified namespace and set. This means you can iterate through every record, its key (see Caveat below) and value(s).
For example (flexible interface):

```C#
// Use the 'flexible' scanner interface.
var scanner = SetScannerBuilder.Configure(myClientProvider)
    .WithDataContext(myDataContext)
    .WithSerializer(mySerializer)
    .WithScanConfiguration(myConfig)
    .Build();

// Write each fetched key to the console.
scanner.ScanSet(key => Console.WriteLine(key));
```

or using the strongly typed interface:

```C#
// Use the 'strongly-typed' scanner interface.
var scanner = SetScannerBuilder.Configure(myClientProvider)
    .WithDataContext(myDataContext)
    .WithSerializer(mySerializer)
    .WithScanConfiguration(myConfig)
    .Build<MyCoolClass>("my_cool_bin_name");

// Write each fetched key to the console.
scanner.ScanSet((key, record) => Console.WriteLine(key));
```

### Caveat

Key retrieval requires that `sendKey` was set to `true` when the record was _written to Aerospike_. Otherwise, the string key isn't stored in Aerospike and all we'll have access to is the digest.

## General

The general interfaces include the SetTruncator and the KeyOperator. As the name implies, the SetTruncator is for removing all records in a set.
The KeyOperator is for interacting with records by their key. As stated above, a [client provider](#client-provider) must be provided to the KeyOperatorBuilder
or the SetTruncatorBuilder.

### SetTruncator

⚠️ **Use With Caution!** ⚠️
The SetTruncator is for quickly truncating all records contained in a namespace/set. It gets the namespace and set from the DataContext
passed into the SetTruncatorBuilder. Removing records using the SetTruncator is many orders of magnitude faster than deleting records one at a time.
You can either remove all records in a set using the TruncateSet method with no parameters, or a DateTime can be specified for removing
records before the last update time.

### KeyOperator

The KeyOperator provides a way for interacting by key with records that are already in Aerospike. The KeyOperator exposes methods to reset
a record's time to live, delete a record, or check if a record exists. Similar to the [set truncator](#SetTruncator), a KeyOperator is created
using the KeyOperatorBuilder, and a [client provider](#client-provider) and DataContext must be provided.
