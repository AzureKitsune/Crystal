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
        public NavigationViewModelAttribute(Type viewModel, NavigationViewSupportedPlatform platform = NavigationViewSupportedPlatform.All, bool useDataContextInsteadOfCreating = false)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            ViewModel = viewModel;
            SupportedPlatforms = platform;
            UseDataContextInsteadOfCreating = useDataContextInsteadOfCreating;
        }
        public Type ViewModel { get; private set; }
        public NavigationViewSupportedPlatform SupportedPlatforms { get; private set; }
        /// <summary>
        /// Interact with the ViewModel that was embedded in the view's XAML instead of creating a view model upon navigation.
        /// </summary>
        public bool UseDataContextInsteadOfCreating { get; private set; }
    }
}
