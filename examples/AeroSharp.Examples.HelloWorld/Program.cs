using System;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;

namespace AeroSharp.Examples.HelloWorld
{
    /// <summary>
    /// The simplest example of connecting to a local instance of Aerospike and writing/reading some data.
    /// 
    /// Before running this, run `../scripts/start_aerospike_in_docker.sh` to start a preconfigured local Aerospike instance.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var clientProvider = ClientProviderBuilder
                .Configure()
                .WithBootstrapServers(new[] { "localhost" })
                .WithoutCredentials()
                .WithConfiguration(
                    new ConnectionConfiguration
                    {
                        ConnectionTimeout = TimeSpan.FromMilliseconds(100)
                    }
                )
                .Build();

            var dataContext = new DataContext("test", "test_set");

            var keyValueStore = KeyValueStoreBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<string>();

            await keyValueStore.WriteAsync("Hello", "World", default);

            var (key, value) = await keyValueStore.ReadAsync("Hello", default);

            Console.WriteLine($"{key} {value}!");
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
        }
    }
}
