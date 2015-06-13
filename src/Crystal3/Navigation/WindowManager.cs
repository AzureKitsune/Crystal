using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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
            return GetNavigationManagerForCurrentWindow().RootNavigationService.NavigationFrame.DataContext as ViewModelBase;
        }
    }
}
