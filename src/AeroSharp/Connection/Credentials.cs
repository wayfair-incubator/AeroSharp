using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.Connection
{
    [ExcludeFromCodeCoverage]
    public class Credentials
    {
        public static readonly Credentials Empty = new Credentials(null, null);

        public string Username { get; }

        public string Password { get; }

        public Credentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
