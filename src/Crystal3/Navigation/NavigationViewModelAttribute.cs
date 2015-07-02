using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Navigation
{

    /// <summary>
    /// Links a page to a view model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NavigationViewModelAttribute : Attribute
    {
        public NavigationViewModelAttribute(Type viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            ViewModel = viewModel;
        }
        public Type ViewModel { get; set; }
        public bool Singleton { get; set; }
        public bool IsHome { get; set; }
    }
}
