using Crystal3.Core;
using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
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

            StatusBarForegroundColor = Colors.White;
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
            if (currentPlatform == Core.Platform.Mobile)
            {
                //todo fix rare bug where this is called on launch and dispatcher is null
                CrystalApplication.Dispatcher.RunAsync(async () =>
                {
                    if (CanAccessMobileStatusBar())
                    {
                        if (mobileStatusBar != null)
                        {
                            if (AnimateStatusBarText)
                                await mobileStatusBar.ProgressIndicator.HideAsync();
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

                            mobileStatusBar.ForegroundColor = StatusBarForegroundColor;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// The text to show in the status bar when there aren't any controllers active. Think of it as the titlebar in a mobile application.
        /// </summary>
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

                if (PropertyChanged != null) //makes this property binding-friendly.
                    PropertyChanged(this, new PropertyChangedEventArgs("NormalStatusText"));
            }
        }

        public bool IsBusy
        {
            get { return isBusyValue; }
            private set
            {
                isBusyValue = value;

                if (PropertyChanged != null) //makes this property binding-friendly.
                    PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        public bool AnimateStatusBarText { get; set; }

        public Color? StatusBarForegroundColor { get; set; }

        private void RefreshStatus()
        {
            //as long as there are active controllers, the status manager should say that it is busy.
            IsBusy = controllers.Count > 0;
        }

        internal void RemoveAllControllersForCallingViewModel(ViewModelBase callingViewModel)
        {
            lock (controllers)
            {
                foreach (var control in controllers.Where(x => object.ReferenceEquals(x.ParentViewModel, callingViewModel)).ToArray())
                    control.Dispose();

                RefreshStatus();
            }
        }

        public IndefiniteWorkStatusManagerControl DoIndefiniteWork(ViewModelBase callingViewModel, string statusText)
        {
            var control = new IndefiniteWorkStatusManagerControl(this, statusText, callingViewModel);
            RefreshStatus();
            return control;
        }

        public DefiniteWorkStatusManagerControl DoWork(ViewModelBase callingViewModel, string statusText)
        {
            var control = new DefiniteWorkStatusManagerControl(this, statusText, callingViewModel);
            RefreshStatus();
            return control;
        }

        public abstract class StatusManagerControl : IDisposable
        {
            internal StatusManagerControl(StatusManager manager, ViewModelBase callingViewModel)
            {
                ParentStatusManager = manager;
                ParentViewModel = callingViewModel;

                if (!ParentStatusManager.controllers.Contains(this))
                    ParentStatusManager.controllers.Add(this);
                else
                    throw new Exception("StatusManagerControl is already present in the ParentStatusManager's controller list.");
            }

            public StatusManager ParentStatusManager { get; private set; }

            internal ViewModelBase ParentViewModel { get; private set; }

            public abstract void Dispose();
        }

        public class IndefiniteWorkStatusManagerControl : StatusManagerControl
        {
            internal IndefiniteWorkStatusManagerControl(StatusManager manager, string statusText, ViewModelBase callingViewModel) : base(manager, callingViewModel)
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
            internal DefiniteWorkStatusManagerControl(StatusManager manager, string statusText, ViewModelBase callingViewModel) : base(manager, statusText, callingViewModel)
            {
                if (manager.currentPlatform == Core.Platform.Mobile)
                {
                    manager.UpdateStatusText(statusText);
                    manager.UpdateProgress(0);
                }
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
