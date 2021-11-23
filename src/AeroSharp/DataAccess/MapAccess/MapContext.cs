using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.MapAccess
{
    [ExcludeFromCodeCoverage]
    public class MapContext
    {
        private const string DefaultBin = "data";

        public MapContext() : this(DefaultBin) { }

        public MapContext(string bin)
        {
            Bin = bin;
        }

        public string Bin { get; }
    }
}
