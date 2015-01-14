using Crystal2.Model;
using Crystal2.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Crystal2.Navigation
{
    internal class W8NavigationProvider : INavigationProvider
    {
        Frame navigationFrame = null;
        //Dictionary<Type, Uri> navigablePages = null;
        public void Setup(object navigationObject)
        {
            navigationFrame = (Frame)navigationObject;

            navigationFrame.Navigated += navigationFrame_Navigated;
            navigationFrame.Navigating += navigationFrame_Navigating;
            navigationFrame.NavigationFailed += navigationFrame_NavigationFailed;
            navigationFrame.NavigationStopped += navigationFrame_NavigationStopped;
        }

        void navigationFrame_NavigationStopped(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void navigationFrame_NavigationFailed(object sender, Windows.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void navigationFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            ViewModelBase oldViewModel = null;

            if (navigationFrame.Content != null)
            {
                if (((Page)navigationFrame.Content).DataContext != null)
                {
                    oldViewModel = ((ViewModelBase)((Page)navigationFrame.Content).DataContext);

                    e.Cancel = oldViewModel.OnNavigatingFrom();
                }
            }

            NavigatedEventHandler navigatedHandler = null;
            navigatedHandler = new NavigatedEventHandler((o, e2) =>
            {
                Uri uri = GetNavigationUriFromArgs(e2);

                var provider = IOC.IoCManager.Resolve<INavigationDirectoryProvider>();

                var map = provider.ProvideMap();

                Type selectedPageViewModel = (Type)map.First(x =>
                    uri == ((Tuple<Type, Uri, bool>)x.Value).Item2).Key;

                if (((navigationFrame.BackStack.Count > 0 && e2.NavigationMode == NavigationMode.Forward) ||
                    (navigationFrame.ForwardStack.Count > 0 && e2.NavigationMode == NavigationMode.Back))
                    && oldViewModel != null)
                {
                    oldViewModel.OnNavigatedFrom(new CrystalWinRTNavigationEventArgs(null)
                    {
                        Direction = ConvertToCrystalNavigation(e2.NavigationMode),
                    });
                }

                if (navigationFrame.Content != null)
                {
                    if (((Page)navigationFrame.Content).DataContext == null)
                    {
                        ViewModelBase newViewModel = (ViewModelBase)Activator.CreateInstance(selectedPageViewModel);//((ViewModelBase)((Page)e2.Content).DataContext);

                        ((Page)navigationFrame.Content).DataContext = newViewModel;

                        newViewModel.OnNavigatedTo(e2.Parameter, new CrystalWinRTNavigationEventArgs(e2.Parameter)
                        {
                            TargetUri = uri,
                            Direction = ConvertToCrystalNavigation(e.NavigationMode),
                        });
                    }
                    else
                    {
                        ((ViewModelBase)((Page)navigationFrame.Content).DataContext).OnNavigatedTo(e2.Parameter, new CrystalWinRTNavigationEventArgs(e2.Parameter)
                        {
                            TargetUri = uri,
                            Direction = ConvertToCrystalNavigation(e.NavigationMode),
                        });
                    }
                }

                navigationFrame.Navigated -= navigatedHandler;
            });

            navigationFrame.Navigated += navigatedHandler;

            if (Navigating != null)
            {
                Uri uri = GetNavigationUriFromArgs(e);

                Navigating(this, new CrystalWinRTNavigationEventArgs(e.Parameter)
                {
                    TargetUri = uri,
                    Direction = ConvertToCrystalNavigation(e.NavigationMode)
                });
            }
        }

        void navigationFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (Navigated != null)
            {
                Uri uri = GetNavigationUriFromArgs(e);

                Navigated(this, new CrystalWinRTNavigationEventArgs(e.Parameter)
                {
                    TargetUri = uri,
                    Direction = ConvertToCrystalNavigation(e.NavigationMode)
                });
            }

            DetectIfHome();
        }

        private void DetectIfHome()
        {
            //detect if we're on the home page.
            var provider = IOC.IoCManager.Resolve<INavigationDirectoryProvider>();

            var map = provider.ProvideMap();

            Tuple<Type, Uri, bool> selectedPage = (Tuple<Type, Uri, bool>)map.First(x =>
                ((Tuple<Type, Uri, bool>)x.Value).Item1 == ((Page)((Frame)NavigationObject).Content).GetType()).Value;
            IsHome = selectedPage.Item1.GetTypeInfo().GetCustomAttribute<NavigationalLinkForPageToViewModelAttribute>().IsHome;
        }
        private Type GetHomeViewModel()
        {
            var provider = IOC.IoCManager.Resolve<INavigationDirectoryProvider>();

            var map = provider.ProvideMap();

            var homePage = map.First(x =>
                ((Tuple<Type, Uri, bool>)x.Value).Item3 == true);

            return homePage.Key; //view models are the key.
        }

        private static Uri GetNavigationUriFromArgs(object e)
        {
            Type sourcePageType = null;
            if (e is NavigatingCancelEventArgs)
                sourcePageType = ((NavigatingCancelEventArgs)e).SourcePageType;
            else if (e is NavigationEventArgs)
                sourcePageType = ((NavigationEventArgs)e).SourcePageType;

            return GetNavigationUri(sourcePageType);
        }
        private static Uri GetNavigationUri(Type sourcePageType)
        {
            var provider = IOC.IoCManager.Resolve<INavigationDirectoryProvider>();

            var map = provider.ProvideMap();

            Tuple<Type, Uri, bool> selectedPage = (Tuple<Type, Uri, bool>)map.First(x => ((Tuple<Type, Uri, bool>)x.Value).Item1 == sourcePageType).Value;

            return selectedPage.Item2;
        }

        private CrystalNavigationDirection ConvertToCrystalNavigation(Windows.UI.Xaml.Navigation.NavigationMode navigationMode)
        {
            switch (navigationMode)
            {
                case Windows.UI.Xaml.Navigation.NavigationMode.Forward:
                case Windows.UI.Xaml.Navigation.NavigationMode.New:
                    return CrystalNavigationDirection.Forward;
                case Windows.UI.Xaml.Navigation.NavigationMode.Back:
                    return CrystalNavigationDirection.Backward;
                case Windows.UI.Xaml.Navigation.NavigationMode.Refresh:
                    return CrystalNavigationDirection.Reset;
            }

            return CrystalNavigationDirection.None;
        }

        [DebuggerNonUserCode]
        public void Navigate(Crystal2.Navigation.NavigationInformation information, INavigationDirectoryProvider provider)
        {
            var map = provider.ProvideMap();

            Tuple<Type, Uri, bool> selectedPage = (Tuple<Type, Uri, bool>)map.First(x =>
                information.TargetViewModelType == x.Key).Value;
            //information.TargetUri = selectedPage.Item2;

            //NavigatingCancelEventHandler nceh = null;

            //nceh = new NavigatingCancelEventHandler((obj, e) =>
            //{
            //    information.TargetViewModel.OnNavigatingTo();

            //    navigationFrame.Navigating -= nceh;
            //});

            //navigationFrame.Navigating += nceh;

            if (information.Parameter == null)
                navigationFrame.Navigate(selectedPage.Item1);
            else
            {
                if (!(ValueTypeHelper.IsConsideredValueType(information.Parameter)) && CrystalWinRTApplication.Current.applicationConfiguration.AutomaticallyHandleSuspendingAndRestoringState)
                    throw new Exception("Suspension/Restoration is not supported when passing non value-types as navigational parameters. This is a WinRT limitation.");

                navigationFrame.Navigate(selectedPage.Item1, information.Parameter);
            }

            //((Page)navigationFrame.Content).DataContext = information.TargetViewModel;

            //information.TargetViewModel.OnNavigatedTo(information.Parameter);
        }


        public event EventHandler<CrystalNavigationEventArgs> Navigating;

        public event EventHandler<CrystalNavigationEventArgs> Navigated;


        public object NavigationObject
        {
            get { return navigationFrame; }
        }


        public bool CanGoBackward
        {
            get { return ((Frame)NavigationObject).CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return ((Frame)NavigationObject).CanGoForward; }
        }


        public void GoBackward()
        {
            ((Frame)NavigationObject).GoBack();
        }
        public void GoForward()
        {
            ((Frame)NavigationObject).GoForward();
        }

        public ViewModelBase GetCurrentViewModel()
        {
            return ((Page)((Frame)NavigationObject).Content).DataContext as ViewModelBase;
        }

        public void ClearBackStack()
        {
            ((Frame)NavigationObject).BackStack.Clear();
        }


        public void SetNavigationContext(object context)
        {
            ((Frame)navigationFrame).SetNavigationState((string)context);
        }

        public object GetNavigationContext()
        {
            return ((Frame)navigationFrame).GetNavigationState();
        }


        public Uri GetUrl()
        {
            return GetNavigationUri(((Page)((Frame)NavigationObject).Content).GetType());
        }


        public bool IsHome { get; private set; }

        public void GoHome()
        {
            DetectIfHome();
            if (!IsHome)
            {
                var homeViewModel = GetHomeViewModel();
                NavigationManager.NavigateToViewModel(homeViewModel, null);
                NavigationManager.ClearBackStack();
            }
        }


        public void RemoveBackStackEntry()
        {
            var backStack = ((Frame)navigationFrame).BackStack;

            if (backStack.Count > 1)
                backStack.Remove(backStack.First());
        }
    }
}
