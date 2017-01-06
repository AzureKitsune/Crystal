using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public abstract class NavigationServiceBase
    {
        protected ViewModelBase lastViewModel = null;
        protected ManualResetEvent navigationLock = null;

        /// <summary>
        /// The frame level of this service.
        /// </summary>
        public FrameLevel NavigationLevel { get; internal set; } = FrameLevel.None;
        internal NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Returns if the frame can go backward in its back stack.
        /// </summary>
        public abstract bool CanGoBackward { get; }

        protected Stack<ViewModelBase> viewModelBackStack = null;
        protected Stack<ViewModelBase> viewModelForwardStack = null;

        /// <summary>
        /// Returns the current view's view model. Niche usage.
        /// </summary>
        /// <returns></returns>
        public abstract ViewModelBase GetNavigatedViewModel();

        public abstract bool IsNavigatedTo<T>() where T : ViewModelBase;
        public abstract bool IsNavigatedTo(Type viewModelType);

        /// <summary>
        /// Goes backward in the back stack.
        /// </summary>
        public abstract void GoBack();

        public abstract void ClearBackStack();

        public virtual void SafeNavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            if (IsNavigatedTo<T>())
                GetNavigatedViewModel().OnNavigatedTo(this, new CrystalNavigationEventArgs() { Direction = CrystalNavigationDirection.Refresh, Parameter = parameter });
            else
                NavigateTo<T>(parameter);
        }

        public abstract void NavigateTo<T>(object parameter = null) where T : ViewModelBase;

        internal virtual void HandleTerminationReload(NavigationEventArgs args = null) { }

        /// <summary>
        /// A workaround for .NET event's first-subscribe, last-fire approach.
        /// </summary>
        public event EventHandler<NavigationServicePreNavigatedSignaledEventArgs> NavigationServicePreNavigatedSignaled;
        protected void InvokePreNavigatedEvent(NavigationServicePreNavigatedSignaledEventArgs args)
        {
            NavigationServicePreNavigatedSignaled?.Invoke(this, args);
        }

        public event EventHandler<NavigationManagerPreBackRequestedEventArgs> PreBackRequested;
        internal bool SignalPreBackRequested()
        {
            NavigationManagerPreBackRequestedEventArgs eventArgs = new NavigationManagerPreBackRequestedEventArgs();

            if (PreBackRequested != null)
                PreBackRequested(this, eventArgs);

            return eventArgs.Handled;
        }

        protected void SetViewModelUIElement(UIViewModelBase viewModel, FrameworkElement element)
        {
            viewModel?.UI.SetUIElement(element);
        }

        protected void FireViewModelNavigatedToEvent(ViewModelBase viewModel, CrystalNavigationEventArgs args)
        {
            viewModel?.OnNavigatedTo(this, args);
        }

        protected void FireViewModelNavigatedFromEvent(ViewModelBase viewModel, CrystalNavigationEventArgs args)
        {
            viewModel?.OnNavigatedFrom(viewModel, args);
        }

        protected Type GetViewType(Type viewModelType)
        {
            return NavigationManager.GetViewType(viewModelType);
        }
    }
}
