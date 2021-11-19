# AeroSharp Integration Tests

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Execution

To run integration tests for the project locally, first start up Aerospike in a local Docker container by running:

```
../scripts/start_aerospike_in_docker.sh
```

After doing this, the tests will connect to your local Aerospike instance and everything should "just work"!

Tests can be run from the project root with `dotnet test` or via your .NET IDE of choice.

## Adding New Tests

For convenience, there is a [TestPreparer](../AeroSharp.Tests/Utility/TestPreparer.cs) utility available for use, which provides a `PrepareTest()` method that:

1. Creates an `IClientProvider` that is preconfigured to run against the local Aerospike instance
2. Clears out any existing test data in within the `DataContext` that it provides
3. Returns the preconfigured `IClientProvider`

When implementing new integration tests, call this `TestPreparer.PrepareTest()` method within your test setup to start with a clean slate and have the configured `IClientProvider` for use in your new test.
