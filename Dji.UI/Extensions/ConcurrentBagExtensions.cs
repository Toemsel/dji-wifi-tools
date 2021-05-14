using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dji.UI.Extensions
{
    public static class ConcurrentBagExtensions
    {
        public static void AddRange<T>(this ConcurrentBag<T> concurrentBag, IEnumerable<T> elements)
        {
            foreach (var currentElement in elements)
                concurrentBag.Add(currentElement);
        }
    }
}
