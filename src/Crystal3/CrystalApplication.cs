using Crystal3.Core;
using Crystal3.InversionOfControl;
using Crystal3.Model;
using Crystal3.Navigation;
using Crystal3.UI.Dispatcher;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Crystal3
{
    /// <summary>
    /// The highest class in the application hierarchy. Your application class should inherit from this.
    /// </summary>
    public abstract class CrystalApplication : Application
    {
        internal static StorageFolder CrystalDataFolder = null;

        public CrystalConfiguration Options { get; private set; }

        public event EventHandler Restored;
        public event EventHandler<CrystalApplicationBackgroundActivationEventArgs> BackgroundActivated;
        private bool IsRestored { get; set; }


        public CrystalApplication() : base()
        {
            IoC.Current = new IoCContainer();

            Options = new CrystalConfiguration();
            OnConfigure();
            InitializeDataFolder();

            //base.InitializeComponent();

            this.Resuming += CrystalApplication_Resuming;
            this.Suspending += CrystalApplication_Suspending;
            this.EnteredBackground += CrystalApplication_EnteredBackground;
            this.LeavingBackground += CrystalApplication_LeavingBackground;
        }

        private async void InitializeDataFolder()
        {
            try
            {
                CrystalDataFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("CrystalDataFolder");
            }
            catch (Exception)
            {
                CrystalDataFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CrystalDataFolder");
            }
        }

        protected virtual void OnConfigure()
        {
            //Options.HandleSystemBackNavigation = true;
            //Options.NavigationRoutingMethod = NavigationRoutingMethod.Dynamic;
        }

        protected sealed override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            var firstWindow = WindowManager.GetAllWindows().FirstOrDefault();
            if (args.Window != firstWindow && firstWindow != null) //make sure it isn't our first window.
            {
                var frame = new Frame();

                var navManager = new NavigationManager(this);
                navManager.RootNavigationService = new NavigationService(frame, navManager);

                args.Window.Content = frame;

                WindowManager.HandleNewWindow(args.Window, navManager);
            }
        }

        private void InitializeIoC()
        {
            if (!IoC.Current.IsRegistered<IUIDispatcher>())
                IoC.Current.Register<IUIDispatcher>(new UIDispatcher(Window.Current.Dispatcher));
        }

        private void InitializeNavigation(NavigationManager navManager)
        {
            //handle the first window.

            WindowManager.HandleNewWindow(Window.Current, navManager);

            if (Options.NavigationRoutingMethod == NavigationRoutingMethod.Dynamic)
                WindowManager.GetNavigationManagerForCurrentWindow().ProbeForViewViewModelPairs();
        }

        private async Task InitializeRootFrameAsync(IActivatedEventArgs e)
        {
            InitializeIoC();

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                var navManager = new NavigationManager(this);

                var navService = new NavigationService(rootFrame, navManager);
                navService.NavigationLevel = FrameLevel.One;

                navManager.RootNavigationService = navService;

                InitializeNavigation(navManager);
            }


            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //Resurrection!

                StorageFile suspensionStateFile = await CrystalApplication.CrystalDataFolder.GetFileAsync(PreservationManager.SuspensionStateFileName);

                if (suspensionStateFile != null)
                {
                    var eventArgs = new CrystalApplicationShouldRestoreEventArgs();
                    eventArgs.SuspensionFileDate = suspensionStateFile.DateCreated;
                    if (OnApplicationShouldRestore(eventArgs))
                    {
                        if (await PreservationManager.RestoreAsync() == true)
                        {
                            //navService.HandleTerminationReload();

                            //todo handle multiple windows in this case.

                            try
                            {
                                await suspensionStateFile.DeleteAsync();
                            }
                            catch (Exception)
                            {
                            }

                            IsRestored = true;

                            if (Restored != null)
                                Restored(this, EventArgs.Empty);
                        }
                    }
                }
            }



            // Ensure the current window is active
            //Window.Current.Activate();

            HandleBackNavigation();

            WindowManager.GetStatusManagerForCurrentWindow().Initialize();
        }

        private void HandleBackNavigation()
        {
            if (Options.HandleSystemBackNavigation)
            {
                EventHandler<BackRequestedEventArgs> systemBackHandler = null;
                systemBackHandler = new EventHandler<BackRequestedEventArgs>((object sender, BackRequestedEventArgs args) =>
                {
                    if (Options.HandleSystemBackNavigation)
                    {
                        //walk down the navigation tree (by FrameLevel) and check if each service wants to handle it

                        foreach (var service in WindowManager.GetNavigationManagerForCurrentWindow()
                                                .GetAllServices()
                                                .OrderByDescending(x => x.NavigationLevel))
                        {
                            if (service.SignalPreBackRequested())
                            {
                                args.Handled = true;

                                WindowManager.GetWindowServiceForCurrentWindow()
                                .RefreshAppViewBackButtonVisibility();

                                return;
                            }

                            if (service.CanGoBackward)
                            {
                                service.GoBack();

                                WindowManager.GetWindowServiceForCurrentWindow()
                                .RefreshAppViewBackButtonVisibility();

                                args.Handled = true;

                                return;
                            }
                        }
                    }
                });
                SystemNavigationManager.GetForCurrentView().BackRequested += systemBackHandler;
            }
        }

        public IActivatedEventArgs LastActivationArgs { get; private set; }

        protected sealed override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            LastActivationArgs = args;

            if (args.PreviousExecutionState != ApplicationExecutionState.Running && args.PreviousExecutionState != ApplicationExecutionState.Suspended && args.TileId == "App")
            {
                await InitializeRootFrameAsync(args);

                if (args.PreviousExecutionState != ApplicationExecutionState.Terminated || (args.PreviousExecutionState == ApplicationExecutionState.Terminated && !IsRestored))
                {

                    await AsyncWindowActivate(OnFreshLaunchAsync(args));
                }
                else
                {
                    Window.Current.Activate();
                }
            }
            else
            {
                OnActivated(args);
            }
        }

        protected sealed override async void OnActivated(IActivatedEventArgs args)
        {
            LastActivationArgs = args;

            if (args.PreviousExecutionState != ApplicationExecutionState.Running && args.PreviousExecutionState != ApplicationExecutionState.Suspended)
                await InitializeRootFrameAsync(args);

            await AsyncWindowActivate(OnActivationAsync(args));
        }

        private sealed override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            //var deferral = args.TaskInstance.GetDeferral();
            await OnBackgroundActivatedAsync(args);
            //deferral.Complete();
        }

        public virtual Task OnBackgroundActivatedAsync(BackgroundActivatedEventArgs args)
        {
            if (BackgroundActivated != null)
                BackgroundActivated(this, new CrystalApplicationBackgroundActivationEventArgs() { ActivationEventArgs = args });

            return Task.CompletedTask;
        }

        private async Task AsyncWindowActivate(Task activationTask)
        {
            // Ensure the current window is active
            await Task.WhenAny(activationTask, Task.Delay(5000));

            Window.Current.Activate();
        }

        public abstract Task OnFreshLaunchAsync(LaunchActivatedEventArgs args);


        public static IUIDispatcher Dispatcher { get { return InversionOfControl.IoC.Current.Resolve<IUIDispatcher>(); } }

