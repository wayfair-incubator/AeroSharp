using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class ListConfiguration
    {
        public ListConfiguration()
        {
            Ordering = false;
            Partial = false;
            NoFail = false;
            AddUniqueOnly = false;
            InsertBounded = false;
        }

        public bool Ordering { get; set; }

        public bool Partial { get; set; }

        public bool NoFail { get; set; }

        public bool AddUniqueOnly { get; set; }

        public bool InsertBounded { get; set; }
    }
}