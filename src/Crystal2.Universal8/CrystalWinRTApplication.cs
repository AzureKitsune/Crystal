using Crystal2.Core;
using Crystal2.IOC;
using Crystal2.Model;
using Crystal2.Navigation;
using Crystal2.State;
using Crystal2.UI;
using Crystal2.UI.MessageDialog;
using Crystal2.UI.SplashScreen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Crystal2
{
    /// <summary>
    /// Implements ICrystalApplication for WinRT and WinPRT (Windows Phone) platforms.
    /// </summary>
    public abstract class CrystalWinRTApplication : Application, ICrystalApplication
    {
        private TransitionCollection transitions;
        internal CrystalWinRTConfiguration applicationConfiguration = null;
        private bool isResuming = false;
        internal W8NavigationProvider GlobalNavigationProvider { get; private set; }

        public CrystalWinRTApplication()
            : base()
        {
            applicationConfiguration = new CrystalWinRTConfiguration();

            OnPreinitialize(applicationConfiguration);

            if (applicationConfiguration.AutomaticallyCallInitializeComponent)
            {
                var initializeComponent = this.GetType().GetTypeInfo().GetDeclaredMethod("InitializeComponent");
                if (initializeComponent != null)
                    initializeComponent.Invoke(this, new object[] { });
            }

            this.Suspending += this.OnSuspending;
            this.Resuming += this.OnResuming;

            DetectPlatform();

            OnInitialize();
        }

        /// <summary>
        /// Called when a running, non-tombstoned application resumes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DebuggerNonUserCode]
        private async void OnResuming(object sender, object e)
        {
            //isResuming = true;

            if (NavigationManager.CurrentViewModel != null)
            {
                switch (applicationConfiguration.ResumingRefreshMethod)
                {
                    case CrystalWinRTResumingRefreshMethodEnum.OnRefresh:
                        NavigationManager.CurrentViewModel.OnRefresh();
                        break;
                    case CrystalWinRTResumingRefreshMethodEnum.ResetNavigation:
                        NavigationManager.CurrentViewModel.OnNavigatedTo(null, new CrystalWinRTNavigationEventArgs(null) { Direction = CrystalNavigationDirection.Reset });
                        break;
                    default:
                        break;
                }
            }

            await OnResumingAsync();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        /// 
        [DebuggerNonUserCode]
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity

            if (applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState)
            {
                if (IoCManager.IsRegistered<IStateProvider>())
                {
                    var state = IoCManager.Resolve<IStateProvider>().State;

                    if (state != null)
                    {
                        state.NavigationState = GlobalNavigationProvider.GetNavigationContext() as string;

                        var viewModel = GlobalNavigationProvider.GetCurrentViewModel();
                        if (viewModel is IStateHandlingViewModel)
                        {
                            var stateObjs = new Dictionary<string, object>();
                            ((IStateHandlingViewModel)viewModel).OnPreserve(stateObjs);

                            state.StateObjects = new Collection<object[]>(stateObjs.Select(x => new object[] { x.Key, x.Value }).ToArray());
                        }
                        else
                            state.StateObjects = new Collection<object[]>();
                    }
                }
            }

            try
            {
                if (IsPhone())
                {
                    //Windows Phone 8.1 doesn't call OnNavigatedFrom when Suspending.
                    //This simulates it.
                    NavigationManager.CurrentViewModel.OnNavigatedFrom(new CrystalWinRTNavigationEventArgs(null)
                    {
                        Direction = CrystalNavigationDirection.None
                    });
                }

                await OnSuspendingAsync();

                if (IoCManager.IsRegistered<IStateProvider>())
                {
                    var stateProvider = IoCManager.Resolve<IStateProvider>();
                    if (stateProvider.State.NavigationState != null)
                        await stateProvider.SaveStateAsync();
                }
            }
            catch (Exception) { throw; }

            deferral.Complete();
        }

        /// <summary>
        /// Detects if we're running WinRT or WinPRT.
        /// </summary>
        private void DetectPlatform()
        {
            try
            {
                //Calls an unsupported API.
                Windows.Networking.BackgroundTransfer.BackgroundDownloader.RequestUnconstrainedDownloadsAsync(null).GetResults();
                CurrentPlatform = Platform.Windows;
            }
            catch (NotImplementedException)
            {
                //The API isn't supported on Windows Phone. Thus, the current platform is Windows Phone.
                CurrentPlatform = Platform.WindowsPhone;
            }
            catch (Exception)
            {
                //Otherwise, this is Windows.
                CurrentPlatform = Platform.Windows;
            }
        }

        public void OnInitialize()
        {
            //set up the dispatcher
            if (!IoCManager.IsRegistered<IUIDispatcher>())
                IoCManager.Register<IUIDispatcher>(new WinRTDispatcher());

            if (applicationConfiguration.AutomaticallyShowExtendedSplashScreen)
            {
                if (!IoCManager.IsRegistered<IWinRTSplashScreenProvider>())
                    IoCManager.Register<IWinRTSplashScreenProvider>(new DefaultSplashScreenProvider());

                //handle the splashscreen
                Tuple<string, string> splashData = applicationConfiguration.AutoDetectSplashScreenImage ? GetSplashScreenPath() : null;
                var splashBackground = splashData.Item1;
                var splashImagePath = splashData.Item2;

                IoCManager.Resolve<IWinRTSplashScreenProvider>().Setup(splashBackground, splashImagePath);
            }

            Parallel.Invoke(InitializeNavigation, () => { if (IsPhone()) { InitializePhoneStuff(); } });

            //set up the message dialog stuff
            if (!IoCManager.IsRegistered<IMessageDialogProvider>())
                IoCManager.Register<IMessageDialogProvider>(new DefaultMessageDialogProvider());

            //set up state management
            if (applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState)
            {
                if (!IoCManager.IsRegistered<IStateProvider>())
                    IoCManager.Register<IStateProvider>(new DefaultStateProvider());
            }
        }

        private void InitializePhoneStuff()
        {
            //If running on the phone, dynamically load the referenced Crystal2.Universal8.Phone.dll for Back button functionality and such.

            var files = Package.Current.InstalledLocation.GetFilesAsync().AsTask().Result;
            var refFile = files.FirstOrDefault(x => x.FileType == ".dll" && x.Name == "Crystal2.Universal8.Phone.dll");

            if (refFile == null) return;

            var name = refFile.DisplayName;

            var assemblyName = new AssemblyName(name);
            Assembly assembly = null;

            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            { }

            if (assembly != null)
            {
                //load back button code
                var phoneBackButtonHandler = (IBackButtonNavigationProvider)Activator.CreateInstance(assembly.ExportedTypes.First(
                    x => x.GetTypeInfo().ImplementedInterfaces.Any(y => y == (typeof(IBackButtonNavigationProvider)))));

                phoneBackButtonHandler.Attach(this);

                IoCManager.Register<IBackButtonNavigationProvider>(phoneBackButtonHandler);

                var statusBarProvider = (IStatusBarProvider)Activator.CreateInstance(assembly.ExportedTypes.First(
                    x => x.GetTypeInfo().ImplementedInterfaces.Any(y => y == (typeof(IStatusBarProvider)))));

                IoCManager.Register<IStatusBarProvider>(statusBarProvider);
            }
        }

        private void InitializeNavigation()
        {
            //set up navigation
            IoCManager.Register<INavigationDirectoryProvider>(new W8NavigationDirectoryProvider(this.GetType().GetTypeInfo().Assembly, applicationConfiguration.AutomaticallyDiscoverViewModelPairs));
            if (!applicationConfiguration.AutomaticallyDiscoverViewModelPairs)
            {
                OnNavigationInitializeOverride(IoCManager.Resolve<INavigationDirectoryProvider>() as Crystal2.Navigation.W8NavigationDirectoryProvider);
            }

            GlobalNavigationProvider = new W8NavigationProvider();
            IoCManager.Register<INavigationProvider>(GlobalNavigationProvider);
        }

        private Tuple<string, string> GetSplashScreenPath()
        {
            //originally from http://tonychampion.net/blog/index.php/2013/01/examining-the-windows-store-apps-appxmanifest-at-runtime/
            //thanks to Tony
            //modified by Amrykid

            var doc = XDocument.Load("AppxManifest.xml", LoadOptions.None);

            var namespaces = doc.Root.Attributes();
            var m2 = XNamespace.Get(namespaces.First(x => x.IsNamespaceDeclaration && x.Name.LocalName == "m2").Value);

            XNamespace m3 = null;
            var m3Attr = namespaces.FirstOrDefault(x => x.IsNamespaceDeclaration && x.Name.LocalName == "m3");
            if (m3Attr != null)
                m3 = XNamespace.Get(m3Attr.Value);


            // Get the SplashScreen node located at Package/Applications/Application/VisualElements/SplashScreen
            var splashScreenElement = doc.Descendants(m2 + "SplashScreen").FirstOrDefault();

            // The Image attribute holds the local path to the Splash Screen image for the application
            var splashScreenPath = string.Empty;

            var splashBackgroundColor = string.Empty;

            if (splashScreenElement != null)
            {
                splashScreenPath = splashScreenElement.Attribute("Image").Value;

                if (splashScreenElement.Attributes().Any(x => x.Name == "BackgroundColor"))
                    splashBackgroundColor = splashScreenElement.Attribute("BackgroundColor").Value;
                else
                {
                    var visualElementsElement = doc.Descendants(m2 + "VisualElements").First();

                    splashBackgroundColor = visualElementsElement.Attribute("BackgroundColor").Value;
                }
            }
            else
            {
                splashScreenElement = doc.Descendants(m3 + "SplashScreen").First();

                splashScreenPath = splashScreenElement.Attribute("Image").Value;

                var visualElementsElement = doc.Descendants(m3 + "VisualElements").First();

                splashBackgroundColor = visualElementsElement.Attribute("BackgroundColor").Value;
            }

            return new Tuple<string, string>(splashBackgroundColor, splashScreenPath);
        }

        protected override async void OnLaunched(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs e)
        {
            bool restored = false;
            bool initialStart = false;

            if (e.PreviousExecutionState == ApplicationExecutionState.NotRunning || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser
                || e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //see: http://msdn.microsoft.com/en-us/library/windows/apps/windows.applicationmodel.activation.applicationexecutionstate.aspx

                restored = await HandleInitialNavigation(e);
                initialStart = true;
            }


            if (e.TileId == "App") //primary tile activation or toast activation.
            {
                if ((initialStart && !CheckIfToastActivation(e)))
                {
                    await _CheckAndWaitForSplashScreenDismissal(restored); //handles if the extended splash screen is enabled, awaiting for it to complete if it is.
                    await IoCManager.Resolve<IUIDispatcher>().RunAsync(() =>
                    {
                        ContinueLaunching(e);
                    });
                }
                else if (CheckIfToastActivation(e))
                {
                    //toast notification?
                    await _CheckAndWaitForSplashScreenDismissal(restored); //handles if the extended splash screen is enabled, awaiting for it to complete if it is.
                    await IoCManager.Resolve<IUIDispatcher>().RunAsync(() =>
                    {
                        //toast activation?
                        OnActivated(e);
                    });
                }
                else
                {
                    await IoCManager.Resolve<IUIDispatcher>().RunAsync(() =>
                    {
                        OnResetLaunchNavigationReady(e);
                    });
                }
            }
            else
            {
                await _CheckAndWaitForSplashScreenDismissal(restored); //handles if the extended splash screen is enabled, awaiting for it to complete if it is.
                Window.Current.Activate();
                await IoCManager.Resolve<IUIDispatcher>().RunAsync(() =>
                    {
                        //secondary tile activation
                        OnSecondaryTileLaunchNavigationReady(e);
                    });
            }

        }

        private static bool CheckIfToastActivation(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs e)
        {
            //todo find other ways to check
            if (e == null) return false;
            return e.Kind == ActivationKind.Launch && !string.IsNullOrWhiteSpace(e.Arguments);
        }

        private async Task _CheckAndWaitForSplashScreenDismissal(bool restored)
        {
            if (applicationConfiguration.AutomaticallyShowExtendedSplashScreen && !restored)
            {
                var splashProvider = IoCManager.Resolve<IWinRTSplashScreenProvider>();

                await splashProvider.CompletionTask;

                RootFrame.Content = null;
            }
        }

        private void ContinueLaunching(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs e)
        {
            if ((e.PreviousExecutionState != ApplicationExecutionState.Terminated && applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState)
                || !applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState)
                OnNormalLaunchNavigationReady(e);


            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task<bool> HandleInitialNavigation(IActivatedEventArgs e, bool noRestore = false)
        {
            bool restoredState = false;

            RootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                RootFrame.CacheSize = 1;

                //Now that we have a frame, initialize the INavigationProvider.
                /*IoCManager.Resolve<INavigationProvider>()
                    .Setup(RootFrame);*/
                GlobalNavigationProvider.Setup(RootFrame);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //Begins restoring the state of a tombstoned application. At this point, the WP OS will show "Resuming..." but this is unrelated to the Resuming event
                    //The Resuming event is only called for non-tombstoned applications.
                    #region restoring state
                    if (applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState && !noRestore)
                    {
                        if (IoCManager.IsRegistered<IStateProvider>())
                        {
                            var stateProvider = IoCManager.Resolve<IStateProvider>();

                            if (stateProvider.CanStateBeStored)
                            {
                                await stateProvider.LoadStateAsync();

                                var state = stateProvider.State;

                                if (state.NavigationState != null)
                                {
                                    GlobalNavigationProvider.SetNavigationContext(state.NavigationState);

                                    var provider = IOC.IoCManager.Resolve<INavigationDirectoryProvider>();

                                    var map = provider.ProvideMap();

                                    Type selectedPageViewModel = (Type)map.First(x =>
                                       GlobalNavigationProvider.GetUrl() == ((Tuple<Type, Uri, bool>)x.Value).Item2).Key;

                                    ViewModelBase viewModel = (ViewModelBase)Activator.CreateInstance(selectedPageViewModel);

                                    ((Page)((Frame)GlobalNavigationProvider.NavigationObject).Content).DataContext = viewModel;

                                    viewModel.OnNavigatedTo(null, new CrystalWinRTNavigationEventArgs(null) { Direction = CrystalNavigationDirection.Forward });

                                    if (viewModel is IStateHandlingViewModel)
                                        ((IStateHandlingViewModel)viewModel).OnRestore(state.StateObjects.ToDictionary(x => (string)x[0], y => y[1]));

                                    restoredState = true;
                                }
                            }
                        }
                    }
                    #endregion
                }

                //splash screen will set this later. only set this if the splash screen isn't enabled.
                if (!applicationConfiguration.AutomaticallyShowExtendedSplashScreen)
                {
                    // Place the frame in the current Window
                    Window.Current.Content = RootFrame;
                }

                if (RootFrame.Content == null)
                {
                    if (IsPhone())
                    {
                        //#if WINDOWS_PHONE_APP
                        // Removes the turnstile navigation for startup.
                        if (RootFrame.ContentTransitions != null)
                        {
                            this.transitions = new TransitionCollection();
                            foreach (var c in RootFrame.ContentTransitions)
                            {
                                this.transitions.Add(c);
                            }
                        }

                        RootFrame.ContentTransitions = null;
                        RootFrame.Navigated += this.RootFrame_FirstNavigated;
                        //#endif
                    }
                }
            }

            if (applicationConfiguration.AutomaticallyShowExtendedSplashScreen)
                IoCManager.Resolve<IWinRTSplashScreenProvider>()
                    .Preload();

            //the following code is me jumping through hoops to make sure the splash screen is showing while the callback is firing.
            if (!restoredState)
            {
                if (applicationConfiguration.AutomaticallyShowExtendedSplashScreen)
                {
                    if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                    {
                        var splashProvider = IoCManager.Resolve<IWinRTSplashScreenProvider>();

                        var splashTask = new Task<Task>(() => OnSplashScreenShownAsync());
                        splashProvider.PreActivationHook(e, splashTask);
                    }
                }
            }

            return restoredState;
        }


        #region Activation handling
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.PreviousExecutionState != ApplicationExecutionState.Running)
                await HandleInitialNavigation(args, noRestore: true);

            if (CheckIfToastActivation(args as LaunchActivatedEventArgs))
                OnToastActivationNavigationReady(args);
            else
                OnActivationNavigationReady((IActivatedEventArgs)args);

            // Ensure the current window is active
            Window.Current.Activate();
            base.OnActivated(args);
        }
        protected override async void OnSearchActivated(SearchActivatedEventArgs args)
        {
            await HandleInitialNavigation(args, noRestore: true);

            OnActivationNavigationReady(args);

            // Ensure the current window is active
            Window.Current.Activate();
            base.OnSearchActivated(args);
        }
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await HandleInitialNavigation(args, noRestore: true);

            OnActivationNavigationReady(args);

            // Ensure the current window is active
            Window.Current.Activate();
            base.OnShareTargetActivated(args);
        }
        #endregion

        /// <summary>
        /// Returns if the application is running on Windows Phone.
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone()
        {
            return CurrentPlatform == Platform.WindowsPhone;
        }

        /// <summary>
        /// Returns the WinRT current platform.
        /// </summary>
        public static Platform CurrentPlatform { get; private set; }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection(); // { new Windows.UI.Xaml.Media.Animation.NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// The root frame created by Crystal.
        /// </summary>
        protected internal Frame RootFrame { get; private set; }

        /// <summary>
        /// An abstract method called when the application is ready to navigate.
        /// </summary>
        protected abstract void OnNormalLaunchNavigationReady(Windows.ApplicationModel.Activation.IActivatedEventArgs args);

        [DebuggerNonUserCode]
        protected virtual void OnResetLaunchNavigationReady(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            try
            {
                NavigationManager.CurrentViewModel.OnRefresh();
            }
            catch (Exception) { }
        }

        protected virtual void OnSecondaryTileLaunchNavigationReady(Windows.ApplicationModel.Activation.IActivatedEventArgs args) { }

        protected virtual void OnActivationNavigationReady(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        { }

        protected virtual void OnToastActivationNavigationReady(Windows.ApplicationModel.Activation.IActivatedEventArgs args) { }

        [DebuggerNonUserCode]
        protected virtual void OnNavigationInitializeOverride(Crystal2.Navigation.W8NavigationDirectoryProvider directoryProvider)
        {
            if (!applicationConfiguration.AutomaticallyDiscoverViewModelPairs)
                throw new Exception("This function must be overriden if AutomaticallyDiscoverViewModelPairs is false.");
        }

        protected virtual Task OnSuspendingAsync()
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Called when a running, non-tombstoned application is resumed
        /// </summary>
        /// <remarks>Internal: See Resuming Event</remarks>
        /// <returns></returns>
        protected virtual Task OnResumingAsync()
        {
            return Task.FromResult<object>(null);
        }

        protected virtual Task OnSplashScreenShownAsync()
        {
            return Task.Delay(1);
        }

        public virtual void OnPreinitialize(CrystalConfiguration _options)
        {
            CrystalWinRTConfiguration options = (CrystalWinRTConfiguration)_options;

            options.AutoDetectSplashScreenImage = false;
            options.AutomaticallyShowExtendedSplashScreen = false;
            options.AutomaticallyDiscoverViewModelPairs = true;
            options.AutomaticallyCallInitializeComponent = true;
        }

        public static new CrystalWinRTApplication Current
        {
            get
            {
                return Application.Current as CrystalWinRTApplication;
            }
        }
    }
}
