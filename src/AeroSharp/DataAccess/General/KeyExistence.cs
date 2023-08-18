using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.General
{
    [ExcludeFromCodeCoverage]
    public class KeyExistence
    {
        public KeyExistence(string key, bool exists)
        {
            Key = key;
            Exists = exists;
        }

        public string Key { get; }

        public bool Exists { get; }
    }
}
