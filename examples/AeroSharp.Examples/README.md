# AeroSharp Examples

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Execution

To run examples for the project locally, first start up Aerospike in a local Docker container by running:

```text
../scripts/start_aerospike_in_docker.sh
```

After doing this, the examples will connect to your local Aerospike instance and everything should "just work"!

Examples can be run from the project root with `dotnet run --framework net7.0 --project examples/AeroSharp.Examples/AeroSharp.Examples.csproj` or via your .NET IDE of choice.
