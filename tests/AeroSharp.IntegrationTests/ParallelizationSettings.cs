using NUnit.Framework;
// Integration tests share one Aerospike namespace/set; parallel workers cause cross-test data races.
[assembly: Parallelizable(ParallelScope.None)]