using Crystal2.Model;
using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Crystal2.Navigation
{
    /// <summary>
    /// A static class responsible for navigating to and from other views and their viewmodels.
    /// </summary>
    public static class NavigationManager
    {
        private static ReadOnlyDictionary<Type, object> navigablePages = null;
        static NavigationManager()
        {
            var navigationProvider = IoCManager.Resolve<INavigationProvider>();
            navigationProvider.Navigating += navigationProvider_Navigating;
            navigationProvider.Navigated += navigationProvider_Navigated;

            //navigablePages = IoCManager.Resolve<INavigationDirectoryProvider>().ProvideMap();
        }

        static void navigationProvider_Navigated(object sender, CrystalNavigationEventArgs e)
        {

        }

        static void navigationProvider_Navigating(object sender, CrystalNavigationEventArgs e)
        {

        }

        /// <summary>
        /// Navigates to the viewmodel in the type parameter with an optional parameter as an argument.
        /// </summary>
        /// <typeparam name="T">The type of the view model.</typeparam>
        /// <param name="parameter">The parameter for the navigation.</param>
        public static void NavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            //grabs the directory provider from the IoC container.
            var directoryProvider = IoCManager.Resolve<INavigationDirectoryProvider>();

            //constructs a navigation information object for navigation.
            var navigationInformation = new NavigationInformation();
            navigationInformation.TargetViewModel = (ViewModelBase)Activator.CreateInstance(typeof(T));
            navigationInformation.Parameter = parameter;

            //grabs the navigation provider from the IoC container and navigates using it.
            IoCManager.Resolve<INavigationProvider>().Navigate(navigationInformation, directoryProvider);
        }

        public static ViewModelBase CurrentViewModel
        {
            get { return IoCManager.Resolve<INavigationProvider>().GetCurrentViewModel(); }
        }

        public static bool CanGoBack { get { return IoCManager.Resolve<INavigationProvider>().CanGoBackward; } }

        public static void GoBackward()
        {
            IoCManager.Resolve<INavigationProvider>().GoBackward();
        }

        public static void ClearBackStack()
        {
            IoCManager.Resolve<INavigationProvider>().ClearBackStack();
        }
    }
}
