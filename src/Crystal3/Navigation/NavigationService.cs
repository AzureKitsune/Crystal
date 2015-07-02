using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public class NavigationService
    {
        private ViewModelBase lastViewModel = null;

        //TODO pass CrystalNavigationEventArgs instead of the built-in WinRT event args

        public Frame NavigationFrame { get; private set; }
        public FrameLevel NavigationLevel { get; internal set; }
        internal NavigationManager NavigationManager { get; set; }

        public bool CanGoBackward { get { return NavigationFrame.CanGoBack; } }

        public void GoBack() { NavigationFrame.GoBack(); }

        private Stack<ViewModelBase> viewModelBackStack = null;
        private Stack<ViewModelBase> viewModelForwardStack = null;

        internal NavigationService(Frame navFrame, NavigationManager manager)
        {
            if (navFrame == null) throw new ArgumentNullException("navFrame");

            NavigationManager = manager;
            NavigationFrame = navFrame;
            //NavigationFrame.DataContext = null;

            viewModelBackStack = new Stack<ViewModelBase>();
            viewModelForwardStack = new Stack<ViewModelBase>();

            NavigationManager.RegisterNavigationService(this);

            NavigationFrame.Navigating += NavigationFrame_Navigating;
            NavigationFrame.Navigated += NavigationFrame_Navigated;

            CrystalApplication.Current.Resuming += Current_Resuming;
            CrystalApplication.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

        }

        private void Current_Resuming(object sender, object e)
        {
            var currentPage = NavigationFrame.Content as Page;

            if (currentPage != null)
            {
                if (currentPage.DataContext == null)
                {
                    currentPage.DataContext = lastViewModel;
                }
            }
        }

        public bool IsNavigatedTo<T>() where T : ViewModelBase
        {
            return ((Page)NavigationFrame.Content).DataContext is T;
        }

        private void NavigationFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {

        }

        private void NavigationFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                //so the following line (view in git history) seems to point out a possible bug. when using inline navigation, the inline-page's datacontext reverts to the datacontext of the frame's parent.
                //... mo-code, mo-problems - we create a new instance to solve that problem.
                //TODO implement event/hook for injecting cached viewmodels

                //ViewModelBase lastViewModel = default(ViewModelBase);
                if (lastViewModel != null)
                {
                    lastViewModel.OnNavigatedFrom(sender, new CrystalNavigationEventArgs(e));

                    viewModelForwardStack.Push(lastViewModel);
                }

                var viewModel = viewModelBackStack.Pop();

                try
                {
                    if (NavigationServicePreNavigatedSignaled != null)
                        NavigationServicePreNavigatedSignaled(this, new NavigationServicePreNavigatedSignaledEventArgs(viewModel, new CrystalNavigationEventArgs(e)));
                }
                catch (Exception) { }

                if (viewModel == null) throw new Exception();

                ((Page)e.Content).DataContext = viewModel;

                viewModel.OnNavigatedTo(this, new CrystalNavigationEventArgs(e));

                lastViewModel = viewModel;
            }
        }

        internal void HandleTerminationReload()
        {
            //since the page is going to be created, we need to recreate the viewmodel and inject it.

            if (((Page)NavigationFrame.Content).DataContext == null) //sanity check
            {
                //gran and create the viewmodel as if we were navigating to it.
                var viewModelType = NavigationManager.GetViewModelType(((Page)NavigationFrame.Content).GetType());
                var viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                viewModel.NavigationService = this;

                ((Page)NavigationFrame.Content).DataContext = viewModel; //set the datacontext


                //simulate the navigation events.
                viewModel.OnNavigatingTo(null, new CrystalNavigationEventArgs());
                viewModel.OnNavigatedTo(null, new CrystalNavigationEventArgs());
                viewModel.OnResumingAsync();
            }
        }

        internal NavigationService(Frame navFrame, NavigationManager manager, FrameLevel navigationLevel) : this(navFrame, manager)
        {
            NavigationLevel = navigationLevel;
        }

        public void NavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            var view = NavigationManager.GetViewType(typeof(T));

            if (view == null) throw new Exception("View not found!");

            ViewModelBase viewModel = null;

            NavigatingCancelEventHandler navigatingHandler = null;
            navigatingHandler = new NavigatingCancelEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) =>
            {
                NavigationFrame.Navigating -= navigatingHandler;

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh)
                {
                    viewModel = Activator.CreateInstance(typeof(T)) as ViewModelBase;
                    viewModel.NavigationService = this;

                    if (lastViewModel != null)
                        e.Cancel = lastViewModel.OnNavigatingFrom(sender, new CrystalNavigationEventArgs(e));

                    viewModel.OnNavigatingTo(sender, new CrystalNavigationEventArgs(e));
                }
                //else if (e.NavigationMode == NavigationMode.Back)
                //{
                //    e.Cancel = viewModel.OnNavigatingFrom(sender, e);
                //}
            });

            NavigatedEventHandler navigatedHandler = null;
            navigatedHandler = new NavigatedEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e) =>
            {
                NavigationFrame.Navigated -= navigatedHandler;

                try
                {
                    if (NavigationServicePreNavigatedSignaled != null)
                        NavigationServicePreNavigatedSignaled(this, new NavigationServicePreNavigatedSignaledEventArgs(viewModel, new CrystalNavigationEventArgs(e)));
                }
                catch (Exception) { }

                if (e.NavigationMode == NavigationMode.New)
                {
                    if (lastViewModel != null)
                    {
                        lastViewModel.OnNavigatedFrom(sender, new CrystalNavigationEventArgs(e));

                        viewModelBackStack.Push(lastViewModel);
                    }

                    Page page = e.Content as Page;

                    //page.NavigationCacheMode = NavigationCacheMode.Enabled;

                    if (viewModel == null) throw new Exception();

                    page.DataContext = viewModel;

                    //page.SetValue(FrameworkElement.DataContextProperty, viewModel);

                    viewModel.OnNavigatedTo(sender, new CrystalNavigationEventArgs(e));

                    lastViewModel = viewModel;

                }
                //else if (e.NavigationMode == NavigationMode.Back)
                //{
                //    viewModel.OnNavigatedFrom(sender, e);
                //}


            });


            NavigationFrame.Navigated += navigatedHandler;
            NavigationFrame.Navigating += navigatingHandler;

            NavigationFrame.Navigate(view, parameter);
        }

        /// <summary>
        /// A workaround for .NET event's first-subscribe, last-fire approach.
        /// </summary>
        public event EventHandler<NavigationServicePreNavigatedSignaledEventArgs> NavigationServicePreNavigatedSignaled;
    }
}
