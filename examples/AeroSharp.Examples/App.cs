using System;
using System.Threading.Tasks;
using AeroSharp.Examples.Keys;
using AeroSharp.Examples.KeyValue;
using AeroSharp.Examples.Lists;
using AeroSharp.Examples.Operations;
using AeroSharp.Examples.Ttl;
using AeroSharp.Examples.Utilities;

namespace AeroSharp.Examples
{
    public class App
    {
        public async Task Run()
        {
            var client = AerospikeClientProvider.GetClient();

            var examples = new IExample[]
            {
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