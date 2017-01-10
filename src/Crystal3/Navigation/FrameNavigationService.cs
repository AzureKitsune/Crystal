using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    /// <summary>
    /// Service for handling navigation on the user-level.
    /// </summary>
    public class FrameNavigationService : NavigationServiceBase
    {
        //TODO pass CrystalNavigationEventArgs instead of the built-in WinRT event args

        /// <summary>
        /// The frame attached to this service.
        /// </summary>
        public Frame NavigationFrame { get; protected set; }

        public override bool CanGoBackward { get { return NavigationFrame != null ? NavigationFrame.CanGoBack : false; } }

        internal FrameNavigationService(Frame navFrame, NavigationManager manager)
        {
            if (navFrame == null) throw new ArgumentNullException("navFrame");

            CoreInitialize(navFrame, manager);

            CrystalApplication.Current.Resuming += Current_Resuming;
            CrystalApplication.Current.Suspending += Current_Suspending;

            NavigationManager.RegisterNavigationService(this);
        }

        private void CoreInitialize(Frame navFrame, NavigationManager manager)
        {
            navigationLock = new ManualResetEvent(true);

            NavigationManager = manager;
            NavigationFrame = navFrame;
            //NavigationFrame.DataContext = null;

            viewModelBackStack = new Stack<ViewModelBase>();
            viewModelForwardStack = new Stack<ViewModelBase>();

            NavigationFrame.Navigating += NavigationFrame_Navigating;
            NavigationFrame.Navigated += NavigationFrame_Navigated;
            NavigationFrame.NavigationFailed += NavigationFrame_NavigationFailed;
            NavigationFrame.NavigationStopped += NavigationFrame_NavigationStopped;
        }

        internal FrameNavigationService(Frame navFrame, NavigationManager manager, FrameLevel navigationLevel) : this(navFrame, manager)
        {
            NavigationLevel = navigationLevel;
        }

        public FrameNavigationService(Frame navFrame)
        {
            CoreInitialize(navFrame, WindowManager.GetNavigationManagerForCurrentWindow());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Reset();

            NavigationFrame.Navigating -= NavigationFrame_Navigating;
            NavigationFrame.Navigated -= NavigationFrame_Navigated;
            NavigationFrame.NavigationFailed -= NavigationFrame_NavigationFailed;
            NavigationFrame.NavigationStopped -= NavigationFrame_NavigationStopped;

            navigationLock.Dispose();
        }

        public override void Reset()
        {
            ClearBackStack();
            NavigationFrame?.ForwardStack.Clear();
            viewModelForwardStack?.Clear();
            NavigationFrame.Content = null;
            lastViewModel = null;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

        }

        private void Current_Resuming(object sender, object e)
        {
            ///IIRC, handle if the page was recreated.... wtf.
            var currentPage = NavigationFrame.Content as Page;

            if (currentPage != null)
            {
                if (currentPage.DataContext == null)
                {
                    currentPage.DataContext = lastViewModel;
                }
            }
        }

        /// <summary>
        /// Goes backward in the back stack.
        /// </summary>
        public override void GoBack()
        {
            //Navigation is asynchronous so it had to be synchronized or else it would cause problems with an earlier app I was writing.
            navigationLock.WaitOne();

            navigationLock.Reset();
            NavigationFrame.GoBack();
        }

        /// <summary>
        /// Returns if the provided ViewModel's view is visible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override bool IsNavigatedTo<T>()
        {
            return ((Page)NavigationFrame.Content)?.DataContext is T;
        }
        public override bool IsNavigatedTo(Type viewModelType)
        {
            return (((Page)NavigationFrame.Content)?.DataContext)?.GetType() == viewModelType;
        }

        /// <summary>
        /// Returns the current view's view model. Niche usage.
        /// </summary>
        /// <returns></returns>
        public override ViewModelBase GetNavigatedViewModel()
        {
            return (ViewModelBase)((Page)NavigationFrame.Content)?.DataContext;
        }

        /// <summary>
        /// Clears the back stack.
        /// </summary>
        public override void ClearBackStack()
        {
            viewModelBackStack?.Clear();
            NavigationFrame?.BackStack.Clear();
        }


        private void NavigationFrame_NavigationStopped(object sender, NavigationEventArgs e)
        {
            navigationLock.Set();
        }

        private void NavigationFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Navigation failed", e.Exception);

            //navigationLock.Set();
            //e.Handled = true;
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
                    lastViewModel.OnNavigatedFrom(sender, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });

                    viewModelForwardStack.Push(lastViewModel);
                }

                if (viewModelBackStack.Count > 0)
                {
                    var viewModel = viewModelBackStack.Pop();


                    InvokePreNavigatedEvent(new NavigationServicePreNavigatedSignaledEventArgs(viewModel, new CrystalNavigationEventArgs(e)));


                    if (viewModel == null) throw new Exception();

                    ((Page)e.Content).DataContext = viewModel;

                    viewModel.OnNavigatedTo(this, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });

                    lastViewModel = viewModel;
                }
                else
                {
                    HandleTerminationReload(e);
                }

                navigationLock.Set();
            }
        }

        internal override void HandleTerminationReload(NavigationEventArgs args = null)
        {
            //since the page is going to be created, we need to recreate the viewmodel and inject it.

            var currentViewModel = ((Page)NavigationFrame.Content).DataContext;

            if (currentViewModel == null || NavigationManager.GetViewModelType(((Page)NavigationFrame.Content).GetType()) != currentViewModel?.GetType()) //sanity check
            {
                //gran and create the viewmodel as if we were navigating to it.
                var viewModelType = NavigationManager.GetViewModelType(((Page)NavigationFrame.Content).GetType());
                var viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                viewModel.NavigationService = this;

                ((Page)NavigationFrame.Content).DataContext = viewModel; //set the datacontext

                //simulate the navigation events.
                viewModel.OnNavigatingTo(null, new CrystalNavigationEventArgs(args) { Direction = CrystalNavigationDirection.Restore });
                viewModel.OnNavigatedTo(null, new CrystalNavigationEventArgs(args) { Direction = CrystalNavigationDirection.Restore });
                //viewModel.OnResumingAsync();

                lastViewModel = viewModel;


                InvokePreNavigatedEvent(new NavigationServicePreNavigatedSignaledEventArgs(viewModel, new CrystalNavigationEventArgs()));
            }
        }

        private CrystalNavigationDirection ConvertToCrystalNavDirections(NavigationMode dir)
        {
            switch (dir)
            {
                case NavigationMode.New:
                case NavigationMode.Forward:
                    return CrystalNavigationDirection.Forward;
                case NavigationMode.Refresh:
                    return CrystalNavigationDirection.Refresh;
                case NavigationMode.Back:
                    return CrystalNavigationDirection.Backward;
            }

            return CrystalNavigationDirection.None;
        }


        private Task waitForNavigationAsyncTask = null;
        private async Task WaitForNavigationLockAsync()
        {
            await Task.Delay(250);

            if (waitForNavigationAsyncTask?.Status == TaskStatus.Running)
                await waitForNavigationAsyncTask;
            else
            {
                waitForNavigationAsyncTask = Task.Run(() => navigationLock.WaitOne());

                await waitForNavigationAsyncTask;
            }
        }

        /// <summary>
        /// Navigates to the view that corresponds to the view model provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        public override void NavigateTo<T>(object parameter = null)
        {
            navigationLock.WaitOne();

            var view = GetViewType(typeof(T));

            NavigateBase(typeof(T), view, parameter);
        }

        public void Navigate(Type viewModelType, object parameter = null)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");

            navigationLock.WaitOne();

            var view = GetViewType(viewModelType);

            NavigateBase(viewModelType, view, parameter);
        }

        private void NavigateBase(Type viewModelType, Type view, object parameter)
        {
            if (view == null) throw new Exception("View not found!");

            ViewModelBase viewModel = null;

            bool useDataContext = (bool)view.GetTypeInfo().GetCustomAttribute<NavigationViewModelAttribute>()?.UseDataContextInsteadOfCreating;

            if (!useDataContext)
            {
                bool viewModelCachingEnabled = Crystal3.CrystalApplication.GetCurrentAsCrystalApplication().Options.EnableViewModelCaching;
                if (viewModelCachingEnabled)
                    viewModel = Crystal3.CrystalApplication.GetCurrentAsCrystalApplication().ResolveCachedViewModel(viewModelType);
            }

            NavigatingCancelEventHandler navigatingHandler = null;
            navigatingHandler = new NavigatingCancelEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) =>
            {
                NavigationFrame.Navigating -= navigatingHandler;

                if (!useDataContext) //we can't access the data context in this event so don't even bother
                {
                    if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh)
                    {
                        if (viewModel == null)
                            viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;

                        viewModel.NavigationService = this;

                        if (lastViewModel != null)
                            e.Cancel = lastViewModel.OnNavigatingFrom(sender, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });

                        viewModel.OnNavigatingTo(sender, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });
                    }
                    //else if (e.NavigationMode == NavigationMode.Back)
                    //{
                    //    e.Cancel = viewModel.OnNavigatingFrom(sender, e);
                    //}
                }
            });

            NavigatedEventHandler navigatedHandler = null;
            navigatedHandler = new NavigatedEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e) =>
            {
                NavigationFrame.Navigated -= navigatedHandler;


                InvokePreNavigatedEvent(new NavigationServicePreNavigatedSignaledEventArgs(viewModel, new CrystalNavigationEventArgs(e)));


                if (e.NavigationMode == NavigationMode.New)
                {
                    if (lastViewModel != null)
                    {
                        lastViewModel.OnNavigatedFrom(sender, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });

                        viewModelBackStack.Push(lastViewModel);
                    }

                    Page page = e.Content as Page;

                    //page.NavigationCacheMode = NavigationCacheMode.Enabled;

                    if (!useDataContext)
                    {
                        page.DataContext = viewModel;
                    }
                    else
                    {
                        viewModel = page.DataContext as ViewModelBase;
                    }

                    if (viewModel == null) throw new Exception();

                    if (viewModel is UIViewModelBase)
                        ((UIViewModelBase)viewModel).UI.SetUIElement(page);

                    //page.SetValue(FrameworkElement.DataContextProperty, viewModel);

                    viewModel.OnNavigatedTo(sender, new CrystalNavigationEventArgs(e) { Direction = ConvertToCrystalNavDirections(e.NavigationMode) });

                    lastViewModel = viewModel;

                }
                //else if (e.NavigationMode == NavigationMode.Back)
                //{
                //    viewModel.OnNavigatedFrom(sender, e);
                //}

                navigationLock.Set();
            });

            navigationLock.Reset();

            NavigationFrame.Navigated += navigatedHandler;
            NavigationFrame.Navigating += navigatingHandler;

            NavigationFrame.Navigate(view, parameter);
        }
    }
}

