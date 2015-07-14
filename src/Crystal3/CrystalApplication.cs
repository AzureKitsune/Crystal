using Crystal3.Core;
using Crystal3.IOC;
using Crystal3.Model;
using Crystal3.Navigation;
using Crystal3.UI.Dispatcher;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3
{
    public abstract class CrystalApplication : Application
    {
        StorageFolder CrystalDataFolder = null;

        public CrystalConfiguration Options { get; private set; }


        public CrystalApplication() : base()
        {
            Options = new CrystalConfiguration();
            OnConfigure();
            InitializeDataFolder();

            //base.InitializeComponent();
            InitializeNavigation();

            this.Resuming += CrystalApplication_Resuming;
            this.Suspending += CrystalApplication_Suspending;
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
            Options.HandleSystemBackNavigation = true;
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            if (args.Window != WindowManager.GetAllWindows().First()) //make sure it isn't our first window.
            {
                var frame = new Frame();
                var navManager = new NavigationManager();

                navManager.RootNavigationService = new NavigationService(frame, navManager);
                args.Window.Content = frame;

                WindowManager.HandleNewWindow(args.Window, navManager);

                if (Options.HandleBackButtonForTopLevelNavigation && Options.HandleBackButtonForTopLevelNavigation)
                {
                    //todo remember to remove this event handler when the view closes
                    navManager.RootNavigationService.NavigationFrame.Navigated += HandleTopLevelNavigationForBackButton_NavigationFrame_Navigated;

                }
            }
        }

        private void InitializeIoC()
        {
            if (!IoCManager.IsRegistered<IUIDispatcher>())
                IoCManager.Register<IUIDispatcher>(new UIDispatcher(Window.Current.Dispatcher));
        }

        private void InitializeNavigation()
        {
            //handle the first window.

            WindowManager.HandleNewWindow(Window.Current, new NavigationManager());
            WindowManager.GetNavigationManagerForCurrentWindow().ProbeForViewViewModelPairs();
        }

        private async Task InitializeRootFrameAsync(IActivatedEventArgs e)
        {
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

                var navService = new NavigationService(rootFrame, WindowManager.GetNavigationManagerForCurrentWindow());
                navService.NavigationLevel = FrameLevel.One;
                WindowManager.GetNavigationManagerForCurrentWindow().RootNavigationService = navService;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //Resurrection!

                    await LoadAppState();

                    navService.HandleTerminationReload();
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            InitializeIoC();

            HandleBackNavigation();

            WindowManager.GetStatusManagerForCurrentWindow().Initialize();
        }

        private void HandleTopLevelNavigationForBackButton_NavigationFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
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
                        //walk down the navigation tree and check if each service wants to handle it

                        foreach (var service in WindowManager.GetNavigationManagerForCurrentWindow()
                                                .GetAllServices()
                                                .OrderByDescending(x => x.NavigationLevel))
                        {
                            if (service.CanGoBackward)
                            {
                                service.GoBack();

                                args.Handled = true;

                                return;
                            }
                        }
                    }
                });

                SystemNavigationManager.GetForCurrentView().BackRequested += systemBackHandler;


                if (Options.HandleBackButtonForTopLevelNavigation && Options.HandleBackButtonForTopLevelNavigation)
                {
                    WindowManager.GetNavigationManagerForCurrentWindow().RootNavigationService.NavigationFrame.Navigated += HandleTopLevelNavigationForBackButton_NavigationFrame_Navigated;

                }
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState != ApplicationExecutionState.Running && args.PreviousExecutionState != ApplicationExecutionState.Suspended)
            {
                await InitializeRootFrameAsync(args);

                if (args.PreviousExecutionState != ApplicationExecutionState.Terminated)
                    OnFreshLaunch(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await InitializeRootFrameAsync(args);

            OnActivation(args);
        }

        public abstract void OnFreshLaunch(LaunchActivatedEventArgs args);
        public virtual void OnActivation(IActivatedEventArgs args) { }

        public static IUIDispatcher Dispatcher { get { return IOC.IoCManager.Resolve<IUIDispatcher>(); } }

        private const string SuspensionStateFileName = "CrystalSuspensionState.txt";

        [DebuggerNonUserCode]
        private async void CrystalApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            var rootViewModel = WindowManager.GetRootViewModel();
            if (rootViewModel != null)
            {
                await rootViewModel.OnSuspendingAsync(null);
            }

            await SaveAppState();

            await OnSuspendingAsync();

            deferral.Complete();
        }

        [DebuggerNonUserCode]
        private async void CrystalApplication_Resuming(object sender, object e)
        {
            await OnResumingAsync();

            var rootViewModel = WindowManager.GetRootViewModel();
            if (rootViewModel != null)
            {
                await rootViewModel.OnResumingAsync();
            }
        }

        public virtual Task OnSuspendingAsync() { return Task.FromResult<object>(null); }
        public virtual Task OnResumingAsync() { return Task.FromResult<object>(null); }

        #region Loading and Saving AppState
        private async Task SaveAppState()
        {
            //todo allow the top level viewmodels to save and load their state via an IDictionary<string, object> object.

            var navigationState = WindowManager.GetNavigationManagerForCurrentWindow().RootNavigationService.NavigationFrame.GetNavigationState();

            StorageFile file = await CrystalDataFolder.CreateFileAsync(SuspensionStateFileName, CreationCollisionOption.OpenIfExists);

            await FileIO.WriteTextAsync(file, navigationState);
        }
        private async Task LoadAppState()
        {
            StorageFile file = await CrystalDataFolder.CreateFileAsync(SuspensionStateFileName, CreationCollisionOption.OpenIfExists);

            string navigationState = await FileIO.ReadTextAsync(file);

            WindowManager.GetNavigationManagerForCurrentWindow().RootNavigationService.NavigationFrame.SetNavigationState(navigationState);
        }
        #endregion

        public static Platform GetDevicePlatform()
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
