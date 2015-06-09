using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3
{
    public abstract class CrystalApplication: Application
    {
        public CrystalConfiguration Options { get; private set; }


        public CrystalApplication() : base()
        {
            Options = new CrystalConfiguration();
            OnConfigure();

            //base.InitializeComponent();

            InitializeNavigation();
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
            }
        }

        private void InitializeNavigation()
        {
            //handle the first window.

            WindowManager.HandleNewWindow(Window.Current, new NavigationManager());
            WindowManager.GetNavigationManagerForCurrentWindow().ProbeForViewViewModelPairs();
        }

        private void InitializeRootFrame(IActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                WindowManager.GetNavigationManagerForCurrentWindow().RootNavigationService = new NavigationService(rootFrame, WindowManager.GetNavigationManagerForCurrentWindow());
            }

            // Ensure the current window is active
            Window.Current.Activate();


            if (Options.HandleSystemBackNavigation)
            {
                EventHandler<BackRequestedEventArgs> systemBackHandler = null;
                systemBackHandler = new EventHandler<BackRequestedEventArgs>((object sender, BackRequestedEventArgs args) =>
                {
                    if (Options.HandleSystemBackNavigation)
                    {
                        //walk down the navigation tree and check if each service wants to handle it?
                    }
                });

                SystemNavigationManager.GetForCurrentView().BackRequested += systemBackHandler;
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            InitializeRootFrame(args);

            OnFreshLaunch(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            InitializeRootFrame(args);
        }

        public abstract void OnFreshLaunch(LaunchActivatedEventArgs args);
    }
}
