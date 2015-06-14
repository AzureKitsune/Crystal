using Crystal3.Model;
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
        static Dictionary<Window, NavigationManager> WindowNavigationManagers = new Dictionary<Window, NavigationManager>();

        internal static void HandleNewWindow(Window window, NavigationManager manager)
        {
            if (WindowNavigationManagers.ContainsKey(window) || WindowNavigationManagers.ContainsValue(manager))
                throw new Exception();

            //handle any future initialization

            WindowNavigationManagers.Add(window, manager);
        }

        public static NavigationManager GetNavigationManagerForCurrentWindow()
        {
            return WindowNavigationManagers[Window.Current];
        }

        internal static IEnumerable<Window> GetAllWindows()
        {
            return WindowNavigationManagers.Keys;
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
