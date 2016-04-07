using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.InversionOfControl
{
    public static class IoC
    {
        /// <summary>
        /// Returns the application-level IoC container.
        /// </summary>
        public static IoCContainer Current { get; internal set; }

        private static Dictionary<Type, IoCContainer> viewModelContainerList = new Dictionary<Type, IoCContainer>();

        public static IoCContainer GetContainerForViewModel<T>() where T : ViewModelBase
        {
            return GetContainerForViewModel(typeof(T));
        }

        public static IoCContainer GetContainerForViewModel(Type type)
        {
            if (!viewModelContainerList.ContainsKey(type))
            {
                var cont = new IoCContainer();
                viewModelContainerList.Add(type, cont);
                return cont;
            }
            else
                return viewModelContainerList[type] as IoCContainer;
        }
    }
}
