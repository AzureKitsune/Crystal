using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.IOC
{
    /// <summary>
    /// A simple Inverse of Control container.
    /// </summary>
    public static class IoCManager
    {
        private static Dictionary<Type, IIoCObject> itemsDictionary = null;

        static IoCManager()
        {
            itemsDictionary = new Dictionary<Type, IIoCObject>();
        }

        /// <summary>
        /// Registers an object that can be resolved later.
        /// </summary>
        /// <typeparam name="T">The type of object (an Interface that implements <see cref="IIoCObject">IIoCObject</see>).)</typeparam>
        /// <param name="objectToRegister">The actual object to be registered.</param>
        public static void Register<T>(T objectToRegister) where T : IIoCObject
        {
            //Makes sure the type parameter is an IIoCObject.
            if (typeof(T) == typeof(IIoCObject))
                throw new ArgumentException("Generic argument cannot be IIoCObject.");

            if (!(objectToRegister is T))
                throw new Exception("Object and type do not match!");

            itemsDictionary.Add(typeof(T), objectToRegister);
        }
        /// <summary>
        /// Resolves an object based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type parameter to resolve against.</typeparam>
        /// <returns></returns>
        public static T Resolve<T>() where T : IIoCObject
        {
            var obj = (T)itemsDictionary.FirstOrDefault(x => x.Key == typeof(T)).Value;

            if (obj == null) throw new Exception("Types implementing " + typeof(T).Name + " were not found.");

            return obj;
        }
        public static T ResolveDefault<T>(Func<T> defaultObjectCreator) where T : IIoCObject
        {
            var obj = (T)itemsDictionary.FirstOrDefault(x => x.Key == typeof(T)).Value;

            if (obj == null) return defaultObjectCreator();

            return obj;
        }

        public static IEnumerable<T> ResolveAll<T>() where T : IIoCObject
        {
            return (IEnumerable<T>)itemsDictionary.Where(x => x.Key == typeof(T)).Select(x => x.Value);
        }

        public static bool IsRegistered<T>() where T : IIoCObject
        {
            return itemsDictionary.Any(x => x.Key == typeof(T));
        }
    }
}
