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
            InitializeNavigation();
        }

        private static void InitializeNavigation()
        {
            NavigationManager.ProbeForViewViewModelPairs();

            EventHandler<BackRequestedEventArgs> systemBackHandler = null;
            systemBackHandler = new EventHandler<BackRequestedEventArgs>((object sender, BackRequestedEventArgs args) =>
            {
                //walk down the navigation tree and check if each service wants to handle it?
            });

            SystemNavigationManager.GetForCurrentView().BackRequested += systemBackHandler;
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

                NavigationManager.RootNavigationService = new NavigationService(rootFrame);
            }

            // Ensure the current window is active
            Window.Current.Activate();
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
