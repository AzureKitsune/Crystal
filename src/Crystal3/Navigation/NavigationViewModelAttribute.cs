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
        public NavigationViewModelAttribute(Type viewModel, NavigationViewSupportedPlatform platform = NavigationViewSupportedPlatform.All)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            ViewModel = viewModel;
            SupportedPlatforms = platform;
        }
        public Type ViewModel { get; private set; }
        public NavigationViewSupportedPlatform SupportedPlatforms { get; private set; }
        public bool Singleton { get; set; }
        public bool IsHome { get; set; }
    }
}
