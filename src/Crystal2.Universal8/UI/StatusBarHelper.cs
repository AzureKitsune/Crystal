using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI
{
    public static class StatusBarHelper
    {
        public static void ShowText(string message)
        {
            ShowStatus(message, 0);
        }
        public static void ShowStatus(string message)
        {
            ShowStatus(message, null);
        }
        public static async void ShowStatus(string message, double? progress)
        {
            var statusBar = IoCManager.ResolveDefault<IStatusBarProvider>(() => new DummyStatusBarProvider());
            statusBar.Message = message;
            statusBar.Progress = progress;
            await statusBar.ShowAsync();
        }

        public static async void HideStatus()
        {
            var statusBar = IoCManager.ResolveDefault<IStatusBarProvider>(() => new DummyStatusBarProvider());
            statusBar.Message = string.Empty;
            statusBar.Progress = null;
            await statusBar.HideAsync();
        }
    }
}
