using Crystal3.Model;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Navigation
{
    /// <summary>
    /// The second highest class in a CrystalApplication. Corresponds to one Window in the application.
    /// </summary>
    public static class WindowManager
    {
        static List<WindowService> WindowNavigationServices = new List<WindowService>();

        internal static void HandleNewWindow(Window window, NavigationManager manager)
        {
            if (WindowNavigationServices.Any(x => x.WindowView == window || x.NavigationManager == manager))
                throw new Exception();

            //handle any further initialization
            var service = new WindowService(window, manager, new UI.StatusManager.StatusManager(window));

            WindowNavigationServices.Add(service);
        }

        public static WindowService GetWindowServiceForCurrentWindow()
        {
            if (Window.Current == null & CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess) throw new Exception("Cannot perform this operation unless we're on the UI thread.");

            return WindowNavigationServices.First(x => x.WindowView == Window.Current);
        }

        public static NavigationManager GetNavigationManagerForCurrentWindow()
        {
            return GetWindowServiceForCurrentWindow()?.NavigationManager;
        }

        public static StatusManager GetStatusManagerForCurrentWindow()
        {
            return GetWindowServiceForCurrentWindow()?.StatusManager;
        }

        internal static IEnumerable<Window> GetAllWindows()
        {
            return WindowNavigationServices.Select(x => x.WindowView);
        }

        public static IEnumerable<WindowService> GetAllWindowServices()
        {
            return WindowNavigationServices.AsReadOnly();
        }

        internal static ViewModelBase GetRootViewModelForCurrentWindow()
        {
            var navManager = GetNavigationManagerForCurrentWindow();
            return navManager.RootNavigationService.GetNavigatedViewModel();
        }

        /// <summary>
        /// Experimental!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task<WindowService> CreateNewWindowAsync<T>(object parameter = null) where T : ViewModelBase
        {
            //https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/MultipleViews/cs/Scenario1.xaml.cs#L71

            var view = CoreApplication.CreateNewView();
            //and here, the above HandleNewWindow method should be called.

            var bundle = WindowNavigationServices.First(x => x.WindowView.Dispatcher == view.Dispatcher);

            await bundle.WindowView.Dispatcher.RunIdleAsync(new IdleDispatchedHandler(x =>
            {
                bundle.NavigationManager.RootNavigationService.NavigateTo<T>(parameter);
            }));
            

            return bundle;
        }
    }
}
