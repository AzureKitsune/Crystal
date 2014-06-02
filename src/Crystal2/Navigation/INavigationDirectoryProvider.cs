using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    /// <summary>
    /// An interface that returns a dictionary of viewmodels and their views.
    /// </summary>
    public interface INavigationDirectoryProvider: IIoCObject
    {
        /// <summary>
        /// Returns a dictionary of ViewModel type and an object that corresponds to a view.
        /// </summary>
        /// <returns></returns>
        Dictionary<Type, object> ProvideMap();
    }
}
