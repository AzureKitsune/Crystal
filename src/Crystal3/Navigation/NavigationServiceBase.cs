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
    public abstract class NavigationServiceBase: IDisposable
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

        public virtual void Reset()
        {

        }

        public virtual void SafeNavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            SafeNavigate(typeof(T), parameter);
        }

        public virtual void SafeNavigate(Type navigationViewModel, object parameter = null)
        {
            if (IsNavigatedTo(navigationViewModel))
            {
                GetNavigatedViewModel().OnNavigatedTo(this,
                    new CrystalNavigationEventArgs() { Direction = CrystalNavigationDirection.Refresh, Parameter = parameter });
            }
            else
            {
                Navigate(navigationViewModel, parameter);
            }
        }

        public abstract void Navigate(Type viewModelType, object parameter = null);
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

        public event EventHandler<CrystalNavigationEventArgs> Navigated;
        protected void InvokeNavigatedEvent(CrystalNavigationEventArgs args)
        {
            Navigated?.Invoke(this, args);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    navigationLock.Dispose();
                    viewModelBackStack?.Clear();
                    viewModelForwardStack?.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null
                lastViewModel = null;


                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NavigationServiceBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
