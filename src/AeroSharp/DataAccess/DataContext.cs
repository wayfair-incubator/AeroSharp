namespace AeroSharp.DataAccess
{
    public class DataContext
    {
        public string Namespace { get; }

        public string Set { get; }

        public DataContext(string @namespace, string set)
        {
            Namespace = @namespace;
            Set = set;
        }
    }
}