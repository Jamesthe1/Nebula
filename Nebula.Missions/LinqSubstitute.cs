using System;
using System.Collections.Generic;

namespace Nebula.Missions {
    using Object = UnityEngine.Object;

    /// <summary>
    /// Because there is no access to Linq, this must suffice
    /// </summary>
    internal static class LinqSubstitute {
        public static int Count<T> (this IEnumerable<T> values) {
            return new List<T> (values).Count;
        }

        /// <summary>
        /// "First" substitute
        /// </summary>
        /// <returns>The first item it can find, otherwise the default</returns>
        public static T FirstOf<T> (this IEnumerable<T> values, Func<T, bool> predicate) {
            foreach (T item in values)
                if (predicate (item))
                    return item;
            return default (T);
        }

        /// <summary>
        /// "Select" substitute
        /// </summary>
        /// <returns>The remapped list</returns>
        public static IEnumerable<U> Remap<T, U> (this IEnumerable<T> values, Func<T, U> remapper) {
            foreach (T item in values)
                yield return remapper (item);
        }

        /// <summary>
        /// "Zip" equivalent. Throws <see cref="ArgumentException"/> when keys and values are not the same length
        /// </summary>
        /// <returns>A <see cref="Dictionary{T, U}"/> made from the keys and values</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<T, U> Zipper<T, U> (this IEnumerable<T> keys, IEnumerable<U> values) {
            List<T> klist = new List<T> (keys);
            List<U> vlist = new List<U> (values);
            if (klist.Count != vlist.Count)
                throw new ArgumentException ("Keys and values are not the same length");

            Dictionary<T, U> dict = new Dictionary<T, U> ();
            for (int i = 0; i < klist.Count; i++)
                dict.Add (klist[i], vlist[i]);
            return dict;
        }

        /// <summary>
        /// Like "First", but for Unity object names
        /// </summary>
        /// <returns>The first item it can find, otherwise the default</returns>
        public static T FirstByName<T> (this IEnumerable<Object> values, string name) where T : Object {
            return values.FirstOf (o => o.name == name) as T;
        }
    }
}
