using Crystal3.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            //todo fix rare bug where this is called on launch and dispatcher is null
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

        public IndefiniteWorkStatusManagerControl DoIndefiniteWork(string statusText)
        {
            return new IndefiniteWorkStatusManagerControl(this, statusText);
        }

        public DefiniteWorkStatusManagerControl DoWork(string statusText)
        {
            return new DefiniteWorkStatusManagerControl(this, statusText);
        }

        public abstract class StatusManagerControl : IDisposable
        {
            internal StatusManagerControl(StatusManager manager)
            {
                ParentStatusManager = manager;
            }

            public StatusManager ParentStatusManager { get; private set; }

            public abstract void Dispose();
        }

        public class IndefiniteWorkStatusManagerControl : StatusManagerControl
        {
            internal IndefiniteWorkStatusManagerControl(StatusManager manager, string statusText) : base(manager)
            {
                if (currentPlatform == Core.Platform.Mobile)
                {
                    CrystalApplication.Dispatcher.RunAsync(IUIDispatcherPriority.Low, () =>
                    {
                        manager.UpdateStatusText(statusText);
                        manager.UpdateProgress(null);
                    });
                }
            }

            public override void Dispose()
            {
                if (currentPlatform == Core.Platform.Mobile)
                {
                    CrystalApplication.Dispatcher.RunAsync(IUIDispatcherPriority.Low, () =>
                    {
                        this.ParentStatusManager.UpdateNormalStatus();
                        this.ParentStatusManager.UpdateProgress(0);
                    });
                }
            }
        }

        public class DefiniteWorkStatusManagerControl : IndefiniteWorkStatusManagerControl
        {
            internal DefiniteWorkStatusManagerControl(StatusManager manager, string statusText) : base(manager, statusText)
            {

            }

            public void SetStatus(string text)
            {
                ParentStatusManager.UpdateStatusText(text);
            }

            public void SetProgress(double value)
            {
                ParentStatusManager.UpdateProgress(value);
            }
        }
    }
}
