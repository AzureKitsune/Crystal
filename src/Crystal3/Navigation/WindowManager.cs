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
        static List<WindowBundle> WindowNavigationManagers = new List<WindowBundle>();

        internal static void HandleNewWindow(Window window, NavigationManager manager)
        {
            if (WindowNavigationManagers.Any(x => x.WindowView == window || x.NavigationManager == manager))
                throw new Exception();

            //handle any further initialization
            var bundle = new WindowBundle();
            bundle.NavigationManager = manager;
            bundle.WindowView = window;
            bundle.StatusManager = new UI.StatusManager.StatusManager(window);

            WindowNavigationManagers.Add(bundle);
        }

        public static NavigationManager GetNavigationManagerForCurrentWindow()
        {
            return WindowNavigationManagers.First(x => x.WindowView == Window.Current)?.NavigationManager;
        }

        public static StatusManager GetStatusManagerForCurrentWindow()
        {
            return WindowNavigationManagers.First(x => x.WindowView == Window.Current)?.StatusManager;
        }

        internal static IEnumerable<Window> GetAllWindows()
        {
            return WindowNavigationManagers.Select(x => x.WindowView);
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
