using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public class NavigationService
    {
        public Frame NavigationFrame { get; private set; }
        public FrameLevel NavigationLevel { get; private set; }
        internal NavigationManager NavigationManager { get; set; }

        internal NavigationService(Frame navFrame, NavigationManager manager)
        {
            if (navFrame == null) throw new ArgumentNullException("navFrame");

            NavigationManager = manager;

            NavigationFrame = navFrame;

            NavigationManager.RegisterNavigationService(this);
        }

        internal NavigationService(Frame navFrame, NavigationManager manager, FrameLevel navigationLevel) : this(navFrame, manager)
        {
            NavigationLevel = navigationLevel;
        }

        public void NavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            var view = NavigationManager.GetView(typeof(T));

            if (view == null) throw new Exception("View not found!");

            ViewModelBase viewModel = Activator.CreateInstance(typeof(T)) as ViewModelBase;
            viewModel.NavigationService = this;

            NavigatingCancelEventHandler navigatingHandler = null;
            navigatingHandler = new NavigatingCancelEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) =>
            {
                NavigationFrame.Navigating -= navigatingHandler;

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Forward)
                {
                    e.Cancel = viewModel.OnNavigatingTo(sender, e);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    e.Cancel = viewModel.OnNavigatingFrom(sender, e);
                }
            });

            NavigatedEventHandler navigatedHandler = null;
            navigatedHandler = new NavigatedEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e) =>
            {
                NavigationFrame.Navigated -= navigatedHandler;

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Forward)
                {
                    viewModel.OnNavigatedTo(sender, e);

                    Page page = e.Content as Page;
                    page.DataContext = viewModel;
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    viewModel.OnNavigatedFrom(sender, e);
                }
            });


            NavigationFrame.Navigated += navigatedHandler;
            NavigationFrame.Navigating += navigatingHandler;

            NavigationFrame.Navigate(view, parameter);
        }

    }
}
