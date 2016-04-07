using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.InversionOfControl
{
    /// <summary>
    /// A simple Inverse of Control container.
    /// </summary>
    public class IoCContainer
    {
        private List<KeyValuePair<Type, IIoCObject>> itemsList = null;

        internal IoCContainer()
        {
            itemsList = new List<KeyValuePair<Type, IIoCObject>>();
        }

        /// <summary>
        /// Registers an object that can be resolved later.
        /// </summary>
        /// <typeparam name="T">The type of object (an Interface that implements <see cref="IIoCObject">IIoCObject</see>).)</typeparam>
        /// <param name="objectToRegister">The actual object to be registered.</param>
        public void Register<T>(T objectToRegister) where T : IIoCObject
        {
            //Makes sure the type parameter is an IIoCObject.
            if (typeof(T) == typeof(IIoCObject))
                throw new ArgumentException("Generic argument cannot be IIoCObject.");

            if (!(objectToRegister is T))
                throw new Exception("Object and type do not match!");

            itemsList.Add(new KeyValuePair<Type, IIoCObject>(typeof(T), objectToRegister));
        }

        public void Unregister<T>(T objectToUnregister) where T : IIoCObject
        {
            var item = itemsList.Where(x => x.Value is T).FirstOrDefault(x => object.ReferenceEquals((T)x.Value, objectToUnregister));
            try
            {
                itemsList.Remove(item); //can't null check since switch to List<KeyValuePair<... . todo
            }
            catch (Exception)
            {
                throw new Exception("Object not found.");
            }
        }

        /// <summary>
        /// Resolves an object based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type parameter to resolve against.</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : IIoCObject
        {
            var obj = (T)itemsList.FirstOrDefault(x => x.Key == typeof(T)).Value;

            if (obj == null) throw new Exception("Types implementing " + typeof(T).Name + " were not found.");

            return obj;
        }
        public T ResolveDefault<T>(Func<T> defaultObjectCreator) where T : IIoCObject
        {
            var obj = (T)itemsList.FirstOrDefault(x => x.Key == typeof(T)).Value;

            if (obj == null) return defaultObjectCreator();

            return obj;
        }

        public IEnumerable<T> ResolveAll<T>() where T : IIoCObject
        {
            var items = itemsList.Where(x =>
                    x.Key == typeof(T))
                .Select(x =>
                      x.Value);
            return (IEnumerable<T>)items.ToArray().Select(x => (T)x);
        }

        public bool IsRegistered<T>() where T : IIoCObject
        {
            return itemsList.Any(x => x.Key == typeof(T));
        }
    }
}
