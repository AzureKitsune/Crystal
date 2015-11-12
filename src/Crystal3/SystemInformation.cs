using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Core;

namespace Crystal3
{
    public static class SystemInformation
    {
        public static bool IsInTabletMode()
        {
            // https://social.msdn.microsoft.com/Forums/windowsapps/en-US/8781bc40-ae4c-43f2-846c-63f3859bc972/uwphow-to-detect-tablet-mode?forum=wpdevelop
            // https://msdn.microsoft.com/en-us/library/windows/apps/Windows.UI.ViewManagement.UIViewSettings.UserInteractionMode.aspx

            return Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode == Windows.UI.ViewManagement.UserInteractionMode.Touch;
        }

        internal static Platform GetDevicePlatform()
        {
            switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToLower())
            {
                case "windows.mobile":
                    return Platform.Mobile;
                case "windows.desktop":
                    return Platform.Desktop;
                default:
                    return Platform.Unknown;
            }
        }
    }
}
