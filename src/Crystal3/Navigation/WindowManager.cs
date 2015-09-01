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

        internal static IEnumerable<WindowService> GetAllWindowServices()
        {
            return WindowNavigationServices.AsReadOnly();
        }

        internal static ViewModelBase GetRootViewModelForCurrentWindow()
        {
            var navManager = GetNavigationManagerForCurrentWindow();
            var frame = navManager.RootNavigationService.NavigationFrame;
            var page = frame.Content as Page;
            var viewModel = page.DataContext as ViewModelBase;

            return viewModel;
        }

        public static async Task<WindowService> CreateNewWindowAsync<T>(object parameter = null) where T : ViewModelBase
        {
            //https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/MultipleViews/cs/Scenario1.xaml.cs#L71

            var view = CoreApplication.CreateNewView();
            //and here, the above HandleNewWindow method should be called.

            await view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                var frame = new Frame();

                var navManager = new NavigationManager((CrystalApplication)CrystalApplication.Current);
                navManager.RootNavigationService = new NavigationService(frame, navManager);

                Window.Current.Content = frame;

                WindowManager.HandleNewWindow(Window.Current, navManager);

                navManager.RootNavigationService.NavigateTo<T>(parameter);
            }));


            var bundle = WindowNavigationServices.First(x =>
                x.WindowView.Dispatcher == view.Dispatcher);

            return bundle;
        }
    }
}
