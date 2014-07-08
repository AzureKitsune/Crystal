using Crystal2.Model;
using System;
using System.Collections.Generic;
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
                    uri == ((Tuple<Type, Uri>)x.Value).Item2).Key;

                ViewModelBase newViewModel = (ViewModelBase)Activator.CreateInstance(selectedPageViewModel);//((ViewModelBase)((Page)e2.Content).DataContext);

                if (((navigationFrame.BackStack.Count > 0 && e2.NavigationMode == NavigationMode.Forward) ||
                    (navigationFrame.ForwardStack.Count > 0 && e2.NavigationMode == NavigationMode.Back))
                    && oldViewModel != null)
                    oldViewModel.OnNavigatedFrom();

                if (navigationFrame.Content != null)
                {
                    if (e.NavigationMode == NavigationMode.New)
                        ((Page)navigationFrame.Content).DataContext = newViewModel;

                    newViewModel.OnNavigatedTo(e2.Parameter, new CrystalWinRTNavigationEventArgs(e2.Parameter)
                    {
                        TargetUri = uri,
                        Direction = ConvertToCrystalNavigation(e.NavigationMode),
                    });
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

            Tuple<Type, Uri> selectedPage = (Tuple<Type, Uri>)map.First(x => ((Tuple<Type, Uri>)x.Value).Item1 == sourcePageType).Value;

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


        public void Navigate(Crystal2.Navigation.NavigationInformation information, INavigationDirectoryProvider provider)
        {
            var map = provider.ProvideMap();

            Tuple<Type, Uri> selectedPage = (Tuple<Type, Uri>)map.First(x =>
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
                navigationFrame.Navigate(selectedPage.Item1, information.Parameter);

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
    }
}
