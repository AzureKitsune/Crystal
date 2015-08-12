using Crystal3.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class StatusManager : INotifyPropertyChanged
    {
        private Crystal3.Core.Platform currentPlatform = Crystal3.Core.Platform.Unknown;
        private string normalStatusText = null;
        private bool isBusyValue = false;
        private StatusBar mobileStatusBar = null;
        private Window boundWindow = null;
        private List<StatusManagerControl> controllers = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal StatusManager(Window window)
        {
            currentPlatform = CrystalApplication.GetDevicePlatform();

            boundWindow = window;

            controllers = new List<StatusManagerControl>();
        }

        internal async void Initialize()
        {
            if (currentPlatform == Core.Platform.Mobile && CanAccessMobileStatusBar())
            {
                mobileStatusBar = StatusBar.GetForCurrentView();
                mobileStatusBar.ProgressIndicator.ProgressValue = 0;
                await mobileStatusBar.ShowAsync();
            }

            try
            {
                NormalStatusText = ApplicationView.GetForCurrentView().Title;
            }
            catch (Exception) { }
        }

        internal bool CanAccessMobileStatusBar()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        private void UpdateStatusText(string status)
        {
            if (currentPlatform == Core.Platform.Mobile)
            {
                //todo fix rare bug where this is called on launch and dispatcher is null
                CrystalApplication.Dispatcher.RunAsync(async () =>
                {
                    if (CanAccessMobileStatusBar())
                    {
                        if (mobileStatusBar != null)
                        {
                            mobileStatusBar.ProgressIndicator.Text = status;
                            await mobileStatusBar.ProgressIndicator.ShowAsync();
                        }
                    }
                });
            }
        }
        private void UpdateNormalStatus()
        {
            UpdateStatusText(NormalStatusText);
        }

        private void UpdateProgress(double? status)
        {
            if (currentPlatform == Core.Platform.Mobile)
            {
                CrystalApplication.Dispatcher.RunAsync(() =>
                {
                    if (CanAccessMobileStatusBar())
                    {
                        if (mobileStatusBar != null)
                        {
                            mobileStatusBar.ProgressIndicator.ProgressValue = status;
                        }
                    }
                });
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

                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NormalStatusText"));
            }
        }

        public bool IsBusy
        {
            get { return isBusyValue; }
            private set
            {
                isBusyValue = value;

                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        private void RefreshStatus()
        {
            IsBusy = controllers.Count > 0;
        }

        public IndefiniteWorkStatusManagerControl DoIndefiniteWork(string statusText)
        {
            var control = new IndefiniteWorkStatusManagerControl(this, statusText);
            RefreshStatus();
            return control;
        }

        public DefiniteWorkStatusManagerControl DoWork(string statusText)
        {
            var control = new DefiniteWorkStatusManagerControl(this, statusText);
            RefreshStatus();
            return control;
        }

        public abstract class StatusManagerControl : IDisposable
        {
            internal StatusManagerControl(StatusManager manager)
            {
                ParentStatusManager = manager;

                ParentStatusManager.controllers.Add(this);
            }

            public StatusManager ParentStatusManager { get; private set; }

            public abstract void Dispose();
        }

        public class IndefiniteWorkStatusManagerControl : StatusManagerControl
        {
            internal IndefiniteWorkStatusManagerControl(StatusManager manager, string statusText) : base(manager)
            {
                if (manager.currentPlatform == Core.Platform.Mobile)
                {
                    manager.UpdateStatusText(statusText);
                    manager.UpdateProgress(null);
                }
            }

            public override void Dispose()
            {
                if (ParentStatusManager.currentPlatform == Core.Platform.Mobile)
                {
                    this.ParentStatusManager.UpdateNormalStatus();
                    this.ParentStatusManager.UpdateProgress(0);
                }

                ParentStatusManager.controllers.Remove(this);
                ParentStatusManager.RefreshStatus();
            }

            public void SetStatus(string text)
            {
                ParentStatusManager.UpdateStatusText(text);
            }


            public async Task SetStatusWithPauseAsync(string text, int showTimeSeconds)
            {
                ParentStatusManager.UpdateStatusText(text);

                await Task.Delay(showTimeSeconds * 1000);
            }

        }

        public class DefiniteWorkStatusManagerControl : IndefiniteWorkStatusManagerControl
        {
            internal DefiniteWorkStatusManagerControl(StatusManager manager, string statusText) : base(manager, statusText)
            {

            }

            public void SetProgress(double value)
            {
                ParentStatusManager.UpdateProgress(value);
            }

            public override void Dispose()
            {
                ParentStatusManager.controllers.Remove(this);
                ParentStatusManager.RefreshStatus();
            }
        }
    }
}
