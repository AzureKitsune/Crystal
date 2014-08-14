using Crystal2.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal2.Model
{
    /// <summary>
    /// A ViewModelBase that implements IsBusy and IsBusyStatusText for WinRT platforms.
    /// </summary>
    public abstract class WinRTBusyViewModelBase : WinRTViewModelBase
    {
        public WinRTBusyViewModelBase()
        {
            //Sets the property keys for IsBusy and IsBusyStatusText
            IsBusyPropertyKey = GetProperty("IsBusy");
            IsBusyStatusTextKey = GetProperty("IsBusyStatusText");
            IsBusyProgressValueKey = GetProperty("IsBusyProgressValue");
            IsBusyProgressIndeterminateKey = GetProperty("IsBusyProgressIndeterminate");
        }

        /// <summary>
        /// Keys for IsBusy and IsBusyStatusText
        /// </summary>
        protected ViewModelPropertyKey IsBusyPropertyKey = null;
        protected ViewModelPropertyKey IsBusyStatusTextKey = null;
        protected ViewModelPropertyKey IsBusyProgressValueKey = null;
        protected ViewModelPropertyKey IsBusyProgressIndeterminateKey = null;

        public bool IsBusy
        {
            get { return GetPropertyValue<bool>(IsBusyPropertyKey); }
            protected set
            {
                SetPropertyValue<bool>(IsBusyPropertyKey, value);
            }
        }
        public string IsBusyStatusText
        {
            get { return GetPropertyValue<string>(IsBusyStatusTextKey); }
            protected set { SetPropertyValue<string>(IsBusyStatusTextKey, value); }
        }
        public double? IsBusyProgressValue
        {
            get { return GetPropertyValue<double?>(IsBusyProgressValueKey); }
            protected set { SetPropertyValue<double?>(IsBusyProgressValueKey, value); }
        }
        public bool IsBusyProgressIndeterminate
        {
            get { return GetPropertyValue<bool>(IsBusyProgressIndeterminateKey); }
            protected set { SetPropertyValue<bool>(IsBusyProgressIndeterminateKey, value); }
        }

        protected async Task<Task> WaitForViewLoadAsync(int paddedWaitTimeInMilliseconds = 100)
        {
            INavigationProvider provider = IOC.IoCManager.Resolve<Crystal2.Navigation.INavigationProvider>();

            Frame navigationFrame = provider.NavigationObject as Frame;
            Page currentPage = navigationFrame.Content as Page;

            var contentField = currentPage.GetType().GetTypeInfo().GetDeclaredField("_contentLoaded");

            if ((bool)contentField.GetValue(currentPage))
            {
                await Task.Delay(paddedWaitTimeInMilliseconds);
                return Task.FromResult<object>(null);
            }

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            RoutedEventHandler eh = null;
            eh = new RoutedEventHandler(async (obj, e) =>
            {
                currentPage.Loaded -= eh;

                while (!(bool)contentField.GetValue(currentPage))
                    await Task.Delay(100);

                await Task.Delay(paddedWaitTimeInMilliseconds);
                taskCompletionSource.SetResult(null);
            });
            currentPage.Loaded += eh;

            return taskCompletionSource.Task;
        }
    }
}
