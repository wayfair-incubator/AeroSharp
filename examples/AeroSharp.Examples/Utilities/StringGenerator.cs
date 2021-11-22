using System;
using System.Linq;

namespace AeroSharp.Examples.Utilities
{
    internal static class StringGenerator
    {
        private static readonly Random Random = new();

        internal static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}