using Crystal3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Crystal3.UI.StatusManager
{
    public class StatusManager
    {
        private static Crystal3.Core.Platform currentPlatform = Crystal3.Core.Platform.Unknown;
        private static string normalStatusText = null;
        private static StatusBar mobileStatusBar = null;
        private static Window boundWindow = null;

        internal StatusManager(Window window)
        {
            currentPlatform = CrystalApplication.GetDevicePlatform();

            boundWindow = window;
        }

        internal async void Initialize()
        {
            if (currentPlatform == Core.Platform.Mobile && CanAccessMobileStatusBar())
            {
                mobileStatusBar = StatusBar.GetForCurrentView();
                mobileStatusBar.ProgressIndicator.ProgressValue = 0;
                await mobileStatusBar.ShowAsync();
            }

            NormalStatusText = ApplicationView.GetForCurrentView().Title;
        }

        internal bool CanAccessMobileStatusBar()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        private void UpdateStatusText(string status)
        {
            CrystalApplication.Dispatcher.RunAsync(() =>
            {
                if (currentPlatform == Core.Platform.Mobile)
                {
                    if (CanAccessMobileStatusBar())
                    {
                        if (mobileStatusBar != null)
                        {
                            mobileStatusBar.ProgressIndicator.Text = status;
                            mobileStatusBar.ProgressIndicator.ShowAsync();
                        }
                    }
                }
            });
        }
        private void UpdateNormalStatus()
        {
            UpdateStatusText(NormalStatusText);
        }

        private void UpdateProgress(double? status)
        {
            if (currentPlatform == Core.Platform.Mobile)
            {
                if (CanAccessMobileStatusBar())
                {
                    if (mobileStatusBar != null)
                    {
                        mobileStatusBar.ProgressIndicator.ProgressValue = status;
                    }
                }
            }
        }

        public string NormalStatusText
        {
            get
            {
                return normalStatusText;
            }
            set
            {
                normalStatusText = value;

                UpdateNormalStatus();
            }
        }

        public Task<T> DoIndefiniteWorkAsync<T>(string statusText, Task<T> workCallback)
        {
            UpdateStatusText(statusText);
            UpdateProgress(null);

            return workCallback.ContinueWith<T>(x =>
            {
                CrystalApplication.Dispatcher.RunAsync(() =>
                {
                    UpdateNormalStatus();
                    UpdateProgress(0);
                });

                return x.Result;
            });
        }

        public async Task<T> DoWorkAsync<T>(string statusText, IAsyncOperationWithProgress<T, double?> workCallback)
        {
            workCallback.Progress = new AsyncOperationProgressHandler<T, double?>((sender, progressValue) =>
            {
                UpdateStatusText(statusText);
                UpdateProgress(progressValue);
            });

            workCallback.Completed = new AsyncOperationWithProgressCompletedHandler<T, double?>((sender, status) =>
           {
               UpdateProgress(null);
               UpdateNormalStatus();
           });

            return await workCallback;
        }
    }
}
