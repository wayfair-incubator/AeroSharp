namespace AeroSharp.Examples
{
    /// <summary>
    /// Runs a suite of examples, with nicely formatted output for your viewing pleasure.
    ///
    /// Before running this, run `../scripts/start_aerospike_in_docker.sh` to start a preconfigured local Aerospike instance.
    /// </summary>
    public class Program
    {
        public static void Main()
        {
            new App().Run().GetAwaiter().GetResult();
        }
    }
}
