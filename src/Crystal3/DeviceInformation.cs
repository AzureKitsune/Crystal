using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Core;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;

namespace Crystal3
{
    public static class DeviceInformation
    {
        public static event EventHandler<DeviceInformationSubplatformChangedEventArgs> SubplatformChanged;

        private static Subplatform currentSubplatform = Subplatform.None;

        public static bool IsInTabletMode()
        {
            // https://social.msdn.microsoft.com/Forums/windowsapps/en-US/8781bc40-ae4c-43f2-846c-63f3859bc972/uwphow-to-detect-tablet-mode?forum=wpdevelop
            // https://msdn.microsoft.com/en-us/library/windows/apps/Windows.UI.ViewManagement.UIViewSettings.UserInteractionMode.aspx

            return Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode == Windows.UI.ViewManagement.UserInteractionMode.Touch;
        }

        public static bool IsCurrentViewInMixedReality()
        {
            if (IsMixedRealitySupported())
            {
                Windows.ApplicationModel.Preview.Holographic.HolographicApplicationPreview.IsCurrentViewPresentedOnHolographicDisplay();
            }

            return false; //not supported.
        }

        public static bool IsMixedRealitySupported()
        {
            return ApiInformation.IsTypePresent("Windows.ApplicationModel.Preview.Holographic.HolographicApplicationPreview");
        }

        internal static bool QueryForTabletMode()
        {
            Crystal3.Core.Subplatform lastSubplatform = currentSubplatform;

            if (IsInTabletMode() && GetDevicePlatform() == Core.Platform.Desktop && lastSubplatform != Subplatform.TabletMode)
            {
                //Tablet mode is specific to desktop sku at this point.
                currentSubplatform = Subplatform.TabletMode;
                RaiseSubplatformChangeEvent(lastSubplatform, currentSubplatform);
                return true;
            }

            return false;
        }

        public static Crystal3.Core.Platform GetDevicePlatform()
        {
            if ((CrystalApplication.Current as CrystalApplication).Options.OverridePlatformDetection)
                return (CrystalApplication.Current as CrystalApplication).Options.OverridePlatformValue;

            switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToLower())
            {
                case "windows.mobile":
                    return Crystal3.Core.Platform.Mobile;
                case "windows.desktop":
                    return Crystal3.Core.Platform.Desktop;
                case "windows.iot":
                    return Crystal3.Core.Platform.IoT;
                case "windows.xbox":
                    return Crystal3.Core.Platform.Xbox;
                case "windows.holographic": //HoloLens. Doesn't include mixed reality headsets
                    return Crystal3.Core.Platform.Holographic;
                case "windows.team": //Surface Hub
                    return Crystal3.Core.Platform.Team;
                default:
                    return Crystal3.Core.Platform.Unknown;
            }
        }

        internal static void RefreshSubplatform(IActivatedEventArgs args = null)
        {
            Crystal3.Core.Subplatform lastSubplatform = currentSubplatform;

            //First check for mixed reality.
            if (IsMixedRealitySupported())
            {
                //mixed reality and holographic supported.

                //https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.preview.holographic.holographicapplicationpreview.isholographicactivation
                //HoloLens will always return true for this.
                //On other platforms, determines if the app was activated in holographic/mixed reality.
                bool holographicActivation = Windows.ApplicationModel.Preview.Holographic.HolographicApplicationPreview.IsHolographicActivation(args);

                if (holographicActivation && lastSubplatform != Subplatform.MixedReality)
                {
                    currentSubplatform = Subplatform.MixedReality;
                    RaiseSubplatformChangeEvent(lastSubplatform, currentSubplatform);
                    return;
                }
            }

            //Check for tablet mode.
            if (QueryForTabletMode())
            {
                return;
            }

            //Resets the subplatform back to none if it gets here.
            currentSubplatform = Subplatform.None;
            RaiseSubplatformChangeEvent(lastSubplatform, currentSubplatform);
        }

        private static void RaiseSubplatformChangeEvent(Subplatform lastSubPlatform, Subplatform newSubPlatform)
        {
            if (lastSubPlatform == newSubPlatform) return;

            SubplatformChanged?.Invoke(null,
                new DeviceInformationSubplatformChangedEventArgs(
                    lastSubPlatform,
                    DeviceInformationSubplatformChangedEventArgs.DeviceInformationSubplatformChangedStatus.Deactivation));

            SubplatformChanged?.Invoke(null,
                new DeviceInformationSubplatformChangedEventArgs(
                    currentSubplatform,
                    DeviceInformationSubplatformChangedEventArgs.DeviceInformationSubplatformChangedStatus.Activation));
        }

        public static Crystal3.Core.Subplatform GetDeviceSubplatform()
        {
            return currentSubplatform;
        } 
    }
}