#if RELEASE
        [DebuggerNonUserCode]
#endif
        private async void CrystalApplication_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            //https://blogs.windows.com/buildingapps/2016/06/07/background-activity-with-the-single-process-model/

            foreach (var window in WindowManager.GetAllWindowServices())
            {
                var viewModelsInWindow = window.NavigationManager.GetAllNavigatedViewModels();

                foreach (var viewModel in viewModelsInWindow)
                {
                    if (viewModel != null)
                    {
                        switch (Options.ViewModelRefreshMethod)
                        {
                            case ViewModelRefreshMethod.OnRefreshingAsync:
                                await viewModel.OnRefreshingAsync();
                                break;
                            case ViewModelRefreshMethod.OnNavigatedToRefresh:
                                viewModel.OnNavigatedTo(sender, new CrystalNavigationEventArgs() { Direction = CrystalNavigationDirection.Refresh });
                                break;
                        }

                    }
                }
            }

            await OnForegroundingAsync();
        }

#if RELEASE
        [DebuggerNonUserCode]
#endif
        private async void CrystalApplication_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            /// https://blogs.windows.com/buildingapps/2016/06/07/background-activity-with-the-single-process-model/
            /// Corresponds to EnteredBackground event. State should now be saved here.

            var deferral = e.GetDeferral();

            try
            {
                await PreservationManager.PreserveAsync(OnBackgroundingAsync());
            }
            catch (Exception)
            {

            }
            finally
            {
                deferral.Complete();
            }
        }

#if RELEASE
        [DebuggerNonUserCode]
#endif
        private async void CrystalApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            try
            {
                await OnSuspendingAsync();
            }
            catch (Exception)
            {

            }
            finally
            {
                deferral.Complete();
            }
        }

#if RELEASE
        [DebuggerNonUserCode]
#endif
        private async void CrystalApplication_Resuming(object sender, object e)
        {
            await OnResumingAsync();

            foreach (var window in WindowManager.GetAllWindowServices())
            {
                var viewModelsInWindow = window.NavigationManager.GetAllNavigatedViewModels();

                foreach (var viewModel in viewModelsInWindow)
                {
                    if (viewModel != null)
                    {
                        await viewModel.OnResumingAsync();
                    }
                }
            }

            //Related: CrystalApplication_LeavingBackground
        }

        /// <summary>
        /// Corresponds to EnteredBackground event. State should now be saved here.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnBackgroundingAsync() { return Task.CompletedTask; }
        protected internal virtual Task OnForegroundingAsync() { return Task.CompletedTask; }
        protected internal virtual Task OnSuspendingAsync() { return Task.CompletedTask; }
        protected internal virtual Task OnResumingAsync() { return Task.CompletedTask; }
        protected internal virtual Task OnRestoringAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual bool OnApplicationShouldRestore(CrystalApplicationShouldRestoreEventArgs args)
        {
            //restore if the suspension file is less than 5 hours old.
            return (args.SuspensionFileDate != null ? (args.SuspensionFileDate - DateTime.Now) < TimeSpan.FromHours(5) : true);
        }

        public static Platform GetDevicePlatform()
        {
            if ((CrystalApplication.Current as CrystalApplication).Options.OverridePlatformDetection)
                return (CrystalApplication.Current as CrystalApplication).Options.OverridePlatformValue;
            else
                return SystemInformation.GetDevicePlatform();
        }

        protected internal virtual Type ResolveStaticPage(Type viewModelType)
        {
            return null;
        }

        public virtual Task OnActivationAsync(IActivatedEventArgs args)
        {
            return Task.CompletedTask;
        }

        public static CrystalApplication GetCurrentAsCrystalApplication()
        {
            return Current as CrystalApplication;
        }
    }
}
