using AeroSharp.Examples.Keys;
using AeroSharp.Examples.KeyValue;
using AeroSharp.Examples.Lists;
using AeroSharp.Examples.Maps;
using AeroSharp.Examples.Operations;
using AeroSharp.Examples.Ttl;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading.Tasks;

namespace AeroSharp.Examples
{
    public class App
    {
        public async Task RunAsync()
        {
            var client = AerospikeClientProvider.GetClient();

            var examples = new IExample[]
            {
                new MapExamples(client),
                new ListExamples(client),
                new KeyValueExamples(client),
                new KeyExample(client),
                new TtlExample(client),
                new OperationExamples(client)
            };

            Console.WriteLine("---------------------------------");
            Console.WriteLine("|                               |");
            Console.WriteLine("|      Aerospike Examples       |");
            Console.WriteLine("|                               |");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");

            foreach (var example in examples)
            {
                await example.ExecuteAsync();
            }

            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }
    }
}
