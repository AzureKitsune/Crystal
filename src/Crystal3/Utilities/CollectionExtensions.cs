using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Utilities
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var item in items)
                collection.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");

            foreach (var item in items)
                if (collection.Contains<T>(item))
                    collection.Remove(item);
        }
    }
}
