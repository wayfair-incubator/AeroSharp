# OrderedMap Example Usage

This document walks through how to use the new `IOrderedMap<TSubKey, TOrderKey, TValue>` /
`IOrderedMapOperator<TSubKey, TOrderKey, TValue>` abstraction that wraps a server-side,
key-ordered set of values in Aerospike.

## What it is

An `OrderedMap` stores a collection of `(SubKey, OrderKey, Value)` entries under a single
Aerospike record, natively sorted by `OrderKey`. It is backed by two bins on that record:

| Bin | Aerospike map type | Keyed by | Purpose |
|---|---|---|---|
| **Data bin** | `KEY_ORDERED` map | Composite `[OrderKey, SubKey]` | Holds the actual values, sorted by `OrderKey` (subkey is a tiebreaker) |
| **Index bin** | `UNORDERED` map | `SubKey` | Maps a subkey to its current `OrderKey`, so an entry can be found/relocated in O(1) without scanning the data bin |

`UpsertAsync`/`RemoveAsync` are implemented as an optimistic read-modify-write cycle:
read the index bin to find the entry's current position, then write both bins together
guarded by `GenerationPolicy.EXPECT_GEN_EQUAL`. If another writer wins the race, the
generation check fails and the operation is retried per `OrderedMapConfiguration.ReadModifyWritePolicy`.

## Basic setup

```csharp
using AeroSharp.Connection;
using AeroSharp.DataAccess;

var client = AerospikeClientProvider.GetClient();
var dataContext = new DataContext("test", "leaderboard");

// Subkey = player id (string), OrderKey = score (long), Value = player name (string)
var leaderboard = OrderedMapBuilder
    .Configure(client)
    .WithDataContext(dataContext)
    .Build<string, long, string>("global_leaderboard");
```

`Build<TSubKey, TOrderKey, TValue>(key)` uses the default bin names (`ordered_data` /
`ordered_index`). Use the `(key, dataBin, indexBin)` overload if you need custom bin
names, e.g. to store more than one ordered map on the same record.

## Inserting and re-sorting entries

```csharp
await leaderboard.UpsertAsync("player_1", 1200, "Alice", CancellationToken.None);
await leaderboard.UpsertAsync("player_2", 950, "Bob", CancellationToken.None);
await leaderboard.UpsertAsync("player_3", 1875, "Carol", CancellationToken.None);

// Alice's score changes - this relocates her entry to stay sorted by score.
await leaderboard.UpsertAsync("player_1", 2100, "Alice", CancellationToken.None);
```

`UpsertAsync` is idempotent per subkey: calling it again with the same subkey but a
different order key removes the old composite-key entry and re-inserts at the new
position; calling it with the same order key just updates the value in place.

## Reading values

```csharp
// All values, ascending by order key.
IEnumerable<string> ranked = await leaderboard.GetAllAsync(CancellationToken.None);

// Top scorer (highest order key) via a negative index, mirroring Aerospike's index semantics.
string topPlayer = await leaderboard.GetByIndexAsync(-1, CancellationToken.None);

// Lowest scorer.
string lowestPlayer = await leaderboard.GetByIndexAsync(0, CancellationToken.None);

long playerCount = await leaderboard.SizeAsync(CancellationToken.None);
```

## Removing entries

```csharp
await leaderboard.RemoveAsync("player_2", CancellationToken.None);

// Removing a subkey that was never inserted throws MapEntryNotFoundException.
```

## Clearing the map

```csharp
// Deletes the whole underlying record (both bins).
await leaderboard.ClearAsync(CancellationToken.None);
```

## Storing complex values

Use a serializer for non-primitive values, same as `MapBuilder`/`ListBuilder`:

```csharp
public sealed class PlayerStats
{
    public int Wins { get; init; }
    public int Losses { get; init; }
}

var statsBoard = OrderedMapBuilder
    .Configure(client)
    .WithDataContext(dataContext)
    .UseMessagePackSerializer() // or UseProtobufSerializer() / UseMessagePackSerializerWithLz4Compression()
    .Build<string, long, PlayerStats>("player_stats_by_wins");

await statsBoard.UpsertAsync("player_1", 42, new PlayerStats { Wins = 42, Losses = 3 }, CancellationToken.None);
```

Sub keys and order keys must be one of `long`, `double`, `string`, `byte[]`, or `bool`
(the primitive Aerospike CDT map key types) — only the *value* can be an arbitrary
serializable type.

## Tuning retry behavior under write contention

Because every subkey under the same record key shares one Aerospike generation
counter, concurrent `UpsertAsync`/`RemoveAsync` calls to *different* subkeys on the
same record still contend with each other. If you expect many concurrent writers per
record, widen the retry policy:

```csharp
var leaderboard = OrderedMapBuilder
    .Configure(client)
    .WithDataContext(dataContext)
    .WithOrderedMapConfiguration(new OrderedMapConfiguration
    {
        ReadModifyWritePolicy = new ReadModifyWritePolicy
        {
            MaxRetries = 20,
            WaitTimeInMilliseconds = 5,
            WithExponentialBackoff = true
        }
    })
    .Build<string, long, string>("global_leaderboard");
```

If your workload has many independent writers, consider spreading entries across
multiple record keys (e.g. sharding the leaderboard) instead of relying solely on
retries, since throughput on a single record is ultimately bounded by how fast
generation conflicts can be retried.

## Building an operator for multiple ordered maps of the same shape

If you need to work with many ordered maps that share the same `TSubKey`/`TOrderKey`/`TValue`
types but live under different record keys, build an `IOrderedMapOperator` instead of a
single `IOrderedMap`, and pass the record key explicitly on each call:

```csharp
var leaderboardOperator = OrderedMapBuilder
    .Configure(client)
    .WithDataContext(dataContext)
    .Build<string, long, string>();

await leaderboardOperator.UpsertAsync("season_2026", "ordered_data", "ordered_index", "player_1", 2100, "Alice", CancellationToken.None);
await leaderboardOperator.UpsertAsync("season_2025", "ordered_data", "ordered_index", "player_1", 1800, "Alice", CancellationToken.None);
```
