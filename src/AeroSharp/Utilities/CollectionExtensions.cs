using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.Utilities
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> BatchNoMultipleEnumeration<T>(this IEnumerable<T> input, Func<IList<T>, IEnumerable<T>> function, int batchSize = 1000)
        {
            var batch = new List<T>(batchSize);
            foreach (var item in input)
            {
                batch.Add(item);

                if (batch.Count >= batchSize)
                {
                    foreach (var output in function(batch))
                    {
                        yield return output;
                    }
                    batch.Clear();
                }
            }

            // any trailing items
            foreach (var output in function(batch))
            {
                yield return output;
            }
        }

        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            if (batchSize <= 0) { throw new ArgumentOutOfRangeException("Batch size cannot be zero unless you like endless CPU cycles!"); }
            if (!collection.Any()) yield break;

            int count = 0;
            do
            {
                var skip = count * batchSize;
                count++;

                yield return collection.Skip(skip).Take(batchSize).ToArray();
            }
            while (count * batchSize < collection.Count());
        }
    }
}
