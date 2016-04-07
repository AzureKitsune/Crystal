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
        public static IoCContainer Current { get; internal set; }

        private static Dictionary<Type, IoCContainer> viewModelContainerList = new Dictionary<Type, IoCContainer>();

        public static IoCContainer GetContainerForViewModel<T>() where T : ViewModelBase
        {
            if (!viewModelContainerList.ContainsKey(typeof(T)))
            {
                var cont = new IoCContainer();
                viewModelContainerList.Add(typeof(T), cont);
                return cont;
            }
            else
                return viewModelContainerList[typeof(T)] as IoCContainer;
        }
    }
}
