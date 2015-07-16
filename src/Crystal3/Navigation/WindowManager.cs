using Crystal3.Model;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static ViewModelBase GetRootViewModel()
        {
            var navManager = GetNavigationManagerForCurrentWindow();
            var frame = navManager.RootNavigationService.NavigationFrame;
            var page = frame.Content as Page;
            var viewModel = page.DataContext as ViewModelBase;

            return viewModel;
        }
    }
}
